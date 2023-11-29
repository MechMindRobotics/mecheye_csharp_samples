/*
With this sample, you can obtain and save the 2D image.
*/

using System;
using MMind.Eye;

class Capture2DImage
{
    static int Main()
    {
        var camera = new Camera();
        if (!Utils.FindAndConnect(ref camera))
            return -1;

        Frame2D frame = new Frame2D();
        Utils.ShowError(camera.Capture2D(ref frame));
        switch (frame.GetColorType())
        {
            case Frame2D.ColorTypeOf2DCamera.Monochrome:
                var gray = frame.GetGrayScaleImage();
                string grayScaleFile = "GrayScale2DImage.png";
                gray.Save(grayScaleFile);
                Console.WriteLine("Capture and save the gray scale 2D image: {0}", grayScaleFile);
                break;
            case Frame2D.ColorTypeOf2DCamera.Color:
                var color = frame.GetColorImage();
                string colorFile = "Color2DImage.png";
                color.Save(colorFile);
                Console.WriteLine("Capture and save the color 2D image: {0}", colorFile);
                break;
        }

        camera.Disconnect();
        Console.WriteLine("Disconnected from the camera successfully.");
        Console.WriteLine("Press any key to exit ...");
        Console.ReadKey();
        return 0;
    }
}
