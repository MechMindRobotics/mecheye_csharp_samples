/*
With this sample, you can retrieve point clouds in the custom reference frame.
*/

using System;
using MMind.Eye;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;


class TransformPointCloud
{

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


    private static void SetParameters(UserSet userSet)
    {
        // Set the "Data Acquisition Trigger Source" parameter to "Software"
        Utils.ShowError(userSet.SetEnumValue(
            MMind.Eye.TriggerSettings.DataAcquisitionTriggerSource.Name,
            (int)MMind.Eye.TriggerSettings.DataAcquisitionTriggerSource.Value.Software));

        // Set the "Line Scan Trigger Source" parameter to "Fixed rate"
        Utils.ShowError(userSet.SetEnumValue(
             MMind.Eye.TriggerSettings.LineScanTriggerSource.Name,
             (int)(MMind.Eye.TriggerSettings.LineScanTriggerSource.Value.FixedRate)));

        // Set the "Scan Line Count" parameter (the number of lines to be scanned) to 1600
        Utils.ShowError(
            userSet.SetIntValue(MMind.Eye.ScanSettings.ScanLineCount.Name, 1600));
    }

    /// Convert the profile data to an untextured point cloud in the custom reference frame and save it
    /// to a PLY file.
    private static void ConvertBatchToPointCloudWithTransformation(ProfileBatch batch, UserSet userSet, FrameTransformation coordTransformation)
    {
        if (batch.IsEmpty())
            return;

        // Get the X-axis resolution
        double xResolution = 0;
        var status = userSet.GetFloatValue(MMind.Eye.PointCloudResolutions.XAxisResolution.Name, ref xResolution);
        if (!status.IsOK())
        {
            Utils.ShowError(status);
            return;
        }

        double yResolution = 0;
        status = userSet.GetFloatValue(MMind.Eye.PointCloudResolutions.YResolution.Name, ref yResolution);
        if (!status.IsOK())
        {
            Utils.ShowError(status);
            return;
        }
        // Uncomment the following lines for custom Y Unit
        // // Prompt to enter the desired encoder resolution, which is the travel distance corresponding to
        // // one quadrature signal.
        // Console.WriteLine("Please enter the desired encoder resolution (integer, unit: μm, min: 1, max: 65535): ");
        // while (true)
        // {
        //     string str = Console.ReadLine();
        //     if (double.TryParse(str, out yResolution) && yResolution >= 1 && yResolution <= 65535)
        //         break;
        //     Console.WriteLine("Input invalid! Please enter the desired encoder resolution (integer, unit: μm, min: 1, max: 65535): ");
        // }

        int lineScanTriggerSource = 0;
        status = userSet.GetEnumValue(MMind.Eye.TriggerSettings.LineScanTriggerSource.Name, ref lineScanTriggerSource);
        if (!status.IsOK())
        {
            Utils.ShowError(status);
            return;
        }
        bool useEncoderValues = lineScanTriggerSource == (int)MMind.Eye.TriggerSettings.LineScanTriggerSource.Value.Encoder;

        int triggerInterval = 0;
        status = userSet.GetIntValue(MMind.Eye.TriggerSettings.EncoderTriggerInterval.Name, ref triggerInterval);
        if (!status.IsOK())
        {
            Utils.ShowError(status);
            return;
        }

        Console.WriteLine("Save the transformed point cloud.");
        var transformedPointCloud = PointCloudTransformationForProfiler.TransformPointCloud(coordTransformation, batch.GetUntexturedPointCloud(xResolution, yResolution, useEncoderValues, triggerInterval));
        Utils.ShowError(ProfileBatch.SaveUntexturedPointCloud(ref transformedPointCloud, FileFormat.PLY, "PointCloud.ply"));
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

        // Acquire profile data without using callback
        if (!AcquireProfileData(profiler, profileBatch, captureLineCount, dataWidth, isSoftwareTrigger))
            return -1;

        if (profileBatch.CheckFlag(ProfileBatch.BatchFlag.Incomplete))
            Console.WriteLine("Part of the batch's data is lost, the number of valid profiles is: {0}", profileBatch.ValidHeight());
        // Obtain the rigid body transformation from the camera reference frame to the custom reference
        // frame
        // The custom reference frame can be adjusted using the "Custom Reference Frame" tool in
        // Mech - Eye Viewer.The rigid body transformations are automatically calculated after the
        // settings in this tool have been applied
        var coordinateTransformation = PointCloudTransformationForProfiler.GetTransformationParams(profiler);
        if (!coordinateTransformation.IsValid())
        {
            Console.WriteLine("Transformation parameters are not set. Please configure the transformation parameters using the custom coordinate system tool in the client.");
        }
        // Transform the reference frame, generate the untextured point cloud, and save the point cloud
        ConvertBatchToPointCloudWithTransformation(profileBatch, userSet, coordinateTransformation);

        // Disconnect from the laser profiler
        profiler.Disconnect();
        Console.WriteLine("Disconnected from the profiler successfully.");
        Console.WriteLine("Press any key to exit ...");
        Console.ReadKey();
        return 0;
    }
}
