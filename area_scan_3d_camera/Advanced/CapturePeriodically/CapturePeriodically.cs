/*
With this sample, you can obtain and save 2D images and point clouds
periodically for the specified duration from a camera.
*/

using System;
using System.Threading;
using MMind.Eye;
class CapturePeriodically
{
    static int Main()
    {
        // Set the camera capture interval to 10 seconds and the total duration of image capturing to 5
        // minutes.
        TimeSpan captureTime = new TimeSpan(0, 5, 0);
        TimeSpan capturePeriod = new TimeSpan(0, 0, 10);

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
        Console.WriteLine("Starting capturing for {0} minutes.", captureTime.Minutes);

        DateTime start = DateTime.Now;

        // Perform image capturing periodically according to the set interval for the set total
        // duration.
        while (DateTime.Now - start < captureTime)
        {
            DateTime before = DateTime.Now;
            int time = (int)(before - start).TotalSeconds;

            Capture(camera, time.ToString());

            DateTime after = DateTime.Now;
            TimeSpan timeUsed = after - before;
            if (timeUsed < capturePeriod)
                Thread.Sleep(capturePeriod - timeUsed);
            else
                Console.WriteLine("The actual capture time is longer than the set capture interval. Please increase the capture interval.");
        }

        Console.WriteLine("Capturing for {0} minutes is completed.", captureTime.Minutes);

        camera.Disconnect();
        Console.WriteLine("Disconnected from the camera successfully.");
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

        // Save the obtained data with the set filenames
        var color = frame.Frame2D().GetColorImage();
        color.Save(colorFile);
        Console.WriteLine("Capture and save the 2D image : {0}", colorFile);

        Utils.ShowError(frame.Frame3D().SaveUntexturedPointCloud(FileFormat.PLY, pointCloudPath));
        Console.WriteLine("Capture and save the untextured point cloud : {0}", pointCloudPath);

        Utils.ShowError(frame.SaveTexturedPointCloud(FileFormat.PLY, colorPointCloudPath));
        Console.WriteLine("Capture and save the textured point cloud : {0}", colorPointCloudPath);
    }
}

