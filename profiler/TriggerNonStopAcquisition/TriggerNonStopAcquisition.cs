/*
With this sample, you can acquire profile data triggered non-stop with software, and save the
resulting intensity images and depth maps.
*/

using System;
using MMind.Eye;
using Emgu.CV;
using Emgu.CV.CvEnum;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;

class TriggerNonStopAcquisition
{
    private static readonly Mutex mut = new Mutex();
    private static readonly int kRetrievalFrameCount = 3;

    private static void SetParameters(UserSet userSet)
    {
        // Set the "Data Acquisition Method" parameter to "Nonstop"
        Utils.ShowError(userSet.SetEnumValue(MMind.Eye.TriggerSettings.DataAcquisitionMethod.Name, (int)MMind.Eye.TriggerSettings.DataAcquisitionMethod.Value.Nonstop));

        // // Set the "Data Acquisition Trigger Source" parameter to "Software"
        // Utils.ShowError(userSet.SetEnumValue(
        //     MMind.Eye.TriggerSettings.DataAcquisitionTriggerSource.Name,
        //     (int)MMind.Eye.TriggerSettings.DataAcquisitionTriggerSource.Value.Software));

        // // Set the "Line Scan Trigger Source" parameter to "Fixed rate"
        // Utils.ShowError(userSet.SetEnumValue(
        //      MMind.Eye.TriggerSettings.LineScanTriggerSource.Name,
        //      (int)(MMind.Eye.TriggerSettings.LineScanTriggerSource.Value.FixedRate)));
    }

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
        var callbackCounter = (int)handle.Target;
        if (callbackCounter >= kRetrievalFrameCount)
        {
            mut.ReleaseMutex();
            return;
        }
        var status = batch.GetErrorStatus();
        if (!status.IsOK())
        {
            Console.WriteLine("Callback batch data with error:");
            Utils.ShowError(status);
        }
        if (batch.CheckFlag(ProfileBatch.BatchFlag.Incomplete))
            Console.WriteLine("Part of the batch's data is lost, the number of valid profiles is: {0}", batch.ValidHeight());
        Console.WriteLine("Save the depth map and intensity image.");
        SaveMap(batch, "Depth_" + callbackCounter + ".tiff");
        // batch.GetDepthMap().Save("Depth_" + callbackCounter + ".tiff"); // Using member function to save the depth map as 4-channels of 8 bits per pixel image.
        batch.GetIntensityImage().Save("Intensity_" + callbackCounter + ".png");
        handle.Target = callbackCounter + 1;
        mut.ReleaseMutex();
    }

    private static bool AcquireProfileDataUsingCallback(Profiler profiler, bool isSoftwareTrigger)
    {
        int callbackCounter = 0;

        // Register the callback function
        GCHandle handle = GCHandle.Alloc(callbackCounter);
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
            var currentCount = (int)handle.Target;
            if (currentCount < kRetrievalFrameCount)
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
        handle.Free();
        return status.IsOK();
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

        SetParameters(userSet);

        int dataAcquisitionTriggerSource = 0;
        Utils.ShowError(userSet.GetEnumValue(MMind.Eye.TriggerSettings.DataAcquisitionTriggerSource.Name, ref dataAcquisitionTriggerSource));
        bool isSoftwareTrigger = dataAcquisitionTriggerSource == (int)MMind.Eye.TriggerSettings.DataAcquisitionTriggerSource.Value.Software;

        // Acquire the profile data using the callback function
        if (!AcquireProfileDataUsingCallback(profiler, isSoftwareTrigger))
            return -1;

        // Disconnect from the laser profiler
        profiler.Disconnect();
        Console.WriteLine("Disconnected from the profiler successfully.");
        Console.WriteLine("Press any key to exit ...");
        Console.ReadKey();
        return 0;
    }
}
