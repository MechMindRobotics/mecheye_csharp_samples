/*
With this sample, you can define and register the callback function for monitoring camera events.
*/

using System;
using System.Threading;
using Emgu.CV;
using Emgu.CV.CvEnum;
using MMind.Eye;

class RegisterCameraEvent
{
    // Define the callback function of event
    private static void CallbackFunc(CameraEvent.Event cameraEvent, IntPtr pUser)
    {
        Console.WriteLine("A camera event has occurred. The event name is {0}", cameraEvent);
    }


    // Define the callback function of event
    private static void CameraCallbackFuncWithPUser(ref EventData eventData, ref CameraEvent.PayloadMember[] extraPayload, IntPtr pUser)
    {
        Console.WriteLine("A camera event has occurred.");
        Console.WriteLine("\tThe event ID is {0}", eventData.eventId);
        if (!string.IsNullOrEmpty(eventData.eventName))
            Console.WriteLine("\tThe event name is {0}", eventData.eventName);
        Console.WriteLine("\tThe event Frame ID is {0}", eventData.frameId);
        DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        DateTime timestamp = epoch.AddMilliseconds(eventData.timestamp);
        Console.WriteLine("\tThe event timestamp is {0}", timestamp.ToString("yyyy-MM-dd HH:mm:ss"));
        foreach (var member in extraPayload)
        {
            switch (member.Type)
            {
                case CameraEvent.PayloadMemberType._UInt32:
                    Console.WriteLine("\tThe event {0} is {1}", member.Name, member.Value.UInt32Value);
                    break;
                case CameraEvent.PayloadMemberType._Int32:
                    Console.WriteLine("\tThe event {0} is {1}", member.Name, member.Value.Int32Value);
                    break;
                case CameraEvent.PayloadMemberType._Int64:
                    Console.WriteLine("\tThe event {0} is {1}", member.Name, member.Value.Int64Value);
                    break;
                case CameraEvent.PayloadMemberType._Float:
                    Console.WriteLine("\tThe event {0} is {1}", member.Name, member.Value.FloatValue);
                    break;
                case CameraEvent.PayloadMemberType._Double:
                    Console.WriteLine("\tThe event {0} is {1}", member.Name, member.Value.DoubleValue);
                    break;
                case CameraEvent.PayloadMemberType._Bool:
                    Console.WriteLine("\tThe event {0} is {1}", member.Name, member.Value.BoolValue);
                    break;
                case CameraEvent.PayloadMemberType._String:
                    Console.WriteLine("\tThe event {0} is {1}", member.Name, member.Value.StringValue);
                    break;
            }
        }
    }

    static int Main()
    {
        var camera = new Camera();
        if (!Utils.FindAndConnect(ref camera))
        {
            Console.WriteLine("Press any key to exit ...");
            Console.ReadKey();
            return -1;
        }

        // Set the heartbeat interval to 2 seconds
        camera.SetHeartbeatInterval(2000);

        var callbackFunc = new CameraEvent.CameraEventCallbackWithPayloadMember((ref EventData eventData, ref CameraEvent.PayloadMember[] extraPayload) => CameraCallbackFuncWithPUser(ref eventData, ref extraPayload, IntPtr.Zero));
        Utils.ShowError(CameraEvent.GetSupportedEvents(ref camera, out CameraEvent.EventInfo[] supportedEvents));
        Console.WriteLine("\nEvents supported by this camera:");
        foreach (var eventInfo in supportedEvents)
        {
            Console.WriteLine("\n{0}: {1}", eventInfo.EventName, eventInfo.EventId);
            Console.WriteLine("Register the callback function for the event {0}:", eventInfo.EventName);
            Utils.ShowError(CameraEvent.RegisterCameraEventCallback(ref camera, eventInfo.EventId, callbackFunc));
        }
        Console.WriteLine();

        // Let the program sleep for 20 seconds. During this period, if the camera is disconnected, the
        // callback function will detect and report the disconnection. To test the event mechanism, you 
        // can disconnect the camera Ethernet cable during this period.
        Console.WriteLine("Wait for 20 seconds for disconnection event.");
        Thread.Sleep(20000);

        if (!Utils.ConfirmCapture3D())
        {
            camera.Disconnect();
            Console.WriteLine("Press any key to exit ...");
            Console.ReadKey();
            return 0;
        }

        // If the 3D data has been acquired successfully, the callback function will detect the
        // CAMERA_EVENT_EXPOSURE_END event.
        // Note: The CAMERA_EVENT_EXPOSURE_END event is only sent after the acquisition of the 3D data
        // (Frame3D) has completed. To ensure both 2D and 3D data have been acquired before the event is
        // sent, check the following recommendations: If the flash exposure mode is used for acquiring
        // the 2D data, and the @ref FlashAcquisitionMode parameter is set to "Fast", call Capture3D()
        // before calling Capture2D(). Otherwise, call Capture2D() before calling Capture3D().
        // Alternatively, you can call Capture2DAnd3D() instead to avoid the timing issue.
        var frame = new Frame3D();
        Utils.ShowError(camera.Capture3D(ref frame));

        var depth = frame.GetDepthMap();

        // Use Emgu.CV to save the depth map as a single-channel 32-bits-per-pixel image.
        string depthFile = "DepthMap.tiff";
        var depth32F = new Mat(unchecked((int)depth.Height()), unchecked((int)depth.Width()), DepthType.Cv32F, 1, depth.Data(), unchecked((int)depth.Width()) * 4);
        CvInvoke.Imwrite(depthFile, depth32F);
        Console.WriteLine("Capture and save the depth map as a single-channel 32-bits-per-pixel image: {0}", depthFile);


        camera.Disconnect();
        Console.WriteLine("Disconnected from the camera successfully.");
        Console.WriteLine("Press any key to exit ...");
        Console.ReadKey();
        return 0;
    }
}
