/*
With this sample, you can connect to a camera and obtain the 2D image and point cloud data.
*/

using System;
using MMind.Eye;

class ConnectAndCaptureImages
{
    static int Main()
    {
        // List all available cameras and connect to a camera by the displayed index.
        var camera = new Camera();
        if (!Utils.FindAndConnect(ref camera))
            return -1;

        // Obtain the 2D image and depth map resolutions of the camera.
        var cameraResolutions = new CameraResolutions();
        Utils.ShowError(camera.GetCameraResolutions(ref cameraResolutions));
        Utils.PrintCameraResolutions(cameraResolutions);

        // Obtain the 2D image.
        var frame2D = new Frame2D();
        uint row = 0;
        uint col = 0;
        Utils.ShowError(camera.Capture2D(ref frame2D));
        Console.WriteLine("The size of the 2D image is: {0} (width) * {1} (height).", frame2D.ImageSize().Width, frame2D.ImageSize().Height);

        switch (frame2D.GetColorType())
        {
            case Frame2D.ColorTypeOf2DCamera.Color:
                try
                {
                    var colorElem = frame2D.GetColorImage().At(row, col);
                    Console.WriteLine("The RGB values of the pixel at ({0},{1}) is R: {2} G: {3} B: {4}.", row, col, colorElem.R, colorElem.G, colorElem.B);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: {0}", e);
                    camera.Disconnect();
                    return 0;
                }
                break;
            case Frame2D.ColorTypeOf2DCamera.Monochrome:
                try
                {
                    var gray = frame2D.GetGrayScaleImage().At(row, col).GrayScale;
                    Console.WriteLine("The gray scale value of the pixel at ({0},{1}) is Grayscale: {2}", row, col, gray);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: {0}", e);
                    camera.Disconnect();
                    return 0;
                }
                break;
        }

        if (!Utils.ConfirmCapture3D())
        {
            camera.Disconnect();
            return 0;
        }

        // Obtain the depth map.
        var frame3D = new Frame3D();
        Utils.ShowError(camera.Capture3D(ref frame3D));
        var depth = frame3D.GetDepthMap();

        Console.WriteLine("The size of the depth map is: {0} (width) * {1} (height).", depth.Width(), depth.Height());
        try
        {
            PointZ depthElem = depth.At(row, col);
            Console.WriteLine("The depth value of the pixel at ({0},{1}) is: {2} mm.", row, col, depthElem.Z);
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: {0}", e);
            camera.Disconnect();
            return 0;
        }

        // Obtain the point cloud.
        var pointCloud = frame3D.GetUntexturedPointCloud();
        Console.WriteLine("The size of the point cloud is: {0} (width) * {1} (height).", pointCloud.Width(), pointCloud.Height());
        try
        {
            var pointXYZ = pointCloud.At(row, col);
            Console.WriteLine("The coordinates of the point corresponding to the pixel at ({0},{1}) is X:  {2} mm Y:  {3} mm Z:  {4} mm.", row, col, pointXYZ.X, pointXYZ.Y, pointXYZ.Z);
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: {0}", e);
            camera.Disconnect();
            return 0;
        }

        camera.Disconnect();
        Console.WriteLine("Disconnected from the camera successfully.");
        Console.WriteLine("Press any key to exit ...");
        Console.ReadKey();
        return 0;
    }
}

