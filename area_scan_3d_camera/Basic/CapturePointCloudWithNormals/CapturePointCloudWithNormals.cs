/*
With this sample, you can calculate normals and save the point cloud with normals. The normals can be calculated on the camera or the computer.
*/

using System;
using MMind.Eye;

class CapturePointCloudWithNormals
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

        // Calculate the normals of the points on the camera and save the point cloud with normals to file
        var frame2DAnd3D = new Frame2DAnd3D();
        if (camera.Capture2DAnd3DWithNormal(ref frame2DAnd3D).IsOK())
        {
            Utils.ShowError(frame2DAnd3D.Frame3D().SaveUntexturedPointCloudWithNormals(FileFormat.PLY, "PointCloud_1.ply"));
            Utils.ShowError(frame2DAnd3D.SaveTexturedPointCloudWithNormals(FileFormat.PLY,
                                                                 "TexutredPointCloud_1.ply"));
        }
        else
        {
            Console.WriteLine("Failed to capture the point cloud.");
            camera.Disconnect();
            return -1;
        }

        // Calculate the normals of the points on the computer and save the point cloud with normals to file
        if (camera.Capture2DAnd3D(ref frame2DAnd3D).IsOK())
        {
            Utils.ShowError(frame2DAnd3D.Frame3D().SaveUntexturedPointCloudWithNormals(FileFormat.PLY, "PointCloud_2.ply"));
            Utils.ShowError(frame2DAnd3D.SaveTexturedPointCloudWithNormals(FileFormat.PLY,
                                                                 "TexutredPointCloud_2.ply"));
        }
        else
        {
            Console.WriteLine("Failed to capture the point cloud.");
            camera.Disconnect();
            return -1;
        }

        camera.Disconnect();
        Console.WriteLine("Disconnected from the camera successfully.");
        Console.WriteLine("Press any key to exit ...");
        Console.ReadKey();
        return 0;
    }
}
