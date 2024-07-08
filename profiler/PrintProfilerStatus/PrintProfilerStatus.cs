/*
With this sample, you can obtain and print the laser profiler's information, such as model, serial number, firmware version, and temperatures.
*/

using System;
using MMind.Eye;

class PrintProfilerStatus
{
    static int Main()
    {
        var profiler = new Profiler();
        if (!Utils.FindAndConnect(ref profiler))
            return -1;

        var profilerInfo = new ProfilerInfo();
        Utils.ShowError(profiler.GetProfilerInfo(ref profilerInfo));
        Utils.PrintProfilerInfo(profilerInfo);

        var profilerStatus = new ProfilerStatus();
        Utils.ShowError(profiler.GetProfilerStatus(ref profilerStatus));
        Utils.PrintProfilerStatus(profilerStatus);

        profiler.Disconnect();
        Console.WriteLine("Disconnected from the profiler successfully.");
        Console.WriteLine("Press any key to exit ...");
        Console.ReadKey();
        return 0;
    }
}

