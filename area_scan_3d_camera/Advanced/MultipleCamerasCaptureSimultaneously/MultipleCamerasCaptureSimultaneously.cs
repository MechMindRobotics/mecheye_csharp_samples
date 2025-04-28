/*
With this sample, you can obtain and save 2D images and point clouds
simultaneously from multiple cameras.
*/

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MMind.Eye;
class MultipleCamerasCaptureSimultaneously
{
    static int Main()
    {
        MultipleCamerasCaptureSimultaneouslyImpl();
        Console.WriteLine("Press any key to exit ...");
        Console.ReadKey();
        return 0;
    }

    static async void MultipleCamerasCaptureSimultaneouslyImpl()
    {
        var cameraList = Utils.FindAndConnectMultiCamera();

        if (cameraList.Count == 0)
        {
            Console.WriteLine("No cameras connected.");
            return;
        }

        if (!Utils.ConfirmCapture3D())
        {
            foreach (var camera in cameraList)
                camera.Disconnect();
            return;
        }

        List<Task> tasks = new List<Task>();
        foreach (var camera in cameraList)
        {
            tasks.Add(Capture(camera));
        }

        await Task.WhenAll(tasks);

        foreach (Camera camera in cameraList)
        {
            camera.Disconnect();
        }
    }

    private static async Task Capture(Camera camera)
    {
        CameraInfo info = new CameraInfo();
        Utils.ShowError(camera.GetCameraInfo(ref info));
        var id = info.SerialNumber;

        var frame = new Frame2DAnd3D();
        Utils.ShowError(camera.Capture2DAnd3D(ref frame));

        // Save the obtained data with the set filenames
        var color = frame.Frame2D().GetColorImage();
        string colorFile = "2DImage_" + id + ".png";
        color.Save(colorFile);
        Console.WriteLine("Capture and save the 2D image: {0}", colorFile);

        string pointCloudPath = "PointCloud_" + id + ".ply";
        var successMessage = "Capture and save the untextured point cloud: " + pointCloudPath;
        Utils.ShowError(frame.Frame3D().SaveUntexturedPointCloud(FileFormat.PLY, pointCloudPath), successMessage);

        string colorPointCloudPath = "TexturedPointCloud_" + id + ".ply";
        successMessage = "Capture and save the textured point cloud: " + colorPointCloudPath;
        Utils.ShowError(frame.SaveTexturedPointCloud(FileFormat.PLY, colorPointCloudPath), successMessage);
    }
}

