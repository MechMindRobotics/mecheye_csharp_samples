using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using mmind.apiSharp;
class Sample
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

    private static async Task capture(MechEyeDevice device)
    {
        MechEyeDeviceInfo info = new MechEyeDeviceInfo();
        showError(device.getDeviceInfo(ref info));

        ColorMap color = new ColorMap();
        showError(device.captureColorMap(ref color));
        string colorFile = "ColorMap_" + info.id + ".png";
        Mat color8UC3 = new Mat(unchecked((int)color.height()), unchecked((int)color.width()), DepthType.Cv8U, 3, color.data(), unchecked((int)color.width()) * 3);
        CvInvoke.Imwrite(colorFile, color8UC3);
        Console.WriteLine("Capture and save color image: {0}", colorFile);

        DepthMap depth = new DepthMap();
        showError(device.captureDepthMap(ref depth));
        string depthFile = "DepthMap_" + info.id + ".png";
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
        string pointCloudPath = "PointCloudXYZ_" + info.id + ".ply";
        string pointCloudColorPath = "PointCloudXYZRGB_" + info.id + ".ply";
        Mat depth32FC3 = new Mat(unchecked((int)pointXYZMap.height()), unchecked((int)pointXYZMap.width()), DepthType.Cv32F, 3, pointXYZMap.data(), unchecked((int)pointXYZMap.width()) * 12);

        CvInvoke.WriteCloud(pointCloudPath, depth32FC3);
        Console.WriteLine("PointCloudXYZ has : {0} data points.", depth32FC3.Rows * depth32FC3.Cols);
        CvInvoke.WriteCloud(pointCloudColorPath, depth32FC3, color8UC3);
        Console.WriteLine("PointCloudXYZRGB has: {0} data points.", depth32FC3.Rows * depth32FC3.Cols);
    }

    static async void captureSimultaneouslyMultiCamera()
    {
        Console.WriteLine("Find Mech-Eye devices...");
        List<MechEyeDeviceInfo> deviceInfoList = MechEyeDevice.enumerateMechEyeDeviceList();

        if (deviceInfoList.Count == 0)
        {
            Console.WriteLine("No Mech-Eye Device found.");
            return;
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
        List<Task> tasks = new List<Task>();
        foreach (int index in indices)
        {
            MechEyeDevice device = new MechEyeDevice();
            showError(device.connect(deviceInfoList[index]));
            tasks.Add(capture(device));
            devices.Add(device);
        }

        await Task.WhenAll(tasks);

        foreach (MechEyeDevice device in devices)
        {
            device.disconnect();
        }
    }


    static int Main()
    {
        captureSimultaneouslyMultiCamera();
        return 0;
    }
}

