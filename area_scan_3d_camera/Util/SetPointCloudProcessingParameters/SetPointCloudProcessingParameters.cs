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
        var depthSmoothName = MMind.Eye.PointCloudProcessingSetting.DepthSmooth.Name;
        var depthHoleFillingName = MMind.Eye.PointCloudProcessingSetting.DepthHoleFilling.Name;
        var depthSurfaceNoiseRemovalName = MMind.Eye.PointCloudProcessingSetting.DepthSurfaceNoiseRemoval.Name;
        var phaseClusterOutlierRemovalName = MMind.Eye.PointCloudProcessingSetting.PhaseClusterOutlierRemoval.Name;
        var spuriousPhaseRemovalName = MMind.Eye.PointCloudProcessingSetting.SpuriousPhaseRemoval.Name;
        var largeGradNoiseRemovalName = MMind.Eye.PointCloudProcessingSetting.LargeGradNoiseRemoval.Name;
        var outlierRemovalName = MMind.Eye.PointCloudProcessingSetting.OutlierRemoval.Name;
        var edgePreservationName = MMind.Eye.PointCloudProcessingSetting.EdgePreservation.Name;
        Utils.ShowError(currentUserSet.SetEnumValue(surfaceSmoothingName, (int)MMind.Eye.PointCloudProcessingSetting.SurfaceSmoothing.Value.Normal));
        Utils.ShowError(currentUserSet.SetEnumValue(noiseRemovalName, (int)MMind.Eye.PointCloudProcessingSetting.NoiseRemoval.Value.Normal));
        Utils.ShowError(currentUserSet.SetEnumValue(depthSmoothName, (int)MMind.Eye.PointCloudProcessingSetting.DepthSmooth.Value.Normal));
        Utils.ShowError(currentUserSet.SetEnumValue(depthHoleFillingName, (int)MMind.Eye.PointCloudProcessingSetting.DepthHoleFilling.Value.Normal));
        Utils.ShowError(currentUserSet.SetEnumValue(depthSurfaceNoiseRemovalName, (int)MMind.Eye.PointCloudProcessingSetting.DepthSurfaceNoiseRemoval.Value.Normal));
        Utils.ShowError(currentUserSet.SetEnumValue(phaseClusterOutlierRemovalName, (int)MMind.Eye.PointCloudProcessingSetting.PhaseClusterOutlierRemoval.Value.L5));
        Utils.ShowError(currentUserSet.SetEnumValue(spuriousPhaseRemovalName, (int)MMind.Eye.PointCloudProcessingSetting.SpuriousPhaseRemoval.Value.Normal));
        Utils.ShowError(currentUserSet.SetEnumValue(largeGradNoiseRemovalName, (int)MMind.Eye.PointCloudProcessingSetting.LargeGradNoiseRemoval.Value.Normal));
        Utils.ShowError(currentUserSet.SetEnumValue(outlierRemovalName, (int)MMind.Eye.PointCloudProcessingSetting.OutlierRemoval.Value.Normal));
        Utils.ShowError(currentUserSet.SetEnumValue(edgePreservationName, (int)MMind.Eye.PointCloudProcessingSetting.EdgePreservation.Value.Normal));

        var surfaceSmoothingMode = new int();
        var noiseRemovalMode = new int();
        var depthSmoothMode = new int();
        var depthHoleFillingMode = new int();
        var depthSurfaceNoiseRemovalMode = new int();
        var phaseClusterOutlierRemovalMode = new int();
        var spuriousPhaseRemovalMode = new int();
        var largeGradNoiseRemovalMode = new int();
        var outlierRemovalMode = new int();
        var edgePreservationMode = new int();
        Utils.ShowError(currentUserSet.GetEnumValue(surfaceSmoothingName, ref surfaceSmoothingMode));
        Utils.ShowError(currentUserSet.GetEnumValue(noiseRemovalName, ref noiseRemovalMode));
        Utils.ShowError(currentUserSet.GetEnumValue(depthSmoothName, ref depthSmoothMode));
        Utils.ShowError(currentUserSet.GetEnumValue(depthHoleFillingName, ref depthHoleFillingMode));
        Utils.ShowError(currentUserSet.GetEnumValue(depthSurfaceNoiseRemovalName, ref depthSurfaceNoiseRemovalMode));
        Utils.ShowError(currentUserSet.GetEnumValue(phaseClusterOutlierRemovalName, ref phaseClusterOutlierRemovalMode));
        Utils.ShowError(currentUserSet.GetEnumValue(spuriousPhaseRemovalName, ref spuriousPhaseRemovalMode));
        Utils.ShowError(currentUserSet.GetEnumValue(largeGradNoiseRemovalName, ref largeGradNoiseRemovalMode));
        Utils.ShowError(currentUserSet.GetEnumValue(outlierRemovalName, ref outlierRemovalMode));
        Utils.ShowError(currentUserSet.GetEnumValue(edgePreservationName, ref edgePreservationMode));

        Console.WriteLine("Point Cloud Surface Smoothing: {0} (0: Off, 1: Weak, 2: Normal, 3: Strong).", surfaceSmoothingMode);
        Console.WriteLine("Point Cloud Noise Removal: {0} (0: Off, 1: Weak, 2: Normal, 3: Strong).", noiseRemovalMode);
        Console.WriteLine("Depth Smooth: {0} (0: Off, 1: Weak, 2: Normal, 3: Strong).", depthSmoothMode);
        Console.WriteLine("Depth Hole Filling: {0} (0: Off, 1: Weak, 2: Normal, 3: Strong).", depthHoleFillingMode);
        Console.WriteLine("Depth Surface Noise Removal: {0} (0: Off, 1: Weak, 2: Normal, 3: Strong).", depthSurfaceNoiseRemovalMode);
        Console.WriteLine("Phase Cluster Outlier Removal: {0} (0: Off, 1: L1, 2: L2, 3: L3, 4: L4, 5: L5, 6: L6, 7: L7, 8: L8, 9: L9, 10: L10).", phaseClusterOutlierRemovalMode);
        Console.WriteLine("Spurious Phase Removal: {0} (0: Off, 1: Weak, 2: Normal, 3: Strong).", spuriousPhaseRemovalMode);
        Console.WriteLine("Large Gradient Noise Removal: {0} (0: Off, 1: Weak, 2: Normal, 3: Strong).", largeGradNoiseRemovalMode);
        Console.WriteLine("Point Cloud Outlier Removal: {0} (0: Off, 1: Weak, 2: Normal, 3: Strong).", outlierRemovalMode);
        Console.WriteLine("Point Cloud Edge Preservation: {0} (0: Sharp, 1: Normal, 2: Smooth).", edgePreservationMode);

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
