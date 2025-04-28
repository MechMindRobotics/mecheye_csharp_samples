/*
With this sample, you can obtain and save the untextured and textured point clouds.
*/

using System;
using MMind.Eye;

class CapturePointCloud
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

        var frame = new Frame2DAnd3D();
        Utils.ShowError(camera.Capture2DAnd3D(ref frame));

        var pointCloudFile = "PointCloud.ply";
        var successMessage = "Capture and save the untextured point cloud: " + pointCloudFile;
        Utils.ShowError(frame.Frame3D().SaveUntexturedPointCloud(FileFormat.PLY, pointCloudFile), successMessage);

        var colorPointCloudFile = "TexturedPointCloud.ply";
        successMessage = "Capture and save the textured point cloud: " + colorPointCloudFile;
        Utils.ShowError(frame.SaveTexturedPointCloud(FileFormat.PLY, colorPointCloudFile));

        camera.Disconnect();
        Console.WriteLine("Disconnected from the camera successfully.");
        Console.WriteLine("Press any key to exit ...");
        Console.ReadKey();
        return 0;
    }
}
