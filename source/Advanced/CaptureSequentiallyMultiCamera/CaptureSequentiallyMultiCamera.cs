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
            Console.WriteLine("No Mech-Eye Device found.");
            return -1;
        }

        for (int i = 0; i < deviceInfoList.Count; ++i)
        {
            Console.WriteLine("Mech-Eye device index : {0}", i);
            printDeviceInfo(deviceInfoList[i]);
        }

        HashSet<int> indices = new HashSet<int>();

        while (true)
        {
            Console.WriteLine("Please enter the device index you want to connect: ");
            Console.WriteLine("Enter a c to terminate adding devices");
            int inputIndex = 0;

            string input = Console.ReadLine();
            if (input == "c")
                break;
            if (int.TryParse(input, out inputIndex) && inputIndex >= 0 && inputIndex < deviceInfoList.Count)
                indices.Add(inputIndex);
            else
                Console.WriteLine("Input invalid!");
        }

        List<MechEyeDevice> devices = new List<MechEyeDevice>(indices.Count);
        foreach (int index in indices)
        {
            MechEyeDevice device = new MechEyeDevice();
            showError(device.Connect(deviceInfoList[index]));

            MechEyeDeviceInfo info = new MechEyeDeviceInfo();
            showError(device.GetDeviceInfo(ref info));

            ColorMap color = new ColorMap();
            showError(device.CaptureColorMap(ref color));
            string colorFile = "ColorMap_" + info.id + ".png";
            Mat color8UC3 = new Mat(unchecked((int)color.Height()), unchecked((int)color.Width()), DepthType.Cv8U, 3, color.Data(), unchecked((int)color.Width()) * 3);
            CvInvoke.Imwrite(colorFile, color8UC3);
            Console.WriteLine("Capture and save color image: {0}", colorFile);

            DepthMap depth = new DepthMap();
            showError(device.CaptureDepthMap(ref depth));
            string depthFile = "DepthMap_" + info.id + ".png";
            Mat depth8U = new Mat();
            Mat depth32F = new Mat(unchecked((int)depth.Height()), unchecked((int)depth.Width()), DepthType.Cv32F, 1, depth.Data(), unchecked((int)depth.Width()) * 4);
            double minDepth = 1, maxDepth = 1;
            System.Drawing.Point minLoc = new System.Drawing.Point(), maxLoc = new System.Drawing.Point();
            CvInvoke.MinMaxLoc(depth32F, ref minDepth, ref maxDepth, ref minLoc, ref maxLoc);
            depth32F.ConvertTo(depth8U, DepthType.Cv8U, 255.0 / (maxDepth));
            CvInvoke.Imwrite(depthFile, depth8U);
            Console.WriteLine("Capture and save depth image: {0}", depthFile);

            PointXYZMap pointXYZMap = new PointXYZMap();
            showError(device.CapturePointXYZMap(ref pointXYZMap));
            string pointCloudPath = "PointCloudXYZ_" + info.id + ".ply";
            string pointCloudColorPath = "PointCloudXYZRGB_" + info.id + ".ply";
            Mat depth32FC3 = new Mat(unchecked((int)pointXYZMap.Height()), unchecked((int)pointXYZMap.Width()), DepthType.Cv32F, 3, pointXYZMap.Data(), unchecked((int)pointXYZMap.Width()) * 12);

            CvInvoke.WriteCloud(pointCloudPath, depth32FC3);
            Console.WriteLine("PointCloudXYZ has : {0} data points.", depth32FC3.Rows * depth32FC3.Cols);
            CvInvoke.WriteCloud(pointCloudColorPath, depth32FC3, color8UC3);
            Console.WriteLine("PointCloudXYZRGB has: {0} data points.", depth32FC3.Rows * depth32FC3.Cols);

            device.Disconnect();
        }

        return 0;
    }
}

