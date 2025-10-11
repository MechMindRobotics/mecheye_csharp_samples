/*
With this sample, you can detect and remove the false data caused by blind spots and obtain the
filtered profile data.

NOTE: The blind spot filtering function can only be applied after all profiles have been retrieved.
Therefore, the profile data must be retrieved with a callback function instead of polling.
*/

using System;
using MMind.Eye;
using Emgu.CV;
using Emgu.CV.CvEnum;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;

class BlindSpotFilter
{
    private static readonly Mutex mut = new Mutex();

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

        // Enable the blind spot filtering function
        userSet.SetBoolValue(MMind.Eye.Filters.EnableBlindSpotFiltering.Name, true);

        // Define a ProfileBatch object to store the profile data
        var profileBatch = new ProfileBatch((ulong)dataWidth);

        int dataAcquisitionTriggerSource = 0;
        Utils.ShowError(userSet.GetEnumValue(MMind.Eye.TriggerSettings.DataAcquisitionTriggerSource.Name, ref dataAcquisitionTriggerSource));
        bool isSoftwareTrigger = dataAcquisitionTriggerSource == (int)MMind.Eye.TriggerSettings.DataAcquisitionTriggerSource.Value.Software;

        // Acquire the profile data using the callback function
        if (!AcquireProfileDataUsingCallback(profiler, ref profileBatch, isSoftwareTrigger))
            return -1;

        if (profileBatch.CheckFlag(ProfileBatch.BatchFlag.Incomplete))
            Console.WriteLine("Part of the batch's data is lost, the number of valid profiles is: {0}", profileBatch.ValidHeight());

        Console.WriteLine("Save the depth map and the intensity image.");
        SaveMap(profileBatch, "Depth.tiff");
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
