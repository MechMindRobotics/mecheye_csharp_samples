/*
With this sample, you can obtain and print the camera intrinsic parameters.
*/

using System;
using MMind.Eye;

class GetCameraIntrinsics
{
    static int Main()
    {
        var camera = new Camera();
        if (!Utils.FindAndConnect(ref camera))
            return -1;

        var intrinsics = new CameraIntrinsics();
        Utils.ShowError(camera.GetCameraIntrinsics(ref intrinsics));
        Utils.PrintCameraIntrinsics(intrinsics);

        camera.Disconnect();
        Console.WriteLine("Disconnected from the camera successfully.");
        Console.WriteLine("Press any key to exit ...");
        Console.ReadKey();
        return 0;
    }
}

