/*
With this sample, you can acquire the profile data, generate the intensity image and depth map, and save the images.
*/

using System;
using MMind.Eye;
using Emgu.CV;
using Emgu.CV.CvEnum;
using System.Threading;

class AcquireProfileData
{
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

        var currentUserSet = profiler.CurrentUserSet();
        // Set the "Exposure Mode" parameter to "Timed"
        Utils.ShowError(currentUserSet.SetEnumValue(
            MMind.Eye.BrightnessSettings.ExposureMode.Name,
            (int)(MMind.Eye.BrightnessSettings.ExposureMode.Value.Timed)));

        // Set the "Exposure Time" parameter to 100 μs
        Utils.ShowError(currentUserSet.SetIntValue(MMind.Eye.BrightnessSettings.ExposureTime.Name, 100));

        /*You can also use the HDR exposure mode, in which the laser profiler exposes in three phases while acquiring one profile. In this mode, you need to set the total exsposure time, the proportions of the three exposure phases, as well as the two thresholds of grayscale values. The code for setting the relevant parameters for the HDR exposure
        mode is given in the following comments.*/

        // Set the "Exposure Mode" parameter to "HDR"
        // Utils.ShowError(currentUserSet.SetEnumValue(
        //      MMind.Eye.BrightnessSettings.ExposureMode.Name,
        //      (int)(MMind.Eye.BrightnessSettings.ExposureMode.Value.HDR)));

        // Set the total exposure time to 100 μs
        // Utils.ShowError(currentUserSet.SetIntValue(MMind.Eye.BrightnessSettings.ExposureTime.Name,
        // 100));

        // Set the proportion of the first exposure phase to 40%
        // Utils.ShowError(currentUserSet.SetFloatValue(
        //      MMind.Eye.BrightnessSettings.​HdrExposureTimeProportion1.Name, 40));

        // Set the proportion of the first + second exposure phases to 80% (that is, the second exposure phase occupies 40%, and the third exposure phase occupies 20% of the total exposure time)
        // Utils.ShowError(currentUserSet.SetFloatValue(
        //      MMind.Eye.BrightnessSettings.​HdrExposureTimeProportion2.Name, 80));

        // Set the first threshold to 10. This limits the maximum grayscale value to 10 after the first exposure phase is completed.
        // Utils.ShowError(currentUserSet.SetFloatValue(MMind.Eye.BrightnessSettings.HdrFirstThreshold​.Name,
        // 10));

        // Set the second threshold to 60. This limits the maximum grayscale value to 60 after the second exposure phase is completed.
        // Utils.ShowError(currentUserSet.SetFloatValue(MMind.Eye.BrightnessSettings.HdrSecondThreshold.Name,
        // 60));

        // Set the "Data Acquisition Trigger Source" parameter to "Software"
        Utils.ShowError(currentUserSet.SetEnumValue(
            MMind.Eye.TriggerSettings.DataAcquisitionTriggerSource.Name,
            (int)MMind.Eye.TriggerSettings.DataAcquisitionTriggerSource.Value.Software));

        // Set the "Data Acquisition Trigger Source" parameter to "External"
        // Utils.ShowError(currentUserSet.SetEnumValue(
        //     MMind.Eye.TriggerSettings.DataAcquisitionTriggerSource.Name,
        //     MMind::Eye.TriggerSettings.DataAcquisitionTriggerSource.Value.External));


        // Set the "Line Scan Trigger Source" parameter to "Fixed rate"
        // Utils.ShowError(currentUserSet.SetEnumValue(
        //      MMind.Eye.TriggerSettings.LineScanTriggerSource.Name,
        //      (int)(MMind.Eye.TriggerSettings.LineScanTriggerSource.Value.FixedRate)));

        // Set the "Line Scan Trigger Source" parameter to "Encoder"
        Utils.ShowError(currentUserSet.SetEnumValue(
            MMind.Eye.TriggerSettings.LineScanTriggerSource.Name,
            (int)(MMind.Eye.TriggerSettings.LineScanTriggerSource.Value.Encoder)));

        // Set the (encoder) "Trigger Direction" parameter to "Both"
        Utils.ShowError(currentUserSet.SetEnumValue(
            MMind.Eye.TriggerSettings.EncoderTriggerDirection.Name,
            (int)(MMind.Eye.TriggerSettings.EncoderTriggerDirection.Value.Both)));

        // Set the (encoder) "Trigger Signal Counting Mode" parameter to "1×"
        Utils.ShowError(currentUserSet.SetEnumValue(
            MMind.Eye.TriggerSettings.EncoderTriggerSignalCountingMode.Name,
            (int)(
                MMind.Eye.TriggerSettings.EncoderTriggerSignalCountingMode.Value.Multiple_1)));

        // Set the (encoder) "Trigger Interval" parameter to 10
        Utils.ShowError(
            currentUserSet.SetIntValue(MMind.Eye.TriggerSettings.EncoderTriggerInterval.Name, 10));

        // Set the "Scan Line Count" parameter (the number of lines to be scanned) to 1600
        Utils.ShowError(
            currentUserSet.SetIntValue(MMind.Eye.ScanSettings.ScanLineCount.Name, 1600));

        // Set the "Laser Power" parameter to 100
        Utils.ShowError(currentUserSet.SetIntValue(MMind.Eye.BrightnessSettings.LaserPower.Name, 100));

        // Set the "Analog Gain" parameter models to "Gain_2"
        Utils.ShowError(currentUserSet.SetEnumValue(
            MMind.Eye.BrightnessSettings.AnalogGain.Name,
            (int)(MMind.Eye.BrightnessSettings.AnalogGain.Value.Gain_2)));

        // Set the "Digital Gain" parameter to 0
        Utils.ShowError(currentUserSet.SetIntValue(MMind.Eye.BrightnessSettings.DigitalGain.Name, 0));


        // Set the "Minimum Grayscale Value" parameter to 50
        Utils.ShowError(
            currentUserSet.SetIntValue(MMind.Eye.ProfileExtraction.MinGrayscaleValue.Name, 50));

        // Set the "Minimum Laser Line Width" parameter to 2
        Utils.ShowError(
            currentUserSet.SetIntValue(MMind.Eye.ProfileExtraction.MinLaserLineWidth.Name, 2));

        // Set the "Maximum Laser Line Width" parameter to 20
        Utils.ShowError(
            currentUserSet.SetIntValue(MMind.Eye.ProfileExtraction.MaxLaserLineWidth.Name, 20));

        // This parameter is only effective for firmware 2.2.1 and below. For firmware 2.3.0 and above,
        // adjustment of this parameter does not take effect.
        // Set the "Minimum Spot Intensity" parameter to 51
        Utils.ShowError(
            currentUserSet.SetIntValue(MMind.Eye.ProfileExtraction.MinSpotIntensity.Name, 51));

        // This parameter is only effective for firmware 2.2.1 and below. For firmware 2.3.0 and above,
        // adjustment of this parameter does not take effect.
        // Set the "Maximum Spot Intensity" parameter to 205
        Utils.ShowError(
            currentUserSet.SetIntValue(MMind.Eye.ProfileExtraction.MaxSpotIntensity.Name, 205));

        /* Set the "Gap Filling" parameter to 16, which controls the size of the gaps that can be filled in the profile. 
        When the number of consecutive data points in a gap in the profile is no greater than 16, this gap will be filled. */
        Utils.ShowError(currentUserSet.SetIntValue(MMind.Eye.ProfileProcessing.GapFilling.Name, 16));

        // Set the "Spot Selection" parameter to "Strongest"
        Utils.ShowError(currentUserSet.SetEnumValue(
            MMind.Eye.ProfileExtraction.SpotSelection.Name,
            (int)(MMind.Eye.ProfileExtraction.SpotSelection.Value.Strongest)));

        /* Set the "Filter" parameter to "Mean". 
        The "Mean Filter Window Size" parameter needs to be set as well. 
        This parameter controls the window size of mean filter. 
        If the "Filter" parameter is set to "Median", the "Median Filter Window Size" parameter needs to be set. 
        This parameter controls the window size of median filter.*/
        Utils.ShowError(currentUserSet.SetEnumValue(
            MMind.Eye.ProfileProcessing.Filter.Name,
            (int)(MMind.Eye.ProfileProcessing.Filter.Value.Mean)));

        // Set the "Mean Filter Window Size" parameter to 2
        Utils.ShowError(currentUserSet.SetEnumValue(
            MMind.Eye.ProfileProcessing.MeanFilterWindowSize.Name,
            (int)(
                MMind.Eye.ProfileProcessing.MeanFilterWindowSize.Value.WindowSize_2)));

        int dataWidth = 0;
        // Get the number of data points in each profile
        Utils.ShowError(currentUserSet.GetIntValue(MMind.Eye.ScanSettings.DataPointsPerProfile.Name,
                                             ref dataWidth));
        int captureLineCount = 0;
        // Get the current value of the "Scan Line Count" parameter
        currentUserSet.GetIntValue(MMind.Eye.ScanSettings.ScanLineCount.Name,
                                   ref captureLineCount);

        // Start data acquisition
        Console.WriteLine("Start data acquisition.");

        // Define a ProfileBatch object to store the profile data
        var totalBatch = new ProfileBatch((ulong)dataWidth);

        /* Call startAcquisition() to enter the laser profiler into the acquisition ready status, and
        then call triggerSoftware() to start the data acquisition (triggered by software).*/
        if (profiler.StartAcquisition().IsOK() && profiler.TriggerSoftware().IsOK())
        {
            totalBatch.Reserve((ulong)captureLineCount);
            while (totalBatch.Height() < (ulong)captureLineCount)
            {
                // Retrieve the profile data
                var batch = new ProfileBatch((ulong)dataWidth);
                MMind.Eye.ErrorStatus status = profiler.RetrieveBatchData(ref batch);
                if (status.IsOK())
                {
                    if (!totalBatch.Append(batch))
                        break;
                    Thread.Sleep(200);
                }
                else
                {
                    Utils.ShowError(status);
                    break;
                }
            }
        }
        Console.WriteLine("Save the depth map and the intensity image.");
        SaveMap(totalBatch, "Depth.tiff");
        // totalBatch.GetDepthMap().Save("Depth.tiff"); // Using member function to save the depth map as 4-channels of 8 bits per pixel image.
        totalBatch.GetIntensityImage().Save("Intensity.tiff");

        // Uncomment the following line to save a virtual device file using the ProfileBatch totalBatch
        // acquired.
        // Utils.ShowError(profiler.SaveVirtualDeviceFile(ref totalBatch, "test.mraw"));

        // Disconnect from the laser profiler
        profiler.Disconnect();
        Console.WriteLine("Disconnected from the profiler successfully.");
        Console.WriteLine("Press any key to exit ...");
        Console.ReadKey();
        return 0;
    }
}
