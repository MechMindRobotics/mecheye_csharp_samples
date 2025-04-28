/*
With this sample, you can set multiple exposure times, and then obtain and save the point cloud.
*/

using System;
using System.Collections.Generic;
using MMind.Eye;

class CapturePointCloudHDR
{
    static int Main()
    {
        var camera = new Camera();
        if (!Utils.FindAndConnect(ref camera))
            return -1;

        if (!Utils.ConfirmCapture3D())
        {
            camera.Disconnect();
            return 0;
        }

        // Set the 3D exposure times
        var currentUserSet = camera.CurrentUserSet();
        currentUserSet.SetFloatArrayValue(MMind.Eye.Scanning3DSetting.ExposureSequence.Name, new List<double> { 5, 10 });

        var frame = new Frame3D();
        Utils.ShowError(camera.Capture3D(ref frame));

        var pointCloudFile = "PointCloud.ply";
        var successMessage = "Capture and save the point cloud " + pointCloudFile;
        Utils.ShowError(frame.SaveUntexturedPointCloud(FileFormat.PLY, pointCloudFile), successMessage);

        camera.Disconnect();
        Console.WriteLine("Disconnected from the camera successfully.");
        Console.WriteLine("Press any key to exit ...");
        Console.ReadKey();
        return 0;
    }
}
