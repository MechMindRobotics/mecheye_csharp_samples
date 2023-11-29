/*
With this sample, you can generate a point cloud from the depth map and save the point cloud.
*/

using System;
using MMind.Eye;
class ConvertDepthMapToPointCloud
{
    static int Main()
    {
        var camera = new Camera();
        if (!Utils.FindAndConnect(ref camera))
            return -1;

        CameraInfo cameraInfo = new CameraInfo();
        Utils.ShowError(camera.GetCameraInfo(ref cameraInfo));
        Utils.PrintCameraInfo(cameraInfo);

        if (!Utils.ConfirmCapture3D())
        {
            camera.Disconnect();
            return 0;
        }

        var frame = new Frame3D();
        Utils.ShowError(camera.Capture3D(ref frame));
        var depth = frame.GetDepthMap();

        var intrinsics = new CameraIntrinsics();
        Utils.ShowError(camera.GetCameraIntrinsics(ref intrinsics));

        var pointCloudPath = "PointCloud.ply";
        var pointCloud = new UntexturedPointCloud();
        ConvertDepthMapToPointCloudImpl(depth, intrinsics, ref pointCloud);
        pointCloud.Save(FileFormat.PLY, pointCloudPath);
        Console.WriteLine("The point cloud contains : {0} data points.", pointCloud.Width() * pointCloud.Height());
        Console.WriteLine("Save the point cloud to file: {0}", pointCloudPath);

        camera.Disconnect();
        Console.WriteLine("Disconnected from the camera successfully.");
        Console.WriteLine("Press any key to exit ...");
        Console.ReadKey();
        return 0;
    }

    static void ConvertDepthMapToPointCloudImpl(DepthMap depth, CameraIntrinsics intrinsics, ref UntexturedPointCloud xyz)
    {
        xyz.Resize(depth.Width(), depth.Height());

        for (ulong m = 0; m < depth.Height(); ++m)
            for (ulong n = 0; n < depth.Width(); ++n)
            {
                float z;
                try
                {
                    z = depth.At(m, n).Z;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: {0}", e);
                    return;
                }
                xyz.At(m, n).Z = z;
                xyz.At(m, n).X = ((float)n - (float)intrinsics.Depth.CameraMatrix.Cx) * z / (float)intrinsics.Depth.CameraMatrix.Fx;
                xyz.At(m, n).Y = ((float)m - (float)intrinsics.Depth.CameraMatrix.Cy) * z / (float)intrinsics.Depth.CameraMatrix.Fy;
            }
    }
}

