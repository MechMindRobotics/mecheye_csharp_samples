/*
With this sample program, you can obtain and save untextured and textured point clouds (PCL format)
of the objects in the ROI from a camera.
*/

using System;
using System.Collections.Generic;
using Emgu.CV;
using Emgu.CV.CvEnum;
using mmind.apiSharp;

class sample
{
    static void showError(ErrorStatus status)
    {
        if (status.errorCode == (int)ErrorCode.MMIND_STATUS_SUCCESS)
            return;
        Console.WriteLine("Error Code : {0}, Error Description: {1}.", status.errorCode, status.errorDescription);
    }

    static void printDeviceInfo(MechEyeDeviceInfo deviceInfo)
    {
        Console.WriteLine("............................");
        Console.WriteLine("Camera Model Name: {0}", deviceInfo.model);
        Console.WriteLine("Camera ID:         {0}", deviceInfo.id);
        Console.WriteLine("Camera IP Address: {0}", deviceInfo.ipAddress);
        Console.WriteLine("Hardware Version:  V{0}", deviceInfo.hardwareVersion);
        Console.WriteLine("Firmware Version:  V{0}", deviceInfo.firmwareVersion);
        Console.WriteLine("............................");
        Console.WriteLine("");
    }
    static int Main()
    {
        Console.WriteLine("Find Mech-Eye devices...");
        List<MechEyeDeviceInfo> deviceInfoList = MechEyeDevice.EnumerateMechEyeDeviceList();

        if (deviceInfoList.Count == 0)
        {
            Console.WriteLine("No Mech-Eye device found.");
            return -1;
        }

        for (int i = 0; i < deviceInfoList.Count; ++i)
        {
            Console.WriteLine("Mech-Eye device index : {0}", i);
            printDeviceInfo(deviceInfoList[i]);
        }

        Console.WriteLine("Please enter the device index you want to connect: ");
        int inputIndex = 0;

        while (true)
        {
            string input = Console.ReadLine();
            if (int.TryParse(input, out inputIndex) && inputIndex >= 0 && inputIndex < deviceInfoList.Count)
                break;
            Console.WriteLine("Input invalid! Please enter the device index you wnat to connect: ");
        }

        //MechEyeDeviceInfo deviceInfo = new MechEyeDeviceInfo() { model = "", id = "", hardwareVersion = "", firmwareVersion = "1.5.0", ipAddress = "127.0.0.1", port = 5577 };

        ErrorStatus status = new ErrorStatus();
        MechEyeDevice device = new MechEyeDevice();
        status = device.Connect(deviceInfoList[inputIndex], 10000);

        //status = device.Connect(deviceInfo);

        if (status.errorCode != (int)ErrorCode.MMIND_STATUS_SUCCESS)
        {
            showError(status);
            return -1;
        }

        Console.WriteLine("Connected to the Mech-Eye device successfully.");

        //MechEye UHP serials in capture mode 'Merge' does not support this parameter.
        showError(device.SetScan3DROI(new ROI(0, 0, 500, 500)));

        ColorMap color = new ColorMap();

        PointXYZBGRMap xyzbgr = new PointXYZBGRMap();
        showError(device.CapturePointXYZBGRMap(ref xyzbgr));

        color.Resize(xyzbgr.Width(), xyzbgr.Height());

        for (uint i = 0; i < xyzbgr.Height(); i++)
            for (uint j = 0; j < xyzbgr.Width(); j++)
            {
                color.At(i, j).b = xyzbgr.At(i, j).b;
                color.At(i, j).g = xyzbgr.At(i, j).g;
                color.At(i, j).r = xyzbgr.At(i, j).r;
            }

        Mat color8UC3 = new Mat(unchecked((int)color.Height()), unchecked((int)color.Width()), DepthType.Cv8U, 3, color.Data(), unchecked((int)color.Width()) * 3);

        PointXYZMap pointXYZMap = new PointXYZMap();
        showError(device.CapturePointXYZMap(ref pointXYZMap));
        string pointCloudPath = "PointCloudXYZ.ply";
        Mat depth32FC3 = new Mat(unchecked((int)pointXYZMap.Height()), unchecked((int)pointXYZMap.Width()), DepthType.Cv32F, 3, pointXYZMap.Data(), unchecked((int)pointXYZMap.Width()) * 12);

        CvInvoke.WriteCloud(pointCloudPath, depth32FC3);
        Console.WriteLine("PointCloudXYZ has : {0} data points.", depth32FC3.Rows * depth32FC3.Cols);

        device.Disconnect();
        Console.WriteLine("Disconnected from the Mech-Eye device successfully.");

        return 0;
    }
}
