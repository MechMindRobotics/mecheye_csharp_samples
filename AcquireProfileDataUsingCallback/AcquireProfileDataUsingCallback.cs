/*
This is a simple example of how to find and connect an available Mech-Eye LNX Device
and then capture a batch line data with trigger source of software.
*/

using System;
using MMind.Eye;
using Emgu.CV;
using Emgu.CV.CvEnum;
using System.Threading;
using System.Runtime.InteropServices;

class AcquireProfileDataUsingCallback
{
    private static readonly Mutex mut = new Mutex();

    private static void SaveMap(MMind.Eye.ProfileBatch batch, string path)
    {
        var depth = batch.GetDepthMap();
        Mat depth32F = new Mat(unchecked((int)depth.Height()), unchecked((int)depth.Width()), DepthType.Cv32F, 1, depth.Data(), unchecked((int)depth.Width()) * 4);
        CvInvoke.Imwrite(path, depth32F);
    }

    private static void CallbackFunc(ref ProfileBatch batch, IntPtr pUser)
    {
        mut.WaitOne();
        GCHandle handle = GCHandle.FromIntPtr(pUser);
        var outputBatch = (handle.Target as ProfileBatch);
        outputBatch.Append(batch);
        mut.ReleaseMutex();
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

        MMind.Eye.UserSet currentUserSet = profiler.CurrentUserSet();
        // Set the exposure mode to Timed
        Utils.ShowError(currentUserSet.SetEnumValue(
            MMind.Eye.BrightnessSettings.ExposureMode.Name,
            (int)(MMind.Eye.BrightnessSettings.ExposureMode.Value.Timed)));

        // Set the exposure time to 100 μs
        Utils.ShowError(currentUserSet.SetIntValue(MMind.Eye.BrightnessSettings.ExposureTime.Name, 100));

        /*The other option for the exposure mode is HDR, in which three exposure times and the
        y-coordinates of two knee points must be Set.
        /*The code for Setting the relevant parameters for the HDR exposure mode is given in the
        following notes. */

        /*Set the the exposure sequence for the HDR exposure mode. The exposure sequence contains three
         * exposure times, 100, 10, and 4. The total exposure time is the sum of the three exposure
         * times, which is 114 in this case.*/
        /*The y-coordinates of the two knee points are Set to 10 and 60.*/
        // Utils.ShowError(currentUserSet.SetEnumValue(
        //     MMind.Eye.BrightnessSettings.ExposureMode.Name,
        //     (int)(MMind.Eye.BrightnessSettings.ExposureMode.Value.HDR)));
        // Utils.ShowError(currentUserSet.SetIntValue(MMind.Eye.BrightnessSettings.ExposureTime.Name, 114));
        // Utils.ShowError(currentUserSet.SetFloatValue(MMind.Eye.BrightnessSettings.HdrExposureTimeProportion1.Name, 87.7193));
        // Utils.ShowError(currentUserSet.SetFloatValue(MMind.Eye.BrightnessSettings.HdrExposureTimeProportion2.Name, 96.4912));
        // Set HDR dual slope to 10
        // Utils.ShowError(currentUserSet.SetFloatValue(MMind.Eye.BrightnessSettings.HdrFirstThreshold.Name ,10));
        // Set HDR triple slope to 60
        // Utils.ShowError(currentUserSet.SetFloatValue(MMind.Eye.BrightnessSettings.HdrSecondThreshold.Name
        // ,60));

        // Set the trigger source to Software
        Utils.ShowError(currentUserSet.SetEnumValue(
            MMind.Eye.TriggerSettings.LineScanTriggerSource.Name,
            (int)(MMind.Eye.TriggerSettings.LineScanTriggerSource.Value.FixedRate)));

        // Set the maximum number of lines to be scanned to 1600
        Utils.ShowError(currentUserSet.SetIntValue(MMind.Eye.ScanSettings.ScanLineCount.Name, 1600));
        // Set the laser power level to 100
        Utils.ShowError(
            currentUserSet.SetIntValue(MMind.Eye.BrightnessSettings.LaserPower.Name, 100));

        MMind.Eye.ProfilerInfo profilerInfo = new ProfilerInfo();
        if (profiler.GetProfilerInfo(ref profilerInfo).IsOK())
        {
            // Set the analog gain to 1.3
            if (profilerInfo.Model == "Mech-Eye LNX 8030")
            {
                Utils.ShowError(currentUserSet.SetEnumValue(
                    MMind.Eye.BrightnessSettings.AnalogGainFor8030.Name,
                    (int)(MMind.Eye.BrightnessSettings.AnalogGainFor8030.Value.Gain_1_3)));
            }
            else
            {
                Utils.ShowError(currentUserSet.SetEnumValue(
                    MMind.Eye.BrightnessSettings.AnalogGain.Name,
                    (int)(MMind.Eye.BrightnessSettings.AnalogGain.Value.Gain_1_3)));
            }
        }

        // Set the digital gain to 0
        Utils.ShowError(currentUserSet.SetIntValue(MMind.Eye.BrightnessSettings.DigitalGain.Name, 0));
        // Set the grayscale value threshold to 50
        Utils.ShowError(currentUserSet.SetIntValue(MMind.Eye.ProfileExtraction.MinGrayscaleValue.Name, 50));
        // Set the minimun laser line width to 2
        Utils.ShowError(currentUserSet.SetIntValue(MMind.Eye.ProfileExtraction.MinLaserLineWidth.Name, 2));
        // Set the maximum laser line width to 20
        Utils.ShowError(currentUserSet.SetIntValue(MMind.Eye.ProfileExtraction.MaxLaserLineWidth.Name, 20));
        // Set the minimum laser line intensity to 10
        Utils.ShowError(currentUserSet.SetIntValue(MMind.Eye.ProfileExtraction.MinSpotIntensity.Name, 51));
        // Set the maximum laser line intensity to 205
        Utils.ShowError(
            currentUserSet.SetIntValue(MMind.Eye.ProfileExtraction.MaxSpotIntensity.Name, 205));
        /* Set the maximum number of invalid points to be interpolated to 16. If the number of continous
         * invalid points is less than or equal to 16, these points will be filled */
        Utils.ShowError(currentUserSet.SetIntValue(
            MMind.Eye.ProfileProcessing.GapFilling.Name, 16));
        // Set the profile spot selection to Strongest
        Utils.ShowError(currentUserSet.SetEnumValue(MMind.Eye.ProfileExtraction.SpotSelection.Name,
            (int)(MMind.Eye.ProfileExtraction.SpotSelection.Value.Strongest)));

        /* Set the filter type to Mean. When the filter type is Set to Mean or
         * MeanEdgePreserving, SetLnxMeanFilterWindow can be called to Set the window size for
         * mean filtering. When the filter type is Set to Median,
         * SetLnxMedianFilterWindow can be called to Set the window size for median filtering.*/
        Utils.ShowError(currentUserSet.SetEnumValue(
            MMind.Eye.ProfileProcessing.Filter.Name,
            (int)(MMind.Eye.ProfileProcessing.Filter.Value.Mean)));
        // Set the window size for mean filtering to WindowSize_2
        Utils.ShowError(currentUserSet.SetEnumValue(
            MMind.Eye.ProfileProcessing.MeanFilterWindowSize.Name,
            (int)(
                MMind.Eye.ProfileProcessing.MeanFilterWindowSize.Value.WindowSize_2)));

        int dataPoints = 0;
        // Get the line width in the X direction
        Utils.ShowError(currentUserSet.GetIntValue(MMind.Eye.ScanSettings.DataPointsPerProfile.Name, ref dataPoints));
        int captureLineCount = 0;
        // Get the current maximum number of lines to be scanned
        currentUserSet.GetIntValue(MMind.Eye.ScanSettings.ScanLineCount.Name, ref captureLineCount);

        var profileBatch = new ProfileBatch((ulong)dataPoints);

        // Start scanning
        Console.WriteLine("Start scanning!");
        GCHandle handle = GCHandle.Alloc(profileBatch);
        IntPtr param = (IntPtr)handle;
        if (profiler.RegisterAcquisitionCallback(CallbackFunc, param).IsOK())
        {
            var status = profiler.StartAcquisition();
            if (!status.IsOK())
            {
                Utils.ShowError(status);
                return -1;
            }

            status = profiler.TriggerSoftware();
            if (!status.IsOK())
            {
                Utils.ShowError(status);
                return -1;
            }
            Console.WriteLine("triggerOnce successfully!");
            while (true)
            {
                mut.WaitOne();
                if (profileBatch.Height() == 0)
                {
                    mut.ReleaseMutex();
                    Thread.Sleep(1000);
                }
                else
                {
                    mut.ReleaseMutex();
                    break;
                }
            }

            SaveMap(profileBatch, "DepthByCallback.tiff");
            profileBatch.GetIntensityImage().Save("IntensityByCallback.tiff");
            profileBatch.Clear();
        }

        // Disconnect from the profiler
        profiler.Disconnect();
        Console.WriteLine("Disconnected from the Mech-Eye profiler successfully.");
        Console.WriteLine("Press any key to exit ...");
        Console.ReadKey();
        return 0;
    }
}
