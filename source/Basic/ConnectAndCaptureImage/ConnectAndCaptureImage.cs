/*
With this sample program, you can connect to a camera and obtain the 2D image, depth map and point
cloud data.
*/

using System;
using System.Collections.Generic;
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

    static void printDeviceIntri(DeviceIntri intri)
    {
        Console.WriteLine("Texture Camera Matrix: ");
        Console.WriteLine("    [{0}, 0, {1}]", intri.textureCameraIntri.fx, intri.textureCameraIntri.cx);
        Console.WriteLine("    [0, {0}, {1}]", intri.textureCameraIntri.fy, intri.textureCameraIntri.cy);
        Console.WriteLine("    [0, 0, 1]");
        Console.WriteLine("");
        Console.WriteLine("Texture Camera Distortion Coefficients: ");
        Console.WriteLine("    k1: {0}, k2: {1}, p1: {2}, p2: {3}, k3: {4}", intri.textureCameraIntri.k1, intri.textureCameraIntri.k2, intri.textureCameraIntri.p1, intri.textureCameraIntri.p2, intri.textureCameraIntri.k3);
        Console.WriteLine("");
        Console.WriteLine("Depth Camera Matrix: ");
        Console.WriteLine("    [{0}, 0, {1}]", intri.depthCameraIntri.fx, intri.depthCameraIntri.cx);
        Console.WriteLine("    [0, {0}, {1}]", intri.depthCameraIntri.fy, intri.depthCameraIntri.cy);
        Console.WriteLine("    [0, 0, 1]");
        Console.WriteLine("");
        Console.WriteLine("Depth Camera Distortion Coefficients: ");
        Console.WriteLine("    k1: {0}, k2: {1}, p1: {2}, p2: {3}, k3: {4}", intri.depthCameraIntri.k1, intri.depthCameraIntri.k2, intri.depthCameraIntri.p1, intri.depthCameraIntri.p2, intri.depthCameraIntri.k3);
        Console.WriteLine("");
        Console.WriteLine("Rotation: from Depth Camera to Texture Camera: ");
        Console.WriteLine("    [{0}, {1}, {2}]", intri.textureToDepth.r1, intri.textureToDepth.r2, intri.textureToDepth.r3);
        Console.WriteLine("    [{0}, {1}, {2}]", intri.textureToDepth.r4, intri.textureToDepth.r5, intri.textureToDepth.r6);
        Console.WriteLine("    [{0}, {1}, {2}]", intri.textureToDepth.r7, intri.textureToDepth.r8, intri.textureToDepth.r9);
        Console.WriteLine("Translation: from Depth Camera to Texture Camera: ");
        Console.WriteLine("    X: {0}mm, Y: {1}mm, Z: {2}mm", intri.textureToDepth.x, intri.textureToDepth.y, intri.textureToDepth.z);
    }

    static void printDeviceResolution(DeviceResolution deviceResolution)
    {
        Console.WriteLine("Color map size: (width : {0}, height : {1}).", deviceResolution.colorMapWidth, deviceResolution.colorMapHeight);
        Console.WriteLine("Depth map size: (width : {0}, height : {1}).", deviceResolution.depthMapWidth, deviceResolution.depthMapHeight);
    }

    static int Main()
    {
        Console.WriteLine("Find Mech-Eye device...");
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

        DeviceResolution deviceResolution = new DeviceResolution();
        showError(device.GetDeviceResolution(ref deviceResolution));
        printDeviceResolution(deviceResolution);

        ColorMap color = new ColorMap();
        showError(device.CaptureColorMap(ref color));
        Console.WriteLine("Color map size is width: {0} height: {1}.", color.Width(), color.Height());
        uint row = 0;
        uint col = 0;
        try
        {
            ElementColor colorElem = color.At(row, col);
            Console.WriteLine("Color map element at ({0},{1}) is R: {2} G: {3} B: {4}.", row, col, colorElem.r, colorElem.g, colorElem.b);
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: {0}", e);
            device.Disconnect();
            return 0;
        }

        DepthMap depth = new DepthMap();
        showError(device.CaptureDepthMap(ref depth));
        Console.WriteLine("Depth map size is width: {0} height: {1}.", depth.Width(), depth.Height());
        try
        {
            ElementDepth depthElem = depth.At(row, col);
            Console.WriteLine("Depth map element at ({0},{1}) is depth: {2} mm.", row, col, depthElem.d);
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: {0}", e);
            device.Disconnect();
            return 0;
        }

        PointXYZMap pointXYZMap = new PointXYZMap();
        showError(device.CapturePointXYZMap(ref pointXYZMap));
        Console.WriteLine("Pointcloud Map size is width: {0} height: {1}.", pointXYZMap.Width(), pointXYZMap.Height());
        try
        {
            ElementPointXYZ pointXYZElem = pointXYZMap.At(row, col);
            Console.WriteLine("PointXYZ map element at ({0},{1}) is X:  {2} mm Y:  {3} mm Z:  {4} mm.", row, col, pointXYZElem.x, pointXYZElem.y, pointXYZElem.z);
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: {0}", e);
            device.Disconnect();
            return 0;
        }

        device.Disconnect();
        Console.WriteLine("Disconnected from the Mech-Eye device successfully.");

        return 0;
    }
}

