/*
With this sample, you can set the parameters in the "3D Parameters", "2D Parameters", and "ROI" categories.
*/

using System;
using System.Collections.Generic;
using MMind.Eye;

class SetScanningParameters
{
    static int Main()
    {
        // List all available cameras and connect to a camera by the displayed index.
        var camera = new Camera();
        if (!Utils.FindAndConnect(ref camera))
            return -1;

        // Obtain the basic information of the connected camera.
        CameraInfo cameraInfo = new CameraInfo();
        Utils.ShowError(camera.GetCameraInfo(ref cameraInfo));
        Utils.PrintCameraInfo(cameraInfo);

        var userSetManager = camera.UserSetManager();

        // Obtain the name of the currently selected user set.
        var currentUserSet = userSetManager.CurrentUserSet();
        Console.WriteLine("Current user set: {0}", currentUserSet.GetName());

        // Set the exposure times for acquiring depth information.
        var exposure3DName = MMind.Eye.Scanning3DSetting.ExposureSequence.Name;
        Utils.ShowError(currentUserSet.SetFloatArrayValue(exposure3DName, new List<double> { 5 }));
        //Utils.ShowError(currentUserSet.SetFloatArrayValue(exposure3DName, new List<double> { 5, 10 }));

        // Obtain the current exposure times for acquiring depth information for checking.
        var exposureSequence = new List<double>();
        Utils.ShowError(currentUserSet.GetFloatArrayValue(exposure3DName, ref exposureSequence));
        Console.WriteLine("The 3D scanning exposure multiplier : {0}.", exposureSequence.Count);
        for (int i = 0; i < exposureSequence.Count; ++i)
            Console.WriteLine("3D scanning exposure time {0} : {1} ms.", i + 1, exposureSequence[i]);

        // Set the ROI for the depth map and point cloud, and then obtain the parameter values for
        // checking.
        var roi3DName = MMind.Eye.Scanning3DSetting.ROI.Name;
        var roi = new ROI(0, 0, 500, 500);
        Utils.ShowError(currentUserSet.SetRoiValue(roi3DName, roi));
        Utils.ShowError(currentUserSet.GetRoiValue(roi3DName, ref roi));
        Console.WriteLine("3D Scanning ROI topLeftX : {0}, topLeftY : {1}, width : {2}, height : {3}", roi.UpperLeftX, roi.UpperLeftY, roi.Width, roi.Height);

        // Set the exposure mode and exposure time for capturing the 2D image, and then obtain the
        // parameter values for checking.
        var exposureModeName = MMind.Eye.Scanning2DSetting.ExposureMode.Name;
        var exposureTime2DName = MMind.Eye.Scanning2DSetting.ExposureTime.Name;
        Utils.ShowError(currentUserSet.SetEnumValue(exposureModeName, (int)MMind.Eye.Scanning2DSetting.ExposureMode.Value.Timed));
        Utils.ShowError(currentUserSet.SetFloatValue(exposureTime2DName, 0.1));

        var exposureMode2D = new int();
        double scan2DExposureTime = new double();
        Utils.ShowError(currentUserSet.GetEnumValue(exposureModeName, ref exposureMode2D));
        Utils.ShowError(currentUserSet.GetFloatValue(exposureTime2DName, ref scan2DExposureTime));
        Console.WriteLine("2D scanning exposure mode enum : {0}, exposure time : {1} ms.", exposureMode2D, scan2DExposureTime);

        // Save all the parameter settings to the currently selected user set.
        Utils.ShowError(currentUserSet.SaveAllParametersToDevice());
        Console.WriteLine("save the current parameter settings to the selected user set.");

        camera.Disconnect();
        Console.WriteLine("Disconnected from the camera successfully.");
        Console.WriteLine("Press any key to exit ...");
        Console.ReadKey();
        return 0;
    }
}
