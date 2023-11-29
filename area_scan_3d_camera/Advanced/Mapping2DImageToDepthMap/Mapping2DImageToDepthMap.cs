/*
With this sample, you can generate untextured and textured point clouds from a masked 2D image and a depth map.
*/

using System;
using MMind.Eye;

class Mapping2DImageToDepthMap
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

        var frame = new Frame2DAnd3D();
        Utils.ShowError(camera.Capture2DAnd3D(ref frame));
        var color = frame.Frame2D().GetColorImage();
        var depth = frame.Frame3D().GetDepthMap();

        var intrinsics = new CameraIntrinsics();
        Utils.ShowError(camera.GetCameraIntrinsics(ref intrinsics));
        Utils.PrintCameraIntrinsics(intrinsics);

        var width = color.Width();
        var height = color.Height();
        var roi1 = new ROI((uint)width / 5, (uint)height / 5, width / 2, height / 2);
        var roi2 = new ROI((uint)width * 2 / 5, (uint)height * 2 / 5, width / 2, height / 2);

        /**
         *  Generate a mask of the following shape:
         *   ______________________________
         *  |                              |
         *  |                              |
         *  |   *****************          |
         *  |   *****************          |
         *  |   ************************   |
         *  |   ************************   |
         *  |          *****************   |
         *  |          *****************   |
         *  |                              |
         *  |______________________________|
         */

        var textureMask = GenerateTextureMask(width, height, roi1, roi2);

        var pointCloud = new UntexturedPointCloud();
        Utils.ShowError(Mapping2DToDepth.GetPointCloudAfterMapping(depth, textureMask, intrinsics, ref pointCloud));

        var pointCloudPath = "UntexturedPointCloud.ply";
        pointCloud.Save(FileFormat.PLY, pointCloudPath);
        Console.WriteLine("Save the untextured point cloud to file: {0}", pointCloudPath);

        var texturedPointCloud = new TexturedPointCloud();
        Utils.ShowError(Mapping2DToDepth.GetPointCloudAfterMapping(depth, textureMask, color, intrinsics, ref texturedPointCloud));

        string pointCloudBGRPath = "TexturedPointCloud.ply";
        texturedPointCloud.Save(FileFormat.PLY, pointCloudBGRPath);
        Console.WriteLine("Save the textured point cloud to file: {0}", pointCloudBGRPath);

        camera.Disconnect();
        Console.WriteLine("Disconnected from the camera successfully.");
        Console.WriteLine("Press any key to exit ...");
        Console.ReadKey();
        return 0;
    }

    static bool Contains(ROI roi, ulong x, ulong y)
    {
        return x >= roi.UpperLeftX && x <= roi.UpperLeftX + roi.Width && y >= roi.UpperLeftY && y <= roi.UpperLeftY + roi.Height;
    }

    static GrayScale2DImage GenerateTextureMask(ulong width, ulong height, ROI roi1, ROI roi2)
    {
        var mask = new GrayScale2DImage();
        mask.Resize(width, height);
        for (var r = (ulong)0; r < height; ++r)
        {
            for (var c = (ulong)0; c < width; ++c)
            {
                if (Contains(roi1, c, r) || Contains(roi2, c, r))
                {
                    mask.At(r, c).GrayScale = 255;
                }
            }
        }
        return mask;
    }

}
