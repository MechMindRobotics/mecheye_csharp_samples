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
        status = device.Connect(deviceInfoList[inputIndex]);

        //status = device.Connect(deviceInfo);

        if (status.errorCode != (int)ErrorCode.MMIND_STATUS_SUCCESS)
        {
            showError(status);
            return -1;
        }

        Console.WriteLine("Connected to the Mech-Eye device successfully.");

        MechEyeDeviceInfo deviceInfo = new MechEyeDeviceInfo();
        showError(device.GetDeviceInfo(ref deviceInfo));
        printDeviceInfo(deviceInfo);

        DeviceResolution deviceResolution = new DeviceResolution();
        showError(device.GetDeviceResolution(ref deviceResolution));
        printDeviceResolution(deviceResolution);

        device.Disconnect();
        Console.WriteLine("Disconnected from the Mech-Eye device successfully.");

        return 0;
    }
}

