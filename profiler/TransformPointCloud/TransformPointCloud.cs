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
    private static readonly Mutex mut = new Mutex();
    private static readonly double kPitch = 1e-3;

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

    /// Calculate the initial coordinates of each point, apply the rigid body transformations to the
    /// initial coordinates, and then write the transformed coordinates to the PLY file.
    private static void TransformAndSaveDepthDataToPly(ProfileDepthMap depth, int[] yValues, double xUnit, double yUnit, string fileName, bool isOrganized,FrameTransformation coordTransformaition)
    {
        if (File.Exists(fileName))
            File.Delete(fileName);
        uint validPointCount = 0;
        var w = depth.Width();
        var h = depth.Height();
        if (!isOrganized)
        {
            for (ulong y = 0; y < h; ++y)
            {
                for (ulong x = 0; x < w; ++x)
                {
                    if (!Single.IsNaN(depth.At(y, x)))
                        validPointCount++;
                }
            }
        }

        using (FileStream fs = File.Create(fileName))
        {
            Utils.AddText(fs, "ply\n");
            Utils.AddText(fs, "format ascii 1.0\n");
            Utils.AddText(fs, "comment File generated\n");
            Utils.AddText(fs, "comment x y z data unit in mm\n");
            Utils.AddText(fs, String.Format("element vertex {0}\n", isOrganized ? (uint)w * h : validPointCount));
            Utils.AddText(fs, "property float x\n");
            Utils.AddText(fs, "property float y\n");
            Utils.AddText(fs, "property float z\n");
            Utils.AddText(fs, "end_header\n");


            var rotation = coordTransformaition.Rotation; 
            var translation = coordTransformaition.Translation;
            for (ulong y = 0; y < h; ++y)
            {
                for (ulong x = 0; x < w; ++x)
                {
                    // Calculate the initial coordinates of each point from the original profile data.
                    double xPos = x * xUnit * kPitch;
                    double yPos = yValues[y] * yUnit * kPitch;
                    double zPos = depth.At(y, x);
                    // Apply the rigid body transformations to the initial coordinates to obtain the
                    // coordinates in the custom reference frame.
                    double transformedX = xPos * rotation.R1 +
                                              yPos * rotation.R2 +
                                              zPos * rotation.R3 +
                                              translation.X;
                     double transformedY = xPos * rotation.R4 +
                                              yPos * rotation.R5 +
                                              zPos * rotation.R6 +
                                              translation.Y;
                     double transformedZ = xPos * rotation.R7 +
                                              yPos * rotation.R8 +
                                              zPos * rotation.R9 +
                                              translation.Z;
                    if (Single.IsNaN(depth.At(y, x)))
                        Utils.AddText(fs, "nan nan nan\n");
                    else
                        Utils.AddText(fs, String.Format("{0} {1} {2} \n", transformedX, transformedY, transformedZ));
                }
            }
        }
    }

    /// Convert the profile data to an untextured point cloud in the custom reference frame and save it
    /// to a PLY file.
    private static void ConvertBatchToPointCloudWithTransformation(ProfileBatch batch, UserSet userSet,FrameTransformation coordTransformaition)
    {
        if (batch.IsEmpty())
            return;

        // Get the X-axis resolution
        double xUnit = 0;
        var status = userSet.GetFloatValue(MMind.Eye.PointCloudResolutions.XAxisResolution.Name, ref xUnit);
        if (!status.IsOK())
        {
            Utils.ShowError(status);
            return;
        }

        double yUnit = 0;
        status = userSet.GetFloatValue(MMind.Eye.PointCloudResolutions.YResolution.Name, ref yUnit);
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
        //     if (double.TryParse(str, out yUnit) && yUnit >= 1 && yUnit <= 65535)
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

        // Shift the encoder values around zero
        var encoderVals = new List<int>();
        var encoder = batch.GetEncoderArray();
        for (ulong r = 0; r < batch.Height(); ++r)
            encoderVals.Add(useEncoderValues ? Utils.ShiftEncoderValsAroundZero(encoder[r], (int)encoder[0]) / triggerInterval : (int)r);

        Console.WriteLine("Save the transformed point cloud.");

        TransformAndSaveDepthDataToPly(batch.GetDepthMap(), encoderVals.ToArray(), xUnit, yUnit, "PointCloud.ply", true,coordTransformaition);
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
