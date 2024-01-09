/*
With this sample, you can acquire the profile data, generate the point cloud, and save the point cloud in the CSV and PLY formats
*/

using System;
using MMind.Eye;
using System.Threading;
using System.IO;
using System.Text;
using System.Collections.Generic;

class AcquirePointCloud
{
    private static readonly double kPitch = 1e-3;

    private static int ShiftEncoderValsAroundZero(uint oriVal, int initValue = 0x0FFFFFFF)
    {
        return (int)(oriVal - initValue);
    }

    private static void AddText(FileStream fs, string value)
    {
        byte[] info = new UTF8Encoding(true).GetBytes(value);
        fs.Write(info, 0, info.Length);
    }


    private static void SaveDepthDataToCSV(ProfileDepthMap depth, int[] encoderValues, double xUnit, int yUnit, string fileName, bool isOrganized)
    {
        Console.WriteLine("Saving the point cloud to file: {0}", fileName);
        if (File.Exists(fileName))
            File.Delete(fileName);
        var w = depth.Width();
        var h = depth.Height();
        using (FileStream fs = File.Create(fileName))
        {
            for (ulong y = 0; y < h; ++y)
            {
                for (ulong x = 0; x < w; ++x)
                {
                    if (!Single.IsNaN(depth.At(y, x)))
                        AddText(fs, String.Format("{0} {1} {2} \n", (int)x * xUnit * kPitch, encoderValues[y] * yUnit * kPitch, depth.At(y, x)));
                    else if (isOrganized)
                        AddText(fs, "nan nan nan\n");
                }
            }
        }
    }

    private static void SaveDepthDataToPly(ProfileDepthMap depth, int[] encoderValues, double xUnit, int yUnit, string fileName, bool isOrganized)
    {
        Console.WriteLine("Saving the point cloud to file: {0}", fileName);
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
            AddText(fs, "ply\n");
            AddText(fs, "format ascii 1.0\n");
            AddText(fs, "comment File generated\n");
            AddText(fs, "comment x y z data unit in mm\n");
            AddText(fs, String.Format("element vertex {0}\n", isOrganized ? (uint)w * h : validPointCount));
            AddText(fs, "property float x\n");
            AddText(fs, "property float y\n");
            AddText(fs, "property float z\n");
            AddText(fs, "end_header\n");

            for (ulong y = 0; y < h; ++y)
            {
                for (ulong x = 0; x < w; ++x)
                {
                    if (Single.IsNaN(depth.At(y, x)))
                        AddText(fs, "nan nan nan\n");
                    else
                        AddText(fs, String.Format("{0} {1} {2} \n", (int)x * xUnit * kPitch, encoderValues[y] * yUnit * kPitch, depth.At(y, x)));
                }
            }
        }
    }

    private static void Capture(ref Profiler profiler, ref ProfileBatch totalBatch,
             ref List<int> encoderValues, int captureLineCount, int dataPoints)
    {
        Console.WriteLine("Start data acquisition.");
        if (profiler.StartAcquisition().IsOK() && profiler.TriggerSoftware().IsOK())
        {
            totalBatch.Reserve((ulong)captureLineCount);
            while (totalBatch.Height() < (ulong)captureLineCount)
            {
                var batch = new ProfileBatch((ulong)dataPoints);
                var status = profiler.RetrieveBatchData(ref batch);

                if (status.IsOK())
                {
                    if (!totalBatch.Append(batch))
                        break;
                    for (ulong r = 0; r < batch.Height(); ++r)
                    {
                        var encoderArray = totalBatch.GetEncoderArray();
                        encoderValues.Add(ShiftEncoderValsAroundZero(encoderArray[totalBatch.Height() - batch.Height() + r], (int)encoderArray[0]));
                    }
                    Thread.Sleep(200);
                }
                else
                {
                    Utils.ShowError(status);
                    break;
                }
            }
        }
    }

    static int Main()
    {
        var profiler = new Profiler();
        if (!Utils.FindAndConnect(ref profiler))
        {
            Console.ReadKey();
            return -1;
        }

        Console.WriteLine("Please enter the number of lines that you want to scan (min: 16, max: 60000): ");
        int captureLineCnt;
        while (true)
        {
            string str = Console.ReadLine();
            if (int.TryParse(str, out captureLineCnt) && captureLineCnt >= 16 && captureLineCnt <= 60000)
                break;
            Console.WriteLine("Input invalid! Please enter the number of lines that you want to scan (min: 16, max: 60000): ");
        }

        // Prompt to enter the desired encoder resolution, which is the travel distance corresponding to
        // one quadrature signal.
        Console.WriteLine("Please enter the desired encoder resolution (integer, unit: μm, min: 1, max: 65535): ");
        int yUnit;
        while (true)
        {
            string str = Console.ReadLine();
            if (int.TryParse(str, out yUnit) && yUnit >= 1 && yUnit <= 65535)
                break;
            Console.WriteLine("Input invalid! Please enter the desired encoder resolution (integer, unit: μm, min: 1, max: 65535): ");
        }

        if (!Utils.ConfirmCapture())
        {
            profiler.Disconnect();
            return 0;
        }

        var currentUserSet = profiler.CurrentUserSet();

        // Set the "Data Acquisition Trigger Source" parameter to "Software"
        Utils.ShowError(currentUserSet.SetEnumValue(
            MMind.Eye.TriggerSettings.DataAcquisitionTriggerSource.Name,
            (int)MMind.Eye.TriggerSettings.DataAcquisitionTriggerSource.Value.Software));
        //// Set the "Data Acquisition Trigger Source" parameter to "External"
        // Utils.ShowError(currentUserSet.SetEnumValue(
        //     MMind.Eye.TriggerSettings.DataAcquisitionTriggerSource.Name,
        //     MMind::Eye.TriggerSettings.DataAcquisitionTriggerSource.Value.External));


        // Set the "Line Scan Trigger Source" parameter to "Encoder"
        Utils.ShowError(currentUserSet.SetEnumValue(
        MMind.Eye.TriggerSettings.LineScanTriggerSource.Name,
        (int)(MMind.Eye.TriggerSettings.LineScanTriggerSource.Value.Encoder)));

        // Set the "Scan Line Count" parameter (the number of lines to be scanned) to captureLineCnt
        Utils.ShowError(
            currentUserSet.SetIntValue(MMind.Eye.ScanSettings.ScanLineCount.Name, captureLineCnt));

        int dataPoints = 0;
        // Get the number of data points in each profile
        Utils.ShowError(currentUserSet.GetIntValue(MMind.Eye.ScanSettings.DataPointsPerProfile.Name, ref dataPoints));
        int captureLineCount = 0;
        // Get the current value of the "Scan Line Count" parameter
        currentUserSet.GetIntValue(MMind.Eye.ScanSettings.ScanLineCount.Name, ref captureLineCount);

        // Get the X-axis resolution
        double xUnit = 0;
        Utils.ShowError(currentUserSet.GetFloatValue(MMind.Eye.PointCloudResolutions.XAxisResolution.Name, ref xUnit));


        var totalBatch = new ProfileBatch((ulong)dataPoints);
        var encoderVals = new List<int>();
        Capture(ref profiler, ref totalBatch, ref encoderVals, captureLineCount, dataPoints);
        if (!totalBatch.IsEmpty())
        {
            SaveDepthDataToCSV(totalBatch.GetDepthMap(), encoderVals.ToArray(), xUnit, yUnit, "PointCloud.csv", true);
            SaveDepthDataToPly(totalBatch.GetDepthMap(), encoderVals.ToArray(), xUnit, yUnit, "PointCloud.ply", true);
        }

        // Disconnect from the camera
        profiler.Disconnect();
        Console.WriteLine("Disconnected from the Profiler successfully.");
        Console.WriteLine("Press any key to exit ...");
        Console.ReadKey();
        return 0;
    }
}
