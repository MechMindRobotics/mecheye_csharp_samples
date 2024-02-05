/*
With this sample, you can obtain and save the stereo 2D images.
*/

using System;
using MMind.Eye;

class CaptureStereo2DImages
{
    static int Main()
    {
        var camera = new Camera();
        if (!Utils.FindAndConnect(ref camera))
            return -1;

        Frame2D left = new Frame2D();
        Frame2D right = new Frame2D();

        var errorStatus = camera.captureStereo2D(ref left, ref right);
        if (!errorStatus.IsOK())
        {
            Utils.ShowError(errorStatus);
            Console.WriteLine("Press any key to exit ...");
            Console.ReadKey();
            return -1;
        }

        string leftImageFile = "stereo2D_left.png";
        string rightImageFile = "stereo2D_right.png";

        switch (left.GetColorType())
        {
            case Frame2D.ColorTypeOf2DCamera.Monochrome:
                var grayLeft = left.GetGrayScaleImage();
                var grayRight = right.GetGrayScaleImage();

                grayLeft.Save(leftImageFile);
                grayRight.Save(rightImageFile);
                break;
            case Frame2D.ColorTypeOf2DCamera.Color:
                var colorLeft = left.GetColorImage();
                var colorRight = right.GetColorImage();

                colorLeft.Save(leftImageFile);
                colorRight.Save(rightImageFile);
                break;
            default:
                break;
        }
        Console.WriteLine("Capture and save the stereo 2D images: {0} and {1}.", leftImageFile, rightImageFile);

        camera.Disconnect();
        Console.WriteLine("Disconnected from the camera successfully.");
        Console.WriteLine("Press any key to exit ...");
        Console.ReadKey();
        return 0;
    }
}
