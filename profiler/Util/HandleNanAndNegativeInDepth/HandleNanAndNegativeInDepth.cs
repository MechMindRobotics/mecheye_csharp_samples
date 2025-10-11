/*
With this sample, you can acquire the profile data triggered with software in a fixed rate, generate and save the intensity image, depth map with the NaN values mapped and an offset applied to remove negative values.
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

using System.Diagnostics;
using System.IO;

class HandleNanAndNegativeInDepth
{
    private static readonly Mutex mut = new Mutex();
    private static readonly float kCustomInvalidDepthValue = 0;
    private static readonly float kCustomOffset = 0;

    private static bool AcquireProfileData(Profiler profiler, ProfileBatch totalBatch, int captureLineCount, int dataWidth, bool isSoftwareTrigger)
    {
        /* Call startAcquisition() to enter the laser profiler into the acquisition ready status, and
        then call triggerSoftware() to start the data acquisition (triggered by software).*/
        Console.WriteLine("Start data acquisition.");
        var status = profiler.StartAcquisition();
        if (!status.IsOK())
        {
            Utils.ShowError(status);
            return false;
        }

        if (isSoftwareTrigger)
        {
            status = profiler.TriggerSoftware();
            if (!status.IsOK())
            {
                Utils.ShowError(status);
                return false;
            }
        }

        totalBatch.Clear();
        totalBatch.Reserve((ulong)captureLineCount);
        while (totalBatch.Height() < (ulong)captureLineCount)
        {
            // Retrieve the profile data
            var batch = new ProfileBatch((ulong)dataWidth);
            status = profiler.RetrieveBatchData(ref batch);
            if (status.IsOK())
            {
                if (!totalBatch.Append(batch))
                    break;
                Thread.Sleep(200);
            }
            else
            {
                Utils.ShowError(status);
                return false;
            }
        }

        Console.WriteLine("Stop data acquisition.");
        status = profiler.StopAcquisition();
        if (!status.IsOK())
            Utils.ShowError(status);
        return status.IsOK();
    }

    // Define the callback function for retrieving the profile data
    private static void CallbackFunc(ref ProfileBatch batch, IntPtr pUser)
    {
        mut.WaitOne();
        if (!batch.GetErrorStatus().IsOK())
        {
            Console.WriteLine("Error occurred during data acquisition");
            Utils.ShowError(batch.GetErrorStatus());
        }
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

    private static void HandleValuesAndSaveDepthMap(MMind.Eye.ProfileBatch batch, string path)
    {
        if (batch.IsEmpty())
        {
            Console.WriteLine("The depth map cannot be saved because the batch does not contain any profile data.");
            return;
        }
        var depth = batch.GetDepthMap();
        Mat depth32F = new Mat(unchecked((int)depth.Height()), unchecked((int)depth.Width()), DepthType.Cv32F, 1, depth.Data(), unchecked((int)depth.Width()) * 4);


        Mat validDepthMask = new Mat(depth32F.Size, DepthType.Cv8U, 1);
        Mat nanMask = new Mat();
        CvInvoke.Compare(depth32F, depth32F, validDepthMask, CmpType.Equal);
        CvInvoke.BitwiseNot(validDepthMask, nanMask);
        double minDepthValue = 0, maxDepthValue = 0;
        Point minLoc = new Point();
        Point maxLoc = new Point();
        CvInvoke.MinMaxLoc(depth32F, ref minDepthValue, ref maxDepthValue, ref minLoc, ref maxLoc, validDepthMask);

        if (minDepthValue < 0)
        {
            depth32F += kCustomOffset == 0 ? -minDepthValue : kCustomOffset;
        }
        depth32F.SetTo(new MCvScalar(kCustomInvalidDepthValue), nanMask);

        CvInvoke.Imwrite(path, depth32F);
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

        // Acquire profile data without using callback
        if (!AcquireProfileData(profiler, profileBatch, captureLineCount, dataWidth, isSoftwareTrigger))
            return -1;

        // // Acquire the profile data using the callback function
        //if (!AcquireProfileDataUsingCallback(profiler, ref profileBatch, isSoftwareTrigger))
        //return -1;

        if (profileBatch.CheckFlag(ProfileBatch.BatchFlag.Incomplete))
            Console.WriteLine("Part of the batch's data is lost, the number of valid profiles is: {0}", profileBatch.ValidHeight());

        Console.WriteLine("Save the depth map and the intensity image.");
        HandleValuesAndSaveDepthMap(profileBatch, "Depth_without_nan_and_negative.tiff");
        // profileBatch.GetDepthMap().Save("Depth.tiff"); // Using member function to save the depth map as 4-channels of 8 bits per pixel image.
        profileBatch.GetIntensityImage().Save("Intensity.png");
        //Utils.SavePointCloud(profileBatch, userSet, isOrganized: true);

        // Uncomment the following line to save a virtual device file using the ProfileBatch profileBatch
        // acquired.
        // var filePath = "test.mraw";
        // byte[] utf16Bytes = Encoding.Unicode.GetBytes(filePath);
        // byte[] utf8Bytes = Encoding.Convert(Encoding.Unicode, Encoding.UTF8, utf16Bytes);
        // var utf8Path = Encoding.Default.GetString(utf8Bytes);
        // Utils.ShowError(profiler.SaveVirtualDeviceFile(ref profileBatch, utf8Path));

        // Disconnect from the laser profiler
        profiler.Disconnect();
        Console.WriteLine("Disconnected from the profiler successfully.");
        Console.WriteLine("Press any key to exit ...");
        Console.ReadKey();
        return 0;
    }
}
