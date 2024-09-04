/*
With this sample, you can define and register the callback function for monitoring the camera connection status.
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
    private static void CameraCallbackFuncWithPUser(ref EventData eventData, IntPtr extraPayload, IntPtr pUser)
    {
        Console.WriteLine("A camera event has occurred.");
        Console.WriteLine("\tThe event name is {0}", eventData.eventId);
        Console.WriteLine("\tThe event timestamp is {0}", eventData.timestamp);
    }
    private static void CameraCallbackFunc(ref EventData eventData, IntPtr extraPayload)
    {
        Console.WriteLine("A camera event has occurred."); 
        Console.WriteLine("\tThe event name is {0}", eventData.eventId);
        DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        DateTime timestamp = epoch.AddMilliseconds(eventData.timestamp);
        Console.WriteLine("\tThe event timestamp is {0}", timestamp.ToString("yyyy-MM-dd HH:mm:ss"));
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

        Console.WriteLine("Register the callback function for exposure end event.");
        // Register a callback function with PUser parameter, and the type of event is CAMERA_EVENT_EXPOSURE_END
         
        var callbackFunc = new CameraEvent.CameraEventCallback((ref EventData eventData, IntPtr extraPayload) => CameraCallbackFuncWithPUser(ref eventData, extraPayload, IntPtr.Zero));
         
        Utils.ShowError(CameraEvent.RegisterCameraEventCallback(ref camera,  CameraEvent.Event.CAMERA_EVENT_EXPOSURE_END, callbackFunc));
        Console.WriteLine("Unregister the callback function for exposure end event.");

        Utils.ShowError(CameraEvent.UnregisterCameraEventCallback(ref camera, CameraEvent.Event.CAMERA_EVENT_EXPOSURE_END));

        Console.WriteLine("Register the callback function for exposure end event.");
        // Register callback function for event  CAMERA_EVENT_EXPOSURE_END
        Utils.ShowError(CameraEvent.RegisterCameraEventCallback(ref camera, CameraEvent.Event.CAMERA_EVENT_EXPOSURE_END,  CameraCallbackFunc));

        if (!Utils.ConfirmCapture3D())
        {
            camera.Disconnect();
            return 0;
        }

        // Note: The CAMERA_EVENT_EXPOSURE_END event is only sent after the acquisition of the 3D data
        // (Frame3D) has completed. To ensure both 2D and 3D data have been acquired before the event is
        // sent, check the following recommendations: If the flash exposure mode is used for acquiring
        // the 2D data, and the @ref FlashAcquisitionMode parameter is set to "Fast", call capture3D()
        // before calling capture2D(). Otherwise, call capture2D() before calling capture3D().
        // Alternatively, you can call capture2Dand3D() instead to avoid the timing issue.
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
