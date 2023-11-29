/*
With this sample, you can obtain and print the camera information, such as model, serial number, firmware version, and temperatures.
*/

using System;
using MMind.Eye;

class PrintCameraInfo
{
    static int Main()
    {
        var camera = new Camera();
        if (!Utils.FindAndConnect(ref camera))
            return -1;

        var cameraInfo = new CameraInfo();
        Utils.ShowError(camera.GetCameraInfo(ref cameraInfo));
        Utils.PrintCameraInfo(cameraInfo);

        var cameraStatus = new CameraStatus();
        Utils.ShowError(camera.GetCameraStatus(ref cameraStatus));
        Utils.PrintCameraStatus(cameraStatus);

        camera.Disconnect();
        Console.WriteLine("Disconnected from the camera successfully.");
        Console.WriteLine("Press any key to exit ...");
        Console.ReadKey();
        return 0;
    }
}

