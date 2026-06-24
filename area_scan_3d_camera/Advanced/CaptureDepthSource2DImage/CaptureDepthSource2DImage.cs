/*
With this sample, you can obtain and save the depth-source 2D image.
*/

using System;
using MMind.Eye;

class CaptureDepthSource2DImage
{
    static int Main()
    {
        var camera = new Camera();
        if (!Utils.FindAndConnect(ref camera))
            return -1;

        Frame2D frame = new Frame2D();
        var errorStatus = camera.CaptureDepthSource2D(ref frame);
        if (!errorStatus.IsOK())
        {
            Utils.ShowError(errorStatus);
            Console.WriteLine("Press any key to exit ...");
            Console.ReadKey();
            return -1;
        }

        switch (frame.GetColorType())
        {
            case Frame2D.ColorTypeOf2DCamera.Monochrome:
                var gray = frame.GetGrayScaleImage();
                string grayScaleFile = "DepthSourceGrayScale2DImage.png";
                gray.Save(grayScaleFile);
                Console.WriteLine("Capture and save the depth-source gray scale 2D image: {0}", grayScaleFile);
                break;
            case Frame2D.ColorTypeOf2DCamera.Color:
                var color = frame.GetColorImage();
                string colorFile = "DepthSourceColor2DImage.png";
                color.Save(colorFile);
                Console.WriteLine("Capture and save the depth-source color 2D image: {0}", colorFile);
                break;
            default:
                Console.WriteLine("The acquired depth-source 2D image has an unsupported color type.");
                break;
        }

        camera.Disconnect();
        Console.WriteLine("Disconnected from the camera successfully.");
        Console.WriteLine("Press any key to exit ...");
        Console.ReadKey();
        return 0;
    }
}