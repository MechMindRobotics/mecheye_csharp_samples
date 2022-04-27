using System;
using System.Collections.Generic;
using Emgu.CV;
using Emgu.CV.CvEnum;
using mmind.apiSharp;

class sample
{
    static bool isNumber(string str)
    {
        foreach (char c in str)
        {
            if (c >= '0' && c <= '9')
                return true;
        }
        return false;
    }

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
        Console.WriteLine("Find Mech-Eye device :");
        List<MechEyeDeviceInfo> deviceInfoList = MechEyeDevice.enumerateMechEyeDeviceList();

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
        status = device.connect(deviceInfoList[inputIndex]);

        //status = device.connect(deviceInfo);

        if (status.errorCode != (int)ErrorCode.MMIND_STATUS_SUCCESS)
        {
            showError(status);
            return -1;
        }

        Console.WriteLine("Connect Mech-Eye Success.");

        showError(device.setScan3DExposure(new List<double> { 5, 10 }));

        ColorMap color = new ColorMap();
        showError(device.captureColorMap(ref color));
        Mat color8UC3 = new Mat(unchecked((int)color.height()), unchecked((int)color.width()), DepthType.Cv8U, 3, color.data(), unchecked((int)color.width()) * 3);

        PointXYZMap pointXYZMap = new PointXYZMap();
        showError(device.capturePointXYZMap(ref pointXYZMap));
        string pointCloudPath = "pointCloudXYZ.ply";
        string pointCloudColorPath = "pointCloudXYZRGB.ply";
        Mat depth32FC3 = new Mat(unchecked((int)pointXYZMap.height()), unchecked((int)pointXYZMap.width()), DepthType.Cv32F, 3, pointXYZMap.data(), unchecked((int)pointXYZMap.width()) * 12);

        CvInvoke.WriteCloud(pointCloudPath, depth32FC3);
        Console.WriteLine("PointCloudXYZ has : {0} data points.", depth32FC3.Rows * depth32FC3.Cols);
        CvInvoke.WriteCloud(pointCloudColorPath, depth32FC3, color8UC3);
        Console.WriteLine("PointCloudXYZRGB has: {0} data points.", depth32FC3.Rows * depth32FC3.Cols);

        device.disconnect();
        Console.WriteLine("Disconnect Mech-Eye Success.");

        return 0;
    }
}
