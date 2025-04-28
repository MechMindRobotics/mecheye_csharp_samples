/*
With this sample, you can obtain and save the depth map rendered with the jet color scheme.
*/

using System;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using MMind.Eye;
using System.Drawing;

class RenderDepthMap
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

        var depth32F = new Mat(unchecked((int)depth.Height()), unchecked((int)depth.Width()), DepthType.Cv32F, 1, depth.Data(), unchecked((int)depth.Width()) * 4);
        string renderedDepthFile = "RenderedDepthMap.tiff";
        var renderedDepth = RenderDepthData(depth32F);
        CvInvoke.Imwrite(renderedDepthFile, renderedDepth);
        Console.WriteLine("Save the color depth map : {0}", renderedDepthFile);

        camera.Disconnect();
        Console.WriteLine("Disconnected from the camera successfully.");
        Console.WriteLine("Press any key to exit ...");
        Console.ReadKey();
        return 0;
    }

    static Mat RenderDepthData(Mat depth)
    {
        if (depth.IsEmpty)
            return new Mat();

        Mat mask = new Mat(depth.Size, DepthType.Cv8U, 1);
        CvInvoke.Compare(depth, depth, mask, CmpType.Equal);
        double minDepthValue = 0, maxDepthValue = 0;
        Point minLoc = new Point();
        Point maxLoc = new Point();
        CvInvoke.MinMaxLoc(depth, ref minDepthValue, ref maxDepthValue, ref minLoc, ref maxLoc, mask);

        Mat depth8U = new Mat();
        if (IsApprox0(maxDepthValue - minDepthValue))
        {
            depth.ConvertTo(depth8U, DepthType.Cv8U);
        }
        else
        {
            double alpha = 255.0 / (minDepthValue - maxDepthValue);
            double beta = ((maxDepthValue * 255.0) / (maxDepthValue - minDepthValue)) + 1;
            depth.ConvertTo(depth8U, DepthType.Cv8U, alpha, beta);
        }

        if (depth8U.IsEmpty)
            return new Mat();

        Mat coloredDepth = new Mat();
        CvInvoke.ApplyColorMap(depth8U, coloredDepth, ColorMapType.Jet);

        Image<Emgu.CV.Structure.Bgr, byte> img = coloredDepth.ToImage<Emgu.CV.Structure.Bgr, byte>();
        Image<Emgu.CV.Structure.Gray, byte> imgDepth8U = depth8U.ToImage<Emgu.CV.Structure.Gray, byte>();

        for (int i = 0; i < img.Rows; i++)
        {
            for (int j = 0; j < img.Cols; j++)
            {
                if (imgDepth8U.Data[i, j, 0] == 0)
                {
                    img.Data[i, j, 0] = 0;
                    img.Data[i, j, 1] = 0;
                    img.Data[i, j, 2] = 0;
                }
            }
        }

        return img.Mat;
    }

    static bool IsApprox0(double value, double epsilon = 1e-6)
    {
        return Math.Abs(value) < epsilon;
    }
}
