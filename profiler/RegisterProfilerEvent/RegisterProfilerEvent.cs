/*
With this sample, you can define and register the callback function for monitoring the profiler connection status.
*/

using System;
using System.Threading;
using MMind.Eye;


class RegisterProfilerEvent
{
    // Define the callback function of event
    private static void CallbackFunc(ProfilerEvent.Event profilerEvent, IntPtr pUser)
    {
        Console.WriteLine("A profiler event has occurred. The event name is {0}", profilerEvent);
    }

    static int Main()
    {
        var profiler = new Profiler();
        if (!Utils.FindAndConnect(ref profiler))
            return -1;

        // Set the heartbeat interval to 2 seconds
        profiler.SetHeartbeatInterval(2000);
        Console.WriteLine("Register the callback function for profiler disconnection events.");
        // Register the callback function, and the type of event is PROFILER_EVENT_DISCONNECTED
        Utils.ShowError(ProfilerEvent.RegisterProfilerEventCallback(ref profiler, CallbackFunc, IntPtr.Zero, (uint)ProfilerEvent.Event.PROFILER_EVENT_DISCONNECTED));
        // Let the program sleep for 20 seconds. During this period, if the profiler is disconnected, the
        // callback function will detect and report the disconnection.
        Thread.Sleep(20000);

        profiler.Disconnect();
        Console.WriteLine("Disconnected from the profiler successfully.");
        Console.WriteLine("Press any key to exit ...");
        Console.ReadKey();
        return 0;
    }
}
