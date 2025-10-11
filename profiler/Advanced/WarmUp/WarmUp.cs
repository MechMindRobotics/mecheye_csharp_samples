/*
With this sample, you can periodically trigger data acquisition with signals input from an external
device at a fixed scan rate to warm up the Mech-Eye Profiler, while the acquired data is used only
for stabilization and not saved.
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

class ProfilerWarmup
{
    private static Mutex mut = new Mutex();
    private static ProfileBatch currentBatch;
    private static Profiler profiler;
    private static string originalUserSet = "";
    private static string warmupUserSet = "";
    private static int sampleCount = 0;
    private static bool stopWarmup = false;
    private static object captureLock = new object();
    private static bool isCapturing = false;


    private const string calibUserSet = "calib";
    private const int defaultWarmupMinutes = 30;
    private const int defaultIntervalSeconds = 5;
    private const int minWarmupMinutes = 30;
    private const int maxWarmupMinutes = 90;
    private const int minIntervalSeconds = 3;
    private const int maxIntervalSeconds = 30;
    private static int warmupTimeMinutes = defaultWarmupMinutes;
    private static int sampleIntervalSeconds = defaultIntervalSeconds;

    private const int minLineCount = 1, maxLineCount = 20000;
    private static double minSoftwareTriggerRate = 2.0f, maxSoftwareTriggerRate = 2500.0f;

    static void Main(string[] args)
    {
        ParseArgs(args);
        Console.CancelKeyPress += OnConsoleCancelKeyPress;

        profiler = new Profiler();

        if (!Utils.FindAndConnect(ref profiler))
        {
            Console.WriteLine("[Error] Failed to connect to profiler.");
            return;
        }

        if (!Utils.ConfirmCapture())
        {
            profiler.Disconnect();
            return;
        }

        if (!SwitchToWarmupUserSet())
        {
            Console.WriteLine("[Error] Failed to switch user set.");
            Cleanup();
            return;
        }

        if (!SetWarmupParameters(profiler.CurrentUserSet()))
        {
            Cleanup();
            return;
        }

        if (!InteractiveSetScanParams())
        {
            Cleanup();
            return;
        }

        int dataWidth = 0;
        Utils.ShowError(profiler.CurrentUserSet().GetIntValue(MMind.Eye.ScanSettings.DataPointsPerProfile.Name, ref dataWidth));
        currentBatch = new ProfileBatch((ulong)dataWidth);

        var status = profiler.RegisterAcquisitionCallback(AcquisitionCallback, GCHandle.ToIntPtr(GCHandle.Alloc(currentBatch)));
        if (!status.IsOK())
        {
            Utils.ShowError(status);
            Cleanup();
            return;
        }
        WarmupLoop();

        Cleanup();
        Console.WriteLine("Profiler warmup finished.");
    }

    private static void ParseArgs(string[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            if ((args[i] == "-w" || args[i] == "--warmup-time") && i + 1 < args.Length)
            {
                if (int.TryParse(args[i + 1], out int w))
                {
                    if (w >= minWarmupMinutes && w <= maxWarmupMinutes) warmupTimeMinutes = w;
                    else
                    {
                        Console.WriteLine("[Error] Warmup time must be between " + minWarmupMinutes + " and " + maxWarmupMinutes + " minutes.");
                        Environment.Exit(1);
                    }
                }
                else
                {
                    Console.WriteLine("[Error] Invalid warmup time argument.");
                    Environment.Exit(1);
                }
                i++;
            }
            else if ((args[i] == "-i" || args[i] == "--sample-interval") && i + 1 < args.Length)
            {
                if (int.TryParse(args[i + 1], out int si))
                {
                    if (si >= minIntervalSeconds && si <= maxIntervalSeconds) sampleIntervalSeconds = si;
                    else
                    {
                        Console.WriteLine("[Error] Sample interval must be between " + minIntervalSeconds + " and " + maxIntervalSeconds + " seconds.");
                        Environment.Exit(1);
                    }
                }
                else
                {
                    Console.WriteLine("[Error] Invalid sample interval argument.");
                    Environment.Exit(1);
                }
                i++;
            }
            else if (args[i] == "-h" || args[i] == "--help")
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("  -w, --warmup-time <minutes>    Set warmup duration (" + minWarmupMinutes + "-" + maxWarmupMinutes + " minutes, default " + defaultWarmupMinutes + ")");
                Console.WriteLine("  -i, --sample-interval <sec>    Set sample interval (" + minIntervalSeconds + "-" + maxIntervalSeconds + " seconds, default " + defaultIntervalSeconds + ")");
                Environment.Exit(0);
            }
            else
            {
                Console.WriteLine($"Unknown argument: {args[i]}");
                Environment.Exit(1);
            }
        }
    }

    static string CreateWarmupUserSet(List<string> groupNames, string baseName = "warmup")
    {
        if (!groupNames.Contains(baseName))
        {
            return baseName;
        }

        var random = new Random();
        string newName;

        do
        {
            int randomNum = random.Next(0, 10000);
            newName = $"{baseName}_{randomNum:D4}";
        }
        while (groupNames.Contains(newName));

        return newName;
    }

    private static bool SwitchToWarmupUserSet()
    {
        originalUserSet = profiler.CurrentUserSet().GetName();
        Console.WriteLine("[Info] Original user set: " + originalUserSet);

        if (!profiler.UserSetManager().SelectUserSet(calibUserSet).IsOK())
        {
            Console.WriteLine("Failed to switch to user set: calib");
            return false;
        }

        List<string> nameList = new List<string>();
        if (!profiler.UserSetManager().GetAllUserSetNames(ref nameList).IsOK())
        {
            Console.WriteLine("Failed to get all  user set");
            return false;
        }

        warmupUserSet = CreateWarmupUserSet(nameList);
        if (!profiler.UserSetManager().AddUserSet(warmupUserSet).IsOK())
        {
            Console.WriteLine("Failed to add to user set: " + warmupUserSet);
            return false;
        }

        if (!profiler.UserSetManager().SelectUserSet(warmupUserSet).IsOK())
        {
            Console.WriteLine("Failed to switch to user set: " + warmupUserSet);
            return false;
        }


        Console.WriteLine("[Info] Switched to user set: " + warmupUserSet);
        return true;
    }

    private static bool SetWarmupParameters(UserSet userSet)
    {
        var status = userSet.SetEnumValue(
            MMind.Eye.TriggerSettings.LineScanTriggerSource.Name,
            (int)MMind.Eye.TriggerSettings.LineScanTriggerSource.Value.FixedRate);
        if (!status.IsOK()) { Utils.ShowError(status); return false; }

        status = userSet.SetEnumValue(
            MMind.Eye.TriggerSettings.DataAcquisitionTriggerSource.Name,
            (int)MMind.Eye.TriggerSettings.DataAcquisitionTriggerSource.Value.Software);
        if (!status.IsOK()) { Utils.ShowError(status); return false; }

        status = userSet.SetEnumValue(
            MMind.Eye.TriggerSettings.DataAcquisitionMethod.Name,
            (int)MMind.Eye.TriggerSettings.DataAcquisitionMethod.Value.Frame_Based);
        if (!status.IsOK()) { Utils.ShowError(status); return false; }

        Console.WriteLine("[Info] Warmup parameters set.");
        return true;
    }
    private static bool InteractiveSetScanParams()
    {
        int currentLineCount = 0;
        double currentSoftwareTriggerRate = 0.0;
        var userSet = profiler.CurrentUserSet();
        var status = userSet.GetIntValue(MMind.Eye.ScanSettings.ScanLineCount.Name, ref currentLineCount);
        if (!status.IsOK()) { Utils.ShowError(status); return false; }

        status = userSet.GetFloatValue(MMind.Eye.TriggerSettings.SoftwareTriggerRate.Name, ref currentSoftwareTriggerRate);
        if (!status.IsOK()) { Utils.ShowError(status); return false; }

        status = userSet.GetFloatValue(MMind.Eye.TriggerSettings.MaxScanRate.Name, ref maxSoftwareTriggerRate);
        if (!status.IsOK()) { Utils.ShowError(status); return false; }


        Console.WriteLine($"[Info] Please input scan line count (current: {currentLineCount}, range: {minLineCount}-{maxLineCount}): ");
        string lineCountInput = Console.ReadLine();
        int scanLineCount;
        if (string.IsNullOrWhiteSpace(lineCountInput))
        {
            scanLineCount = currentLineCount;
            Console.WriteLine($"[Info] Using current scan line count: {scanLineCount}");
        }
        else if (!int.TryParse(lineCountInput, out scanLineCount) || scanLineCount < minLineCount || scanLineCount > maxLineCount)
        {
            Console.WriteLine($"[Error] Invalid scan line count. Must be integer in range {minLineCount}–{maxLineCount}.");
            return false;
        }
        Console.WriteLine($"[Info] Please input software trigger rate (Hz, current: {currentSoftwareTriggerRate}, range: {minSoftwareTriggerRate}-{maxSoftwareTriggerRate}): ");
        string freqInput = Console.ReadLine();
        double softwareTriggerRate;
        if (string.IsNullOrWhiteSpace(freqInput))
        {
            softwareTriggerRate = currentSoftwareTriggerRate;
            Console.WriteLine($"[Info] Using current software trigger: {softwareTriggerRate}");
        }
        else if (!double.TryParse(freqInput, out softwareTriggerRate) || softwareTriggerRate < minSoftwareTriggerRate || softwareTriggerRate > maxSoftwareTriggerRate)
        {
            Console.WriteLine($"[Error] Invalid software trigger rate. Must be float in range {minSoftwareTriggerRate}–{maxSoftwareTriggerRate}.");
            return false;
        }
        var setStatus1 = userSet.SetIntValue(MMind.Eye.ScanSettings.ScanLineCount.Name, scanLineCount);
        if (!setStatus1.IsOK()) { Utils.ShowError(setStatus1); return false; }

        var setStatus2 = userSet.SetFloatValue(MMind.Eye.TriggerSettings.SoftwareTriggerRate.Name, softwareTriggerRate);
        if (!setStatus2.IsOK()) { Utils.ShowError(setStatus2); return false; }

        Console.WriteLine($"[Info] Scan params set: line count = {scanLineCount}, software trigger rate = {softwareTriggerRate} Hz");
        return true;
    }


    private static void AcquisitionCallback(ref ProfileBatch batch, IntPtr pUser)
    {
        mut.WaitOne();
        sampleCount++;
        if (!batch.GetErrorStatus().IsOK())
        {
            Console.WriteLine("[Error] Data acquisition error.");
            Utils.ShowError(batch.GetErrorStatus());
        }
        else
        {
            var handle = GCHandle.FromIntPtr(pUser);
            var outputBatch = (ProfileBatch)handle.Target;
            outputBatch.Append(batch);
            ProfilerStatus profilerStatus = new ProfilerStatus();
            var status = profiler.GetProfilerStatus(ref profilerStatus);
            if (status.IsOK())
            {
                double progress = Math.Min(100.0, (double)(sampleCount * sampleIntervalSeconds) / (warmupTimeMinutes * 60) * 100);
                string nowStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                Console.WriteLine("--------");
                Console.WriteLine($"[Info] [{nowStr}] Warmup sample #{sampleCount} completed. Progress: {progress:F1} %");
                Utils.PrintProfilerStatus(profilerStatus);
                Console.WriteLine("--------");
            }
        }
        mut.ReleaseMutex();
    }

    private static void WarmupLoop()
    {
        Console.WriteLine($"Warmup time: {warmupTimeMinutes} minutes, sample interval: {sampleIntervalSeconds} seconds");
        Console.WriteLine("[Info] Warmup loop started...");

        DateTime deadline = DateTime.Now.AddMinutes(warmupTimeMinutes);

        var status = profiler.StartAcquisition();
        if (!status.IsOK())
        {
            Utils.ShowError(status);
            return;
        }

        while (!stopWarmup && DateTime.Now < deadline)
        {
            lock (captureLock)
            {
                if (isCapturing)
                {
                    Console.WriteLine("[Warning] Last capture not finished, skip this cycle.");
                    Thread.Sleep(500);
                    continue;
                }
                isCapturing = true;
            }

            Console.WriteLine($"[Info] Trigger capture at: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

            currentBatch.Clear();

            var triggerStatus = profiler.TriggerSoftware();
            if (!triggerStatus.IsOK())
            {
                Utils.ShowError(triggerStatus);
                lock (captureLock) { isCapturing = false; }
                break;
            }

            int waited = 0;
            int waitStep = 200;
            int waitTime = sampleIntervalSeconds * 1000;
            while (!stopWarmup && waited < waitTime)
            {
                Thread.Sleep(waitStep);
                waited += waitStep;
            }

            lock (captureLock) { isCapturing = false; }
        }

        profiler.StopAcquisition();
        Console.WriteLine("[Info] Warmup loop exited.");
    }

    private static void Cleanup()
    {
        if (profiler != null)
        {
            try
            {
                if (!string.IsNullOrEmpty(originalUserSet))
                {
                    if (!profiler.UserSetManager().SelectUserSet(originalUserSet).IsOK())
                    {
                        Console.WriteLine("Failed to switch back to user set: " + originalUserSet);
                    }
                    else
                    {
                        Console.WriteLine("Switched back to user set: " + originalUserSet);
                    }

                    if (!profiler.UserSetManager().DeleteUserSet(warmupUserSet).IsOK())
                    {
                        Console.WriteLine("Failed to deleta user set: " + warmupUserSet);
                    }
                    else
                    {
                        Console.WriteLine("delete user set: " + warmupUserSet);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Error] Failed to restore user set: " + ex.Message);
            }
            profiler.Disconnect();
            Console.WriteLine("[Info] Profiler disconnected.");
        }
    }

    private static void OnConsoleCancelKeyPress(object sender, ConsoleCancelEventArgs e)
    {
        Console.WriteLine("\n[Interrupt] Ctrl+C received. Preparing to stop warmup...");
        stopWarmup = true;
        e.Cancel = true;
    }
}
