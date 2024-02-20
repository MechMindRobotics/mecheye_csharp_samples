/*
With this sample, you can acquire the profile data stored in a virtual device, generate the intensity image and depth map, and save the images.
*/

using System;
using MMind.Eye;
using Emgu.CV;
using Emgu.CV.CvEnum;
using System.Threading;
using System.Runtime.InteropServices;

class UseVirtualDevice
{
    private static readonly Mutex mut = new Mutex();

    private static void SaveMap(MMind.Eye.ProfileBatch batch, string path)
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


    // Define the callback function for retrieving the profile data
    private static void CallbackFunc(ref ProfileBatch batch, IntPtr pUser)
    {
        mut.WaitOne();
        GCHandle handle = GCHandle.FromIntPtr(pUser);
        var outputBatch = (handle.Target as ProfileBatch);
        outputBatch.Append(batch);
        mut.ReleaseMutex();
    }

    static bool AcquireProfileDataWithoutCallback(ref VirtualProfiler profiler)
    {
        var currentUserSet = profiler.CurrentUserSet();

        int dataPoints = 0;
        // Get the number of data points in each profile
        Utils.ShowError(currentUserSet.GetIntValue(MMind.Eye.ScanSettings.DataPointsPerProfile.Name, ref dataPoints));
        int captureLineCount = 0;
        // Get the current value of the "Scan Line Count" parameter
        currentUserSet.GetIntValue(MMind.Eye.ScanSettings.ScanLineCount.Name, ref captureLineCount);

        // Define a ProfileBatch object to store the profile data
        var totalBatch = new ProfileBatch((ulong)dataPoints);

        // Acquire data without using callback
        var status = profiler.StartAcquisition();
        if (!status.IsOK())
        {
            Utils.ShowError(status);
            return false;
        }
        totalBatch.Reserve((ulong)captureLineCount);
        while (totalBatch.Height() < (ulong)captureLineCount)
        {
            // Retrieve the profile data
            var batch = new ProfileBatch((ulong)dataPoints);
            if (profiler.RetrieveBatchData(ref batch).IsOK())
            {
                if (!totalBatch.Append(batch))
                    break;
                Thread.Sleep(100);
            }
            else
            {
                break;
            }
        }

        status = profiler.StopAcquisition();
        if (!status.IsOK())
        {
            Utils.ShowError(status);
            return false;
        }

        Console.WriteLine("Save the depth map and the intensity image.");
        SaveMap(totalBatch, "Depth.tiff");
        // totalBatch.GetDepthMap().Save("Depth.tiff"); // Using member function to save the depth map as 4-channels of 8 bits per pixel image.
        totalBatch.GetIntensityImage().Save("Intensity.png");
        return true;
    }

    static bool AcquireProfileDataWithCallback(ref VirtualProfiler profiler)
    {
        var currentUserSet = profiler.CurrentUserSet();

        int dataPoints = 0;
        // Get the number of data points in each profile
        Utils.ShowError(currentUserSet.GetIntValue(MMind.Eye.ScanSettings.DataPointsPerProfile.Name, ref dataPoints));
        int captureLineCount = 0;
        // Get the current value of the "Scan Line Count" parameter
        currentUserSet.GetIntValue(MMind.Eye.ScanSettings.ScanLineCount.Name, ref captureLineCount);

        // Define a object of ProfileBatch to accommodate profile data
        var profileBatch = new ProfileBatch((ulong)dataPoints);

        // Acquire data with the callback function
        GCHandle handle = GCHandle.Alloc(profileBatch);
        IntPtr param = (IntPtr)handle;
        var status = profiler.RegisterAcquisitionCallback(CallbackFunc, param);
        if (!status.IsOK())
        {
            Utils.ShowError(status);
            return false;
        }

        // Call startAcquisition() to enter the virtual device into the acquisition ready status
        status = profiler.StartAcquisition();
        if (!status.IsOK())
        {
            Utils.ShowError(status);
            return false;
        }


        while (true)
        {
            mut.WaitOne();
            if (profileBatch.Height() == 0)
            {
                mut.ReleaseMutex();
                Thread.Sleep(100);
            }
            else
            {
                mut.ReleaseMutex();
                break;
            }
        }

        status = profiler.StopAcquisition();
        if (!status.IsOK())
        {
            Utils.ShowError(status);
            return false;
        }

        Console.WriteLine("Save the depth map and the intensity image.");
        SaveMap(profileBatch, "DepthByCallback.tiff");
        // profileBatch.GetDepthMap().Save("DepthByCallback.tiff"); // Using member function to save the depth map as 4-channels of 8 bits per pixel image.
        profileBatch.GetIntensityImage().Save("IntensityByCallback.png");
        return true;
    }


    static int Main()
    {
        try
        {
            var profiler = new VirtualProfiler("test.mraw");
            if (!AcquireProfileDataWithoutCallback(ref profiler))
                return -1;
            if (!AcquireProfileDataWithCallback(ref profiler))
                return -1;
            Console.WriteLine("Press any key to exit ...");
            Console.ReadKey();
            return 0;
        }
        catch (Exception)
        {
            return -1;
        }
    }
}
