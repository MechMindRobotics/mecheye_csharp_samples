/*
With this sample, you can define and register the callback function for monitoring the profiler connection status.
*/

using System;
using System.Threading;
using System.Runtime.InteropServices;
using MMind.Eye;

class RegisterProfilerEvent
{
    // Define the callback function of event
    private static void CallbackFunc(ProfilerEvent.Event profilerEvent, IntPtr pUser)
    {
        Console.WriteLine("A profiler event has occurred. The event name is {0}", profilerEvent);
    }


    // Define the callback function for handling the events
    private static void ProfilerCallbackFuncWithPUser(ref ProfilerEvent.EventData eventData, ref ProfilerEvent.PayloadMember[] extraPayload, IntPtr pUser)
    {
        Console.WriteLine("A profiler event has occurred.");
        Console.WriteLine("\tThe event ID is {0}", eventData.eventId);
        Console.WriteLine("\tThe event name is {0}", eventData.eventName);
        DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        DateTime timestamp = epoch.AddMilliseconds(eventData.timestamp);
        Console.WriteLine("\tThe event timestamp is {0}", timestamp.ToString("yyyy-MM-dd HH:mm:ss"));
        foreach (var member in extraPayload)
        {
            switch (member.Type)
            {
                case ProfilerEvent.PayloadMemberType._UInt32:
                    Console.WriteLine("\tThe event {0} is {1}", member.Name, member.Value.UInt32Value);
                    break;
                case ProfilerEvent.PayloadMemberType._Int32:
                    Console.WriteLine("\tThe event {0} is {1}", member.Name, member.Value.Int32Value);
                    break;
                case ProfilerEvent.PayloadMemberType._Int64:
                    Console.WriteLine("\tThe event {0} is {1}", member.Name, member.Value.Int64Value);
                    break;
                case ProfilerEvent.PayloadMemberType._Float:
                    Console.WriteLine("\tThe event {0} is {1}", member.Name, member.Value.FloatValue);
                    break;
                case ProfilerEvent.PayloadMemberType._Double:
                    Console.WriteLine("\tThe event {0} is {1}", member.Name, member.Value.DoubleValue);
                    break;
                case ProfilerEvent.PayloadMemberType._Bool:
                    Console.WriteLine("\tThe event {0} is {1}", member.Name, member.Value.BoolValue);
                    break;
                case ProfilerEvent.PayloadMemberType._String:
                    Console.WriteLine("\tThe event {0} is {1}", member.Name, member.Value.StringValue);
                    break;
            }
        }
    }
    private static bool AcquireProfileData(Profiler profiler, ProfileBatch totalBatch, int captureLineCount, int dataWidth, bool isSoftwareTrigger)
    {
        /* Call startAcquisition() to set the laser profiler to the acquisition-ready status, and
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

    static int Main()
    {
        var profiler = new Profiler();
        if (!Utils.FindAndConnect(ref profiler))
        {
            Console.WriteLine("Press any key to exit ...");
            Console.ReadKey();
            return -1;
        }

        // Set the heartbeat interval to 2 seconds
        profiler.SetHeartbeatInterval(2000);

        var callbackFunc = new ProfilerEvent.ProfilerEventCallback((ref ProfilerEvent.EventData eventData, ref ProfilerEvent.PayloadMember[] extraPayload) => ProfilerCallbackFuncWithPUser(ref eventData, ref extraPayload, IntPtr.Zero));
        Utils.ShowError(ProfilerEvent.GetSupportedEvents(ref profiler, out ProfilerEvent.EventInfo[] supportedEvents));
        Console.WriteLine("\nEvents supported by this profiler:");
        foreach (var eventInfo in supportedEvents)
        {
            Console.WriteLine("\n{0}: {1}", eventInfo.EventName, eventInfo.EventId);
            Console.WriteLine("Register the callback function for the event {0}:", eventInfo.EventName);
            Utils.ShowError(ProfilerEvent.RegisterProfilerEventCallback(ref profiler, eventInfo.EventId, callbackFunc));
        }
        Console.WriteLine();

        // The program pauses for 20 seconds to allow the user to test if the profiler disconnection
        // event works properly. If the network cable is unplugged, the disconnection will be detected
        // and the callback function will be triggered.
        Console.WriteLine("Wait for 20 seconds for disconnection event.");
        Thread.Sleep(20000);

        if (!Utils.ConfirmCapture())
        {
            profiler.Disconnect();
            Console.WriteLine("Press any key to exit ...");
            Console.ReadKey();
            return 0;
        }

        var userSet = profiler.CurrentUserSet();

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

        profiler.Disconnect();
        Console.WriteLine("Disconnected from the profiler successfully.");
        Console.WriteLine("Press any key to exit ...");
        Console.ReadKey();
        return 0;
    }
}

