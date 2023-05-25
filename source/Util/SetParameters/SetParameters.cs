/*
With this sample program, you can set specified parameters to a camera.
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
        status = device.Connect(deviceInfoList[inputIndex], 10000);

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

        List<string> userSets = new List<string>();
        showError(device.GetAllUserSets(ref userSets));

        Console.WriteLine("All user sets : ");
        for (int i = 0; i < userSets.Count; ++i)
        {
            Console.Write(userSets[i]);
            Console.Write("  ");
        }
        Console.WriteLine("");

        string currentUserSet = "";
        showError(device.GetCurrentUserSet(ref currentUserSet));
        Console.WriteLine("Current user set: {0}", currentUserSet);

        showError(device.SetCurrentUserSet(userSets[0]));
        Console.WriteLine("Set \"{0}\" as the current user set.", userSets[0]);
        Console.WriteLine("");

        showError(device.SetScan3DExposure(new List<double> { 59, 0.1, 99 }));

        List<double> exposureSequence = new List<double>();
        showError(device.GetScan3DExposure(ref exposureSequence));

        Console.WriteLine("The 3D scanning exposure multiplier : {0}.", exposureSequence.Count);
        for (int i = 0; i < exposureSequence.Count; ++i)
            Console.WriteLine("3D scanning exposure time {0} : {1} ms.", i + 1, exposureSequence[i]);

        showError(device.SetDepthRange(new DepthRange(100, 1000)));
        DepthRange depthRange = new DepthRange();
        showError(device.GetDepthRange(ref depthRange));
        Console.WriteLine("3D Scanning depth lower limit : {0} mm, depth upper limit : {1} mm.", depthRange.lower, depthRange.upper);

        showError(device.SetScan3DROI(new ROI(0, 0, 500, 500)));
        ROI scan3dRoi = new ROI();
        showError(device.GetScan3DROI(ref scan3dRoi));
        Console.WriteLine("3D Scanning ROI topLeftX : {0}, topLeftY : {1}, width : {2}, height : {3}", scan3dRoi.x, scan3dRoi.y, scan3dRoi.width, scan3dRoi.height);

        showError(device.SetScan2DExposureMode(Scan2DExposureMode.Timed));
        showError(device.SetScan2DExposureTime(999));

        Scan2DExposureMode exposureMode2D = new Scan2DExposureMode();
        double scan2DExposureTime = new double();
        showError(device.GetScan2DExposureMode(ref exposureMode2D));
        showError(device.GetScan2DExposureTime(ref scan2DExposureTime));
        Console.WriteLine("2D scanning exposure mode enum : {0}, exposure time : {1} ms.", exposureMode2D, scan2DExposureTime);

        showError(device.SetCloudSurfaceSmoothingMode(PointCloudSurfaceSmoothing.Normal));
        showError(device.SetCloudNoiseRemovalMode(PointCloudNoiseRemoval.Normal));
        showError(device.SetCloudOutlierRemovalMode(PointCloudOutlierRemoval.Normal));
        showError(device.SetCloudEdgePreservationMode(PointCloudEdgePreservation.Normal));

        PointCloudSurfaceSmoothing surfaceSmoothingMode = new PointCloudSurfaceSmoothing();
        PointCloudNoiseRemoval noiseRemovalMode = new PointCloudNoiseRemoval();
        PointCloudOutlierRemoval outlierRemovalMode = new PointCloudOutlierRemoval();
        PointCloudEdgePreservation edgePreservationMode = new PointCloudEdgePreservation();
        showError(device.GetCloudSurfaceSmoothingMode(ref surfaceSmoothingMode));
        showError(device.GetCloudNoiseRemovalMode(ref noiseRemovalMode));
        showError(device.GetCloudOutlierRemovalMode(ref outlierRemovalMode));
        showError(device.GetCloudEdgePreservationMode(ref edgePreservationMode));

        Console.WriteLine("Cloud surface smoothing mode enum : {0}, cloud edge preservation mode enum : {1}.", surfaceSmoothingMode, edgePreservationMode);
        Console.WriteLine("Cloud noise removal mode enum : {0}, cloud outlier removal mode enum : {1}.", noiseRemovalMode, outlierRemovalMode);

        showError(device.SaveAllSettingsToUserSets());
        Console.WriteLine("save all parammeters to current user set.");

        device.Disconnect();
        Console.WriteLine("Disconnected from the Mech-Eye device successfully.");

        return 0;
    }
}
