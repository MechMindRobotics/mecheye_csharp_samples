using System;
using System.Collections.Generic;
using Emgu.CV;
using Emgu.CV.CvEnum;
using mmind.apiSharp;

class captureResultToOpenCV
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
            if (int.TryParse(input, out inputIndex) && inputIndex < deviceInfoList.Count)
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

        DepthMap depth = new DepthMap();
        showError(device.captureDepthMap(ref depth));
        string depthFile = "depthMap.png";
        Mat depth8U = new Mat();
        Mat depth32F = new Mat(unchecked((int)depth.height()), unchecked((int)depth.width()), DepthType.Cv32F, 1, depth.data(), unchecked((int)depth.width()) * 4);
        double minDepth = 1, maxDepth = 1;
        System.Drawing.Point minLoc = new System.Drawing.Point(), maxLoc = new System.Drawing.Point();
        CvInvoke.MinMaxLoc(depth32F, ref minDepth, ref maxDepth, ref minLoc, ref maxLoc);
        depth32F.ConvertTo(depth8U, DepthType.Cv8U, 255.0 / (maxDepth));
        CvInvoke.Imwrite(depthFile, depth8U);
        Console.WriteLine("Capture and save depth image: {0}", depthFile);

        device.disconnect();
        Console.WriteLine("Disconnect Mech-Eye Success.");

        return 0;
    }
}
