/*
With this sample, you can define and register the callback function for monitoring the camera connection status.
*/

using System;
using System.Threading;
using MMind.Eye;

class RegisterCameraEvent
{
    // Define the callback function of event
    private static void CallbackFunc(CameraEvent.Event cameraEvent, IntPtr pUser)
    {
        Console.WriteLine("A camera event has occurred. The event name is {0}", cameraEvent);
    }

    static int Main()
    {
        var camera = new Camera();
        if (!Utils.FindAndConnect(ref camera))
            return -1;

        // Set the heartbeat interval to 2 seconds
        camera.SetHeartbeatInterval(2000);
        Console.WriteLine("Register the callback function for camera disconnection events.");
        // Register the callback function, and the type of event is CAMERA_EVENT_DISCONNECTED
        Utils.ShowError(CameraEvent.RegisterCameraEventCallback(ref camera, CallbackFunc, IntPtr.Zero, (uint)CameraEvent.Event.CAMERA_EVENT_DISCONNECTED));
        // Let the program sleep for 20 seconds. During this period, if the camera is disconnected, the
        // callback function will detect and report the disconnection.
        Thread.Sleep(20000);

        camera.Disconnect();
        Console.WriteLine("Disconnected from the camera successfully.");
        Console.WriteLine("Press any key to exit ...");
        Console.ReadKey();
        return 0;
    }
}
