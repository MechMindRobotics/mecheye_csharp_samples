/*
With this sample, you can acquire the profile data triggered with software in a fixed rate, generate
and save the profile intensity image, depth map, color depth map ,and point cloud.
*/

using System;
using MMind.Eye;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Drawing;

class RenderDepthMap
{
    private static readonly Mutex mut = new Mutex();

    // Define the callback function for retrieving the profile data
    private static void CallbackFunc(ref ProfileBatch batch, IntPtr pUser)
    {
        mut.WaitOne();
        GCHandle handle = GCHandle.FromIntPtr(pUser);
        var outputBatch = (handle.Target as ProfileBatch);
        outputBatch.Append(batch);
        mut.ReleaseMutex();
    }

    private static bool AcquireProfileDataUsingCallback(Profiler profiler, ref ProfileBatch profileBatch, bool isSoftwareTrigger)
    {
        profileBatch.Clear();

        // Set a large value for CallbackRetrievalTimeout
        Utils.ShowError(profiler.CurrentUserSet().SetIntValue(MMind.Eye.ScanSettings.CallbackRetrievalTimeout.Name, 60000));

        // Register the callback function
        GCHandle handle = GCHandle.Alloc(profileBatch);
        IntPtr param = (IntPtr)handle;
        var status = profiler.RegisterAcquisitionCallback(CallbackFunc, param);
        if (!status.IsOK())
        {
            Utils.ShowError(status);
            return false;
        }

        // Call the startAcquisition to take the laser profiler into the data acquisition status
        // Start data acquisition
        Console.WriteLine("Start data acquisition!");
        status = profiler.StartAcquisition();
        if (!status.IsOK())
        {
            Utils.ShowError(status);
            return false;
        }


        // Call the triggerSoftware to start capturing a frame
        if (isSoftwareTrigger)
        {
            status = profiler.TriggerSoftware();
            if (!status.IsOK())
            {
                Utils.ShowError(status);
                return false;
            }
        }

        while (true)
        {
            mut.WaitOne();
            if (profileBatch.Height() == 0)
            {
                mut.ReleaseMutex();
                Thread.Sleep(500);
            }
            else
            {
                mut.ReleaseMutex();
                break;
            }
        }

        Console.WriteLine("Stop data acquisition.");
        status = profiler.StopAcquisition();
        if (!status.IsOK())
            Utils.ShowError(status);
        return status.IsOK();
    }

    private static void SaveDepthMap(MMind.Eye.ProfileBatch batch, string path)
    {
        if (batch.IsEmpty())
        {
            Console.WriteLine("The depth map cannot be saved because the batch does not contain any profile data.");
            return;
        }
        var depth = batch.GetDepthMap();
        Mat depth32F = new Mat(unchecked((int)depth.Height()), unchecked((int)depth.Width()), DepthType.Cv32F, 1, depth.Data(), unchecked((int)depth.Width()) * 4);
        CvInvoke.Imwrite(path, depth32F);
    }

    private static void SaveRenderedDepthMap(MMind.Eye.ProfileBatch batch, string path)
    {
        if (batch.IsEmpty())
        {
            Console.WriteLine("The color depth map cannot be saved because the batch does not contain any profile data.");
            return;
        }
        var depth = batch.GetDepthMap();
        Mat depth32F = new Mat(unchecked((int)depth.Height()), unchecked((int)depth.Width()), DepthType.Cv32F, 1, depth.Data(), unchecked((int)depth.Width()) * 4);
        var renderedDepth = RenderDepthData(depth32F);
        CvInvoke.Imwrite(path, renderedDepth);
    }

    private static Mat RenderDepthData(Mat depth)
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

    private static bool IsApprox0(double value, double epsilon = 1e-6)
    {
        return Math.Abs(value) < epsilon;
    }

    static int Main()
    {
        var profiler = new Profiler();
        if (!Utils.FindAndConnect(ref profiler))
        {
            Console.ReadKey();
            return -1;
        }

        if (!Utils.ConfirmCapture())
        {
            profiler.Disconnect();
            Console.ReadKey();
            return 0;
        }

        var userSet = profiler.CurrentUserSet();

        int dataWidth = 0;
        // Get the number of data points in each profile
        Utils.ShowError(userSet.GetIntValue(MMind.Eye.ScanSettings.DataPointsPerProfile.Name,
                                             ref dataWidth));
        int captureLineCount = 0;
        // Get the current value of the "Scan Line Count" parameter
        userSet.GetIntValue(MMind.Eye.ScanSettings.ScanLineCount.Name,
                                   ref captureLineCount);

        // Define a ProfileBatch object to store the profile data
        var profileBatch = new ProfileBatch((ulong)dataWidth);

        int dataAcquisitionTriggerSource = 0;
        Utils.ShowError(userSet.GetEnumValue(MMind.Eye.TriggerSettings.DataAcquisitionTriggerSource.Name, ref dataAcquisitionTriggerSource));
        bool isSoftwareTrigger = dataAcquisitionTriggerSource == (int)MMind.Eye.TriggerSettings.DataAcquisitionTriggerSource.Value.Software;

        // // Acquire the profile data using the callback function
        if (!AcquireProfileDataUsingCallback(profiler, ref profileBatch, isSoftwareTrigger))
            return -1;

        Console.WriteLine("Save the depth map and the intensity image.");
        SaveDepthMap(profileBatch, "Depth.tiff");
        SaveRenderedDepthMap(profileBatch, "RenderedDepthMap.tiff");
        profileBatch.GetIntensityImage().Save("Intensity.png");

        // Uncomment the following line to save a virtual device file using the ProfileBatch profileBatch
        // acquired.
        // Utils.ShowError(profiler.SaveVirtualDeviceFile(ref profileBatch, "test.mraw"));

        // Disconnect from the laser profiler
        profiler.Disconnect();
        Console.WriteLine("Disconnected from the profiler successfully.");
        Console.WriteLine("Press any key to exit ...");
        Console.ReadKey();
        return 0;
    }
}
