/*
With this sample, you can acquire the profile data triggered with external device and encoder, generate
and save the intensity image and depth map.
*/

using System;
using MMind.Eye;
using Emgu.CV;
using Emgu.CV.CvEnum;
using System.Threading;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

class TriggerWithExternalDeviceAndEncoder
{
    private static readonly Mutex mut = new Mutex();

    private static void SetTimedExposure(UserSet userSet, int exposureTime)
    {
        // Set the "Exposure Mode" parameter to "Timed"
        Utils.ShowError(userSet.SetEnumValue(
            MMind.Eye.BrightnessSettings.ExposureMode.Name,
            (int)MMind.Eye.BrightnessSettings.ExposureMode.Value.Timed));

        // Set the "Exposure Time" parameter to {exposureTime} μs
        Utils.ShowError(
            userSet.SetIntValue(MMind.Eye.BrightnessSettings.ExposureTime.Name, exposureTime));
    }

    private static void SetHDRExposure(UserSet userSet, int exposureTime, double proportion1, double proportion2, double firstThreshold, double secondThreshold)
    {
        // Set the "Exposure Mode" parameter to "HDR"
        Utils.ShowError(userSet.SetEnumValue(
             MMind.Eye.BrightnessSettings.ExposureMode.Name,
             (int)(MMind.Eye.BrightnessSettings.ExposureMode.Value.HDR)));

        // Set the total exposure time to {exposureTime} μs
        Utils.ShowError(userSet.SetIntValue(MMind.Eye.BrightnessSettings.ExposureTime.Name,
        exposureTime));

        // Set the proportion of the first exposure phase to {proportion1}%
        Utils.ShowError(userSet.SetFloatValue(MMind.Eye.BrightnessSettings.HdrExposureTimeProportion1.Name, proportion1));

        // Set the proportion of the first + second exposure phases to {proportion2}% (that is, the
        // second exposure phase occupies {proportion2 - proportion1}%, and the third exposure phase
        // occupies {100 - proportion2}% of the total exposure time)
        Utils.ShowError(userSet.SetFloatValue(
             MMind.Eye.BrightnessSettings.HdrExposureTimeProportion2.Name, proportion2));

        // Set the first threshold to {firstThreshold}. This limits the maximum grayscale value to
        // {firstThreshold} after the first exposure phase is completed.
        Utils.ShowError(userSet.SetFloatValue(MMind.Eye.BrightnessSettings.HdrFirstThreshold​.Name,
        firstThreshold));

        // Set the second threshold to {secondThreshold}. This limits the maximum grayscale value to
        // {secondThreshold} after the second exposure phase is completed.
        Utils.ShowError(userSet.SetFloatValue(MMind.Eye.BrightnessSettings.HdrSecondThreshold.Name,
        secondThreshold));
    }

    private static void SetEncoderTrigger(UserSet userSet, MMind.Eye.TriggerSettings.EncoderTriggerDirection.Value triggerDirection, MMind.Eye.TriggerSettings.EncoderTriggerSignalCountingMode.Value triggerSignalCountingMode, int triggerInterval)
    {
        // Set the "Line Scan Trigger Source" parameter to "Encoder"
        Utils.ShowError(userSet.SetEnumValue(
            MMind.Eye.TriggerSettings.LineScanTriggerSource.Name,
            (int)(MMind.Eye.TriggerSettings.LineScanTriggerSource.Value.Encoder)));

        // Set the (encoder) "Trigger Direction" parameter to "Both"
        Utils.ShowError(userSet.SetEnumValue(
            MMind.Eye.TriggerSettings.EncoderTriggerDirection.Name,
            (int)(MMind.Eye.TriggerSettings.EncoderTriggerDirection.Value.Both)));

        // Set the (encoder) "Trigger Signal Counting Mode" parameter to "1×"
        Utils.ShowError(userSet.SetEnumValue(
            MMind.Eye.TriggerSettings.EncoderTriggerSignalCountingMode.Name,
            (int)(
                MMind.Eye.TriggerSettings.EncoderTriggerSignalCountingMode.Value.Multiple_1)));

        // Set the (encoder) "Trigger Interval" parameter to 10
        Utils.ShowError(
            userSet.SetIntValue(MMind.Eye.TriggerSettings.EncoderTriggerInterval.Name, 10));
    }

    private static void SetParameters(UserSet userSet)
    {
        // Set the "Exposure Mode" parameter to "Timed"
        // Set the "Exposure Time" parameter to 100 μs
        SetTimedExposure(userSet, 100);

        /*You can also use the HDR exposure mode, in which the laser profiler exposes in three phases
        while acquiring one profile. In this mode, you need to set the total exposure time, the
        proportions of the three exposure phases, as well as the two thresholds of grayscale values. The
        code for setting the relevant parameters for the HDR exposure mode is given in the following
        comments.*/
        // // Set the "Exposure Mode" parameter to "HDR"
        // // Set the total exposure time to 100 μs
        // // Set the proportion of the first exposure phase to 40%
        // // Set the proportion of the first + second exposure phases to 80% (that is, the second
        // // exposure phase occupies 40%, and the third exposure phase occupies 20% of the total
        // // exposure
        // // Set the first threshold to 10. This limits the maximum grayscale value to 10 after the
        // // first exposure phase is completed.
        // // Set the second threshold to 60. This limits the maximum grayscale value to 60 after the
        // // second exposure phase is completed.
        // SetHDRExposure(userSet, 100, 40, 80, 10, 60);

        // Set the "Data Acquisition Trigger Source" parameter to "External"
        Utils.ShowError(userSet.SetEnumValue(
            MMind.Eye.TriggerSettings.DataAcquisitionTriggerSource.Name,
            (int)MMind.Eye.TriggerSettings.DataAcquisitionTriggerSource.Value.External));

        // Set the "Line Scan Trigger Source" parameter to "Encoder"
        // Set the (encoder) "Trigger Direction" parameter to "Both"
        // Set the (encoder) "Trigger Signal Counting Mode" parameter to "1×"
        // Set the (encoder) "Trigger Interval" parameter to 10
        SetEncoderTrigger(userSet, MMind.Eye.TriggerSettings.EncoderTriggerDirection.Value.Both, MMind.Eye.TriggerSettings.EncoderTriggerSignalCountingMode.Value.Multiple_1, 10);

        // Set the "Scan Line Count" parameter (the number of lines to be scanned) to 1600
        Utils.ShowError(
            userSet.SetIntValue(MMind.Eye.ScanSettings.ScanLineCount.Name, 1600));

        // Set the "Laser Power" parameter to 100
        Utils.ShowError(userSet.SetIntValue(MMind.Eye.BrightnessSettings.LaserPower.Name, 100));
        // Set the "Analog Gain" parameter models to "Gain_2"
        Utils.ShowError(userSet.SetEnumValue(
            MMind.Eye.BrightnessSettings.AnalogGain.Name,
            (int)(MMind.Eye.BrightnessSettings.AnalogGain.Value.Gain_2)));
        // Set the "Digital Gain" parameter to 0
        Utils.ShowError(userSet.SetIntValue(MMind.Eye.BrightnessSettings.DigitalGain.Name, 0));


        // Set the "Minimum Grayscale Value" parameter to 50
        Utils.ShowError(
            userSet.SetIntValue(MMind.Eye.ProfileExtraction.MinGrayscaleValue.Name, 50));
        // Set the "Minimum Laser Line Width" parameter to 2
        Utils.ShowError(
            userSet.SetIntValue(MMind.Eye.ProfileExtraction.MinLaserLineWidth.Name, 2));
        // Set the "Maximum Laser Line Width" parameter to 20
        Utils.ShowError(
            userSet.SetIntValue(MMind.Eye.ProfileExtraction.MaxLaserLineWidth.Name, 20));
        // Set the "Spot Selection" parameter to "Strongest"
        Utils.ShowError(userSet.SetEnumValue(
            MMind.Eye.ProfileExtraction.SpotSelection.Name,
            (int)(MMind.Eye.ProfileExtraction.SpotSelection.Value.Strongest)));

        // This parameter is only effective for firmware 2.2.1 and below. For firmware 2.3.0 and above,
        // adjustment of this parameter does not take effect.
        // Set the "Minimum Spot Intensity" parameter to 51
        Utils.ShowError(
            userSet.SetIntValue(MMind.Eye.ProfileExtraction.MinSpotIntensity.Name, 51));
        // This parameter is only effective for firmware 2.2.1 and below. For firmware 2.3.0 and above,
        // adjustment of this parameter does not take effect.
        // Set the "Maximum Spot Intensity" parameter to 205
        Utils.ShowError(
            userSet.SetIntValue(MMind.Eye.ProfileExtraction.MaxSpotIntensity.Name, 205));

        /* Set the "Gap Filling" parameter to 16, which controls the size of the gaps that can be filled in the profile. 
        When the number of consecutive data points in a gap in the profile is no greater than 16, this gap will be filled. */
        Utils.ShowError(userSet.SetIntValue(MMind.Eye.ProfileProcessing.GapFilling.Name, 16));
        /* Set the "Filter" parameter to "Mean". 
        The "Mean Filter Window Size" parameter needs to be set as well. 
        This parameter controls the window size of mean filter. 
        If the "Filter" parameter is set to "Median", the "Median Filter Window Size" parameter needs to be set. 
        This parameter controls the window size of median filter.*/
        Utils.ShowError(userSet.SetEnumValue(
            MMind.Eye.ProfileProcessing.Filter.Name,
            (int)(MMind.Eye.ProfileProcessing.Filter.Value.Mean)));
        // Set the "Mean Filter Window Size" parameter to 2
        Utils.ShowError(userSet.SetEnumValue(
            MMind.Eye.ProfileProcessing.MeanFilterWindowSize.Name,
            (int)(
                MMind.Eye.ProfileProcessing.MeanFilterWindowSize.Value.WindowSize_2)));
    }

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

        SetParameters(userSet);

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

        // // Acquire profile data without using callback
        //if (!AcquireProfileData(profiler, profileBatch, captureLineCount, dataWidth, isSoftwareTrigger))
        //return -1;

        // Acquire the profile data using the callback function
        if (!AcquireProfileDataUsingCallback(profiler, ref profileBatch, isSoftwareTrigger))
            return -1;

        if (profileBatch.CheckFlag(ProfileBatch.BatchFlag.Incomplete))
            Console.WriteLine("Part of the batch's data is lost, the number of valid profiles is: {0}", profileBatch.ValidHeight());

        Console.WriteLine("Save the depth map and the intensity image.");
        SaveMap(profileBatch, "Depth.tiff");
        // profileBatch.GetDepthMap().Save("Depth.tiff"); // Using member function to save the depth map as 4-channels of 8 bits per pixel image.
        profileBatch.GetIntensityImage().Save("Intensity.png");

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
