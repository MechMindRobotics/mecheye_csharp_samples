/*
With this sample, you can obtain and save the depth map.
*/

using System;
using Emgu.CV;
using Emgu.CV.CvEnum;
using MMind.Eye;

class CaptureDepthMap
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

        var frame = new Frame3D();
        Utils.ShowError(camera.Capture3D(ref frame));

        var depth = frame.GetDepthMap();

        // Use Emgu.CV to save the depth map as a single-channel 32-bits-per-pixel image.
        string depthFile = "DepthMap.tiff";
        var depth32F = new Mat(unchecked((int)depth.Height()), unchecked((int)depth.Width()), DepthType.Cv32F, 1, depth.Data(), unchecked((int)depth.Width()) * 4);
        CvInvoke.Imwrite(depthFile, depth32F);
        Console.WriteLine("Capture and save the depth map as a single-channel 32-bits-per-pixel image: {0}", depthFile);

        /* Using DepthMap's member function to save the depth map as a 4-channel 8-bits-per-pixel image.
        A 4-channel 8-bits-per-pixel image may be interpreted as an RGBA color image in most image viewing software applications,
        which cannot display the depth information correctly.
        Therefore, it is recommended to use Emgu.CV to save the depth map.*/
        // string depthFile8UC4 = "DepthMap8UC4.png"
        // depth.Save(depthFile8UC4);
        // Console.WriteLine("Capture and save the depth map as a 4-channel 8-bits-per-pixel image: {0}", depthFile8UC4);

        camera.Disconnect();
        Console.WriteLine("Disconnected from the camera successfully.");
        Console.WriteLine("Press any key to exit ...");
        Console.ReadKey();
        return 0;
    }
}
