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

    private static void SaveDepthDataToCSV(ProfileDepthMap depth, int[] encoderValues, double xUnit, int yUnit, string fileName)
    {
        Console.WriteLine("Saving point cloud to file: {0}", fileName);
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
                    if (Single.IsNaN(depth.At(y, x)))
                        continue;
                    AddText(fs, String.Format("{0} {1} {2} \n", (int)x * xUnit * kPitch, encoderValues[y] * yUnit * kPitch, depth.At(y, x)));
                }
            }
        }
    }

    private static void Capture(ref Profiler profiler, ref ProfileBatch totalBatch,
             ref List<int> encoderValues, int captureLineCount, int dataPoints)
    {
        Console.WriteLine("Start scanning!");
        if (profiler.StartAcquisition().IsOK() && profiler.TriggerSoftware().IsOK())
        {
            totalBatch.Reserve((ulong)captureLineCount);
            while (totalBatch.Height() < (ulong)captureLineCount)
            {
                var batch = new ProfileBatch((ulong)dataPoints);
                var status = profiler.RetrieveBatchData(ref batch);

                if (status.IsOK())
                {
                    totalBatch.Append(batch);
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

        Console.WriteLine("Please enter capture line count: ");
        int captureLineCnt;
        while (true)
        {
            string str = Console.ReadLine();
            if (int.TryParse(str, out captureLineCnt))
                break;
            Console.WriteLine("Input invalid! Please enter capture line count: ");
        }

        Console.WriteLine("Please enter encoder trigger interval distance (unit : um): ");
        int yUnit;
        while (true)
        {
            string str = Console.ReadLine();
            if (int.TryParse(str, out yUnit))
                break;
            Console.WriteLine( "Input invalid! Please enter encoder trigger interval distance (unit : um) that must be integer : ");
        }

        if (!Utils.ConfirmCapture())
        {
            profiler.Disconnect();
            return 0;
        }

        var currentUserSet = profiler.CurrentUserSet();
        // Set the trigger source to Encoder
        Utils.ShowError(currentUserSet.SetEnumValue(
        MMind.Eye.TriggerSettings.LineScanTriggerSource.Name,
        (int)(MMind.Eye.TriggerSettings.LineScanTriggerSource.Value.Encoder)));

        // Set the maximum number of lines to be scanned to captureLineCnt
        Utils.ShowError(
            currentUserSet.SetIntValue(MMind.Eye.ScanSettings.ScanLineCount.Name, captureLineCnt));

        int dataPoints = 0;
        // Get the line width in the X direction
        Utils.ShowError(currentUserSet.GetIntValue(MMind.Eye.ScanSettings.DataPointsPerProfile.Name, ref dataPoints));
        int captureLineCount = 0;
        // Get the current maximum number of lines to be scanned
        currentUserSet.GetIntValue(MMind.Eye.ScanSettings.ScanLineCount.Name, ref captureLineCount);

        double xUnit = 0;
        Utils.ShowError(currentUserSet.GetFloatValue(MMind.Eye.PointCloudResolutions.XAxisResolution.Name, ref xUnit));

        string fileName = "PointCloud.csv";
        var totalBatch = new ProfileBatch((ulong)dataPoints);
        var encoderVals = new List<int>();
        Capture(ref profiler, ref totalBatch, ref encoderVals, captureLineCount, dataPoints);

        SaveDepthDataToCSV(totalBatch.GetDepthMap(), encoderVals.ToArray(), xUnit, yUnit, fileName);

        // Disconnect from the camera
        profiler.Disconnect();
        Console.WriteLine("Disconnected from the Mech-Eye Profiler successfully.");
        Console.WriteLine("Press any key to exit ...");
        Console.ReadKey();
        return 0;
    }
}
