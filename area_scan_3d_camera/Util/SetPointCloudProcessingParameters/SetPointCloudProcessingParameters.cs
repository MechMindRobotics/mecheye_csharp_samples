/*
With this sample, you can set the "Point Cloud Processing" parameters.
*/

using System;
using System.Collections.Generic;
using MMind.Eye;

class SetPointCloudProcessingParameters
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

        // Set the "Point Cloud Processing" parameters, and then obtain the parameter values to check if the setting was successful.
        var surfaceSmoothingName = MMind.Eye.PointCloudProcessingSetting.SurfaceSmoothing.Name;
        var noiseRemovalName = MMind.Eye.PointCloudProcessingSetting.NoiseRemoval.Name;
        var outlierRemovalName = MMind.Eye.PointCloudProcessingSetting.OutlierRemoval.Name;
        var edgePreservationName = MMind.Eye.PointCloudProcessingSetting.EdgePreservation.Name;
        Utils.ShowError(currentUserSet.SetEnumValue(surfaceSmoothingName, (int)MMind.Eye.PointCloudProcessingSetting.SurfaceSmoothing.Value.Normal));
        Utils.ShowError(currentUserSet.SetEnumValue(noiseRemovalName, (int)MMind.Eye.PointCloudProcessingSetting.NoiseRemoval.Value.Normal));
        Utils.ShowError(currentUserSet.SetEnumValue(outlierRemovalName, (int)MMind.Eye.PointCloudProcessingSetting.OutlierRemoval.Value.Normal));
        Utils.ShowError(currentUserSet.SetEnumValue(edgePreservationName, (int)MMind.Eye.PointCloudProcessingSetting.EdgePreservation.Value.Normal));

        var surfaceSmoothingMode = new int();
        var noiseRemovalMode = new int();
        var outlierRemovalMode = new int();
        var edgePreservationMode = new int();
        Utils.ShowError(currentUserSet.GetEnumValue(surfaceSmoothingName, ref surfaceSmoothingMode));
        Utils.ShowError(currentUserSet.GetEnumValue(noiseRemovalName, ref noiseRemovalMode));
        Utils.ShowError(currentUserSet.GetEnumValue(outlierRemovalName, ref outlierRemovalMode));
        Utils.ShowError(currentUserSet.GetEnumValue(edgePreservationName, ref edgePreservationMode));

        Console.WriteLine("Point Cloud Surface Smoothing: {0}.", surfaceSmoothingMode);
        Console.WriteLine("Point Cloud Noise Removal: {0}.", noiseRemovalMode);
        Console.WriteLine("Point Cloud Outlier Removal: {0}.", outlierRemovalMode);
        Console.WriteLine("Point Cloud Edge Preservation: {0}.", edgePreservationMode);

        // Save all the parameter settings to the currently selected user set.
        var successMessage = "Save the current parameter settings to the selected user set.";
        Utils.ShowError(currentUserSet.SaveAllParametersToDevice(), successMessage);

        camera.Disconnect();
        Console.WriteLine("Disconnected from the camera successfully.");
        Console.WriteLine("Press any key to exit ...");
        Console.ReadKey();
        return 0;
    }
}
