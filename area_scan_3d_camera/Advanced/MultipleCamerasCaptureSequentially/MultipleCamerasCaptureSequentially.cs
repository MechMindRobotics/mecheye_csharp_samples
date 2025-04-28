/*
With this sample, you can obtain and save 2D images and point clouds
sequentially from multiple cameras.
*/

using System;
using MMind.Eye;
class MultipleCamerasCaptureSequentially
{
    static int Main()
    {
        var cameraList = Utils.FindAndConnectMultiCamera();

        if (cameraList.Count == 0)
        {
            Console.WriteLine("No cameras connected.");
            Console.ReadKey();
            return 0;
        }

        if (!Utils.ConfirmCapture3D())
        {
            foreach (var camera in cameraList)
                camera.Disconnect();
            return 0;
        }

        // Start capturing images
        foreach (var camera in cameraList)
        {
            var cameraInfo = new CameraInfo();
            Utils.ShowError(camera.GetCameraInfo(ref cameraInfo));
            Capture(camera, cameraInfo.SerialNumber);
            camera.Disconnect();
        }

        Console.WriteLine("Press any key to exit ...");
        Console.ReadKey();
        return 0;
    }

    static void Capture(Camera camera, string suffix)
    {
        var colorFile = suffix.Length == 0 ? "2DImage.png" : "2DImage_" + suffix + ".png";
        var pointCloudPath = suffix.Length == 0 ? "PointCloud.ply" : "PointCloud_" + suffix + ".ply";
        var colorPointCloudPath = suffix.Length == 0 ? "TexturedPointCloud.ply" : "TexturedPointCloud_" + suffix + ".ply";

        var frame = new Frame2DAnd3D();
        Utils.ShowError(camera.Capture2DAnd3D(ref frame));

        var color = frame.Frame2D().GetColorImage();
        color.Save(colorFile);
        Console.WriteLine("Capture and save the 2D image: {0}", colorFile);

        var successMessage = "Capture and save the untextured point cloud: {0}" + pointCloudPath;
        Utils.ShowError(frame.Frame3D().SaveUntexturedPointCloud(FileFormat.PLY, pointCloudPath), successMessage);

        successMessage = "Capture and save the textured point cloud: " + colorPointCloudPath;
        Utils.ShowError(frame.SaveTexturedPointCloudWithNormals(FileFormat.PLY, colorPointCloudPath), successMessage);
    }
}

