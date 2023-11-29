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
        Utils.ShowError(frame.Frame3D().SaveUntexturedPointCloud(FileFormat.PLY, pointCloudFile));
        Console.WriteLine("Capture and save the untextured point cloud: {0}", pointCloudFile);

        var colorPointCloudFile = "TexturedPointCloud.ply";
        Utils.ShowError(frame.SaveTexturedPointCloud(FileFormat.PLY, colorPointCloudFile));
        Console.WriteLine("Capture and save the textured point cloud: {0}", colorPointCloudFile);

        camera.Disconnect();
        Console.WriteLine("Disconnected from the camera successfully.");
        Console.WriteLine("Press any key to exit ...");
        Console.ReadKey();
        return 0;
    }
}
