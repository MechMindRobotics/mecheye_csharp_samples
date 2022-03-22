using System;
using System.Collections.Generic;
using mmind.apiSharp;

class sample
{
    static bool isNumber(String str)
    {
        foreach(char c in str)
        {
            if (c >= '0' && c <= '9')
                return true;
        }
        return false;
    }

    static void showError(ErrorStatus status)
    {
        if (status.errorCode == (Int32)ErrorCode.MMIND_STATUS_SUCCESS)
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
        Console.WriteLine("CameraMatrix: ");
        Console.WriteLine("    [{0}, 0, {1}]", intri.fx, intri.cx);
        Console.WriteLine("    [0, {0}, {1}]", intri.fy, intri.cy);
        Console.WriteLine("    [0, 0, 1]");
        Console.WriteLine("");
        Console.WriteLine("CameraDistCoeffs: ");
        Console.WriteLine("    k1: {0}, k2: {1}, p1: {2}, p2: {3}, k3: {4}", intri.k1, intri.k2, intri.p1, intri.p2, intri.k3);
        Console.WriteLine("");
    }

    static void printDeviceResolution(DeviceResolution deviceResolution)
    {
        Console.WriteLine("Color map size: (width : {0}, height : {1}).", deviceResolution.colorMapWidth, deviceResolution.colorMapHeight);
        Console.WriteLine("Depth map size: (width : {0}, height : {1}).", deviceResolution.depthMapWidth, deviceResolution.depthMapHeight);
    }

    static int Main()
    {
        Console.WriteLine("Find Mech-Eye device :");
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
        Int32 inputIndex = 0;

        while (true)
        {
            String input = Console.ReadLine();
            if (Int32.TryParse(input, out inputIndex) && inputIndex < deviceInfoList.Count)
                break;
            Console.WriteLine("Input invalid! Please enter the device index you wnat to connect: ");
        }

        //MechEyeDeviceInfo deviceInfo = new MechEyeDeviceInfo() { model = "", id = "", hardwareVersion = "", firmwareVersion = "1.5.0", ipAddress = "127.0.0.1", port = 5577 };

        ErrorStatus status = new ErrorStatus();
        MechEyeDevice device = new MechEyeDevice();
        status = device.connect(deviceInfoList[inputIndex]);

        //status = device.connect(deviceInfo);

        if (status.errorCode != (Int32)ErrorCode.MMIND_STATUS_SUCCESS)
        {
            showError(status);
            return -1;
        }

        Console.WriteLine("Connect Mech-Eye Success.");

        MechEyeDeviceInfo deviceInfo = new MechEyeDeviceInfo();
        showError(device.getDeviceInfo(ref deviceInfo));
        printDeviceInfo(deviceInfo);

        DeviceIntri deviceIntri = new DeviceIntri();
        showError(device.getDeviceIntri(ref deviceIntri));
        printDeviceIntri(deviceIntri);

        DeviceResolution deviceResolution = new DeviceResolution();
        showError(device.getDeviceResolution(ref deviceResolution));
        printDeviceResolution(deviceResolution);

        ColorMap color = new ColorMap();
        showError(device.captureColorMap(ref color));
        Console.WriteLine("Color map size is width: {0} height: {1}.", color.width(), color.height());
        UInt32 row = 0;
        UInt32 col = 0;
        try
        {
            ElementColor colorElem = color.at(row, col);
            Console.WriteLine("Color map element at ({0},{1}) is R: {2} G: {3} B: {4}.", row, col, colorElem.r, colorElem.g, colorElem.b);
        } catch (Exception e)
        {
            Console.WriteLine("Exception: {0}", e);
            device.disconnect();
            return 0;
        }

        DepthMap depth = new DepthMap();
        showError(device.captureDepthMap(ref depth));
        Console.WriteLine("Depth map size is width: {0} height: {1}.", depth.width(), depth.height());
        try
        {
            ElementDepth depthElem = depth.at(row, col);
            Console.WriteLine("Depth map element at ({0},{1}) is depth: {2} mm.", row, col, depthElem.d);
        } catch (Exception e)
        {
            Console.WriteLine("Exception: {0}", e);
            device.disconnect();
            return 0;
        }

        PointXYZMap pointXYZMap = new PointXYZMap();
        showError(device.capturePointXYZMap(ref pointXYZMap));
        Console.WriteLine("Pointcloud Map size is width: {0} height: {1}.", pointXYZMap.width(), pointXYZMap.height());
        try
        {
            ElementPointXYZ pointXYZElem = pointXYZMap.at(row, col);
            Console.WriteLine("PointXYZ map element at ({0},{1}) is X:  {2} mm Y:  {3} mm Z:  {4} mm.", row, col, pointXYZElem.x, pointXYZElem.y, pointXYZElem.z);
        } catch (Exception e)
        {
            Console.WriteLine("Exception: {0}", e);
            device.disconnect();
            return 0;
        }

        device.disconnect();
        Console.WriteLine("Disconnect Mech-Eye Success.");

        return 0;
    }
}

