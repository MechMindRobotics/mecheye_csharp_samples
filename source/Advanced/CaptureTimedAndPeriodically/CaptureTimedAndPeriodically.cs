﻿using System;
using System.Collections.Generic;
using System.Threading;
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
        TimeSpan captureTime = new TimeSpan(0, 5, 0);
        TimeSpan capturePeriod = new TimeSpan(0, 0, 10);

        Console.WriteLine("Find Mech-Eye devices...");
        List<MechEyeDeviceInfo> deviceInfoList = MechEyeDevice.enumerateMechEyeDeviceList();

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

        Console.WriteLine("Connected to the Mech-Eye device successfully.");

        MechEyeDeviceInfo deviceInfo = new MechEyeDeviceInfo();
        showError(device.getDeviceInfo(ref deviceInfo));
        printDeviceInfo(deviceInfo);

        Console.WriteLine("Starting capturing for {0} minutes.", captureTime.Minutes);

        DateTime start = DateTime.Now;

        while (DateTime.Now - start < captureTime)
        {
            DateTime before = DateTime.Now;
            int time = (before - start).Seconds;

            ColorMap color = new ColorMap();
            showError(device.captureColorMap(ref color));
            string colorFile = "ColorMap_" + time.ToString() + ".png";
            Mat color8UC3 = new Mat(unchecked((int)color.height()), unchecked((int)color.width()), DepthType.Cv8U, 3, color.data(), unchecked((int)color.width()) * 3);
            CvInvoke.Imwrite(colorFile, color8UC3);
            Console.WriteLine("Capture and save color image: {0}", colorFile);

            DepthMap depth = new DepthMap();
            showError(device.captureDepthMap(ref depth));
            string depthFile = "DepthMap_" + time.ToString() + ".png";
            Mat depth8U = new Mat();
            Mat depth32F = new Mat(unchecked((int)depth.height()), unchecked((int)depth.width()), DepthType.Cv32F, 1, depth.data(), unchecked((int)depth.width()) * 4);
            double minDepth = 1, maxDepth = 1;
            System.Drawing.Point minLoc = new System.Drawing.Point(), maxLoc = new System.Drawing.Point();
            CvInvoke.MinMaxLoc(depth32F, ref minDepth, ref maxDepth, ref minLoc, ref maxLoc);
            depth32F.ConvertTo(depth8U, DepthType.Cv8U, 255.0 / (maxDepth));
            CvInvoke.Imwrite(depthFile, depth8U);
            Console.WriteLine("Capture and save depth image: {0}", depthFile);

            PointXYZMap pointXYZMap = new PointXYZMap();
            showError(device.capturePointXYZMap(ref pointXYZMap));
            string pointCloudPath = "PointCloudXYZ_" + time.ToString() + ".ply";
            string pointCloudColorPath = "PointCloudXYZRGB_" + time.ToString() + ".ply";
            Mat depth32FC3 = new Mat(unchecked((int)pointXYZMap.height()), unchecked((int)pointXYZMap.width()), DepthType.Cv32F, 3, pointXYZMap.data(), unchecked((int)pointXYZMap.width()) * 12);

            CvInvoke.WriteCloud(pointCloudPath, depth32FC3);
            Console.WriteLine("PointCloudXYZ has : {0} data points.", depth32FC3.Rows * depth32FC3.Cols);
            CvInvoke.WriteCloud(pointCloudColorPath, depth32FC3, color8UC3);
            Console.WriteLine("PointCloudXYZRGB has: {0} data points.", depth32FC3.Rows * depth32FC3.Cols);

            DateTime after = DateTime.Now;
            TimeSpan timeUsed = after - before;
            if (timeUsed < capturePeriod)
                Thread.Sleep(capturePeriod - timeUsed);
            else
                Console.WriteLine("Your capture time is longer than your capture period. Please increase your capture period.");
        }

        Console.WriteLine("Capturing completed for {0} minutes.", captureTime.Minutes);

        device.disconnect();
        Console.WriteLine("Disconnected from the Mech-Eye device successfully.");

        return 0;
    }
}

