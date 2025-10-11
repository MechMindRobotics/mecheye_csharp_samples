/*
With this sample, you can obtain and save 2D images and point clouds
periodically for the specified duration from a camera.
*/

using System;
using System.Threading;
using System.Collections.Generic;
using MMind.Eye;
class WarmUp
{
    static volatile bool isRunning = true;
    static volatile bool isCapturing = false;
    static Thread currentCaptureThread = null;
    static String originalUserSet;
    static String warmupUserSet;
    private const string calibUserSet = "calib";
    private const int defaultWarmupMinutesOfUHPOrLSRS = 60;
    private const int defaultWarmupMinutes = 30;
    private const int defaultIntervalSeconds = 5;
    private const int defaultIntervalSecondsOfUHP = 3;
    private const int minWarmupMinutes = 15;
    private const int maxWarmupMinutes = 90;
    private const int minIntervalSeconds = 5;
    private const int maxIntervalSeconds = 30;
    private static bool userSetSampleInterval = false;
    private static int warmupTimeMinutes = defaultWarmupMinutes;
    private static int sampleIntervalSeconds = defaultIntervalSeconds;

    private static bool userSetWarmupTime = false;

    static int Main(string[] args)
    {
        if (HasHelpFlag(args))
        {
            PrintHelp();
            return 0;
        }

        if (!ParseArgs(args))
            return -1;

        Console.CancelKeyPress += (sender, e) =>
        {
            Console.WriteLine("\n[Interrupt] Ctrl+C pressed. Cleaning up...");
            e.Cancel = true;
            isRunning = false;
        };

        var camera = new Camera();
        if (!Utils.FindAndConnect(ref camera))
            return -1;

        CameraInfo cameraInfo = new CameraInfo();
        Utils.ShowError(camera.GetCameraInfo(ref cameraInfo));
        Utils.PrintCameraInfo(cameraInfo);

        if (!userSetWarmupTime)
        {
            bool isUhpOrLsrs = cameraInfo.Model == "Mech-Eye LSR S" || cameraInfo.Model.Contains("Mech-Eye UHP");
            warmupTimeMinutes = isUhpOrLsrs ? defaultWarmupMinutesOfUHPOrLSRS : defaultWarmupMinutes;
            Console.WriteLine($"[Info] Detected {cameraInfo.Model}, warmup time set to {warmupTimeMinutes} minutes.");
        }


        if (!userSetSampleInterval)
        {
            bool isUhp = cameraInfo.Model.Contains("Mech-Eye UHP");
            sampleIntervalSeconds = isUhp ? defaultIntervalSecondsOfUHP : defaultIntervalSeconds;
            Console.WriteLine($"[Info] Detected {cameraInfo.Model}, warmup interval set to {sampleIntervalSeconds} minutes.");
        }

        SwitchUserSet(camera);
        Console.WriteLine("Starting warmup for {0} minutes with {1} seconds interval.", warmupTimeMinutes, sampleIntervalSeconds);
        TimeSpan captureTime = TimeSpan.FromMinutes(warmupTimeMinutes);
        TimeSpan capturePeriod = TimeSpan.FromSeconds(sampleIntervalSeconds);
        RunWarmup(camera, captureTime, capturePeriod);

        Cleanup(camera);
        return 0;
    }

    static void RunWarmup(Camera camera, TimeSpan totalDuration, TimeSpan interval)
    {
        DateTime start = DateTime.Now;
        int captureCount = 0;
        int intervalSeconds = (int)interval.TotalSeconds;

        while (isRunning && DateTime.Now - start < totalDuration)
        {
            if (!isCapturing)
            {
                captureCount++;

                isCapturing = true;
                int localCaptureCount = captureCount;

                currentCaptureThread = new Thread(() =>
                {
                    try
                    {
                        Capture(camera, localCaptureCount, intervalSeconds, totalDuration);
                    }
                    finally
                    {
                        isCapturing = false;
                    }
                });
                currentCaptureThread.Start();
            }
            else
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [Warning] Previous capture still running. Skipping this interval.");
            }

            TimeSpan slept = TimeSpan.Zero;
            TimeSpan step = TimeSpan.FromMilliseconds(100);
            while (isRunning && slept < interval)
            {
                Thread.Sleep(step);
                slept += step;
            }
        }

        if (currentCaptureThread != null && currentCaptureThread.IsAlive)
            currentCaptureThread.Join();

        Console.WriteLine("Warmup completed.");
    }

    static void Capture(Camera camera, int currentCount, int intervalSeconds, TimeSpan totalDuration)
    {
        var frame = new Frame2DAnd3D();
        var status = camera.Capture2DAnd3D(ref frame);
        Utils.ShowError(status);
        if (!status.IsOK())
            return;

        CameraStatus cameraStatus = new CameraStatus();
        status = camera.GetCameraStatus(ref cameraStatus);
        Utils.ShowError(status);

        double progress = (double)(currentCount * intervalSeconds) / totalDuration.TotalSeconds * 100;
        if (progress > 100) progress = 100;

        Console.WriteLine("------------------------------");
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Warmup capture #{currentCount} (Progress: {progress:F1}%) Completed");
        Utils.PrintCameraStatus(cameraStatus);
        Console.WriteLine("------------------------------");

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

    static void SwitchUserSet(Camera camera)
    {
        originalUserSet = camera.CurrentUserSet().GetName();
        Console.WriteLine("Current user set: " + originalUserSet);
        if(!camera.UserSetManager().SelectUserSet(calibUserSet).IsOK()){
            Console.WriteLine("Failed to switch to user set: calib");
            return ;
        }

        List<string> nameList = new List<string>();
        if (!camera.UserSetManager().GetAllUserSetNames(ref nameList).IsOK())
        {
            Console.WriteLine("Failed to get all  user set");
            return;
        }

        warmupUserSet = CreateWarmupUserSet(nameList);
        if (!camera.UserSetManager().AddUserSet(warmupUserSet).IsOK())
        {
            Console.WriteLine("Failed to add to user set: "+ warmupUserSet);
            return;
        }

        if (!camera.UserSetManager().SelectUserSet(warmupUserSet).IsOK())
        {
            Console.WriteLine("Failed to switch to user set: " + warmupUserSet);
            return;
        }
        Console.WriteLine("Switched to user set:" +  warmupUserSet);
    }

    static void Cleanup(Camera camera)
    {
        if (currentCaptureThread != null && currentCaptureThread.IsAlive)
            currentCaptureThread.Join();

        if (!camera.UserSetManager().SelectUserSet(originalUserSet).IsOK())
        {
            Console.WriteLine("Failed to switch back to user set: " + originalUserSet);
        }

        if (!camera.UserSetManager().DeleteUserSet(warmupUserSet).IsOK())
        {
            Console.WriteLine("Failed to deleta user set: " + warmupUserSet);
        }

        camera.Disconnect();
        Console.WriteLine("Camera disconnected.");
    }

    static bool HasHelpFlag(string[] args)
    {
        foreach (string arg in args)
        {
            if (arg == "-h" || arg == "--help")
                return true;
        }
        return false;
    }

    static void PrintHelp()
    {
        Console.WriteLine("Usage: warmup.exe [--warmup-time <minutes>] [--sample-interval <seconds>]");
        Console.WriteLine("Options:");
        Console.WriteLine($" -w, --warmup-time       Warmup duration in minutes ({minWarmupMinutes}-{maxWarmupMinutes}). Default is {defaultWarmupMinutes}.");
        Console.WriteLine($"                         (For Mech-Eye LSRS/UHP camera, default is {defaultWarmupMinutesOfUHPOrLSRS} if not set)");
        Console.WriteLine($" -i, --sample-interval   Interval between captures in seconds ({minIntervalSeconds}-{maxIntervalSeconds}). Default is {defaultIntervalSeconds}.");
        Console.WriteLine($"                         (For Mech-Eye UHP camera, default is {defaultIntervalSecondsOfUHP} if not set)");
        Console.WriteLine(" -h, --help          Show this help message.");
    }

    static bool ParseArgs(string[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            if ((args[i] == "-w" || args[i] == "--warmup-time") && i + 1 < args.Length &&
                int.TryParse(args[i + 1], out int t))
            {
                if (t < minWarmupMinutes || t > maxWarmupMinutes)
                {
                    Console.WriteLine($"Error: Warmup time must be between {minWarmupMinutes} and {maxWarmupMinutes} minutes.");
                    return false;
                }
                warmupTimeMinutes = t;
                userSetWarmupTime = true;
                i++;
            }
            else if ((args[i] == "-i" || args[i] == "--sample-interval") && i + 1 < args.Length &&
                     int.TryParse(args[i + 1], out int iSec))
            {
                if (iSec < minIntervalSeconds || iSec > maxIntervalSeconds)
                {
                    Console.WriteLine($"Error: Sample interval must be between {minIntervalSeconds} and {maxIntervalSeconds} seconds.");
                    return false;
                }
                sampleIntervalSeconds = iSec;
                userSetSampleInterval = true;
                i++;
            }
        }

        return true;
    }
}
