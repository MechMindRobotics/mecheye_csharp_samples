/*
With this sample, you can set the parameters specific to the UHP series.
*/

using System;
using MMind.Eye;
class SetParametersOfUHPCameras
{
    static int Main()
    {
        var camera = new Camera();
        if (!Utils.FindAndConnect(ref camera))
            return -1;

        var currentUserSet = camera.CurrentUserSet();

        // Set the capture mode to "Merge"
        int mode = (int)MMind.Eye.UhpSetting.CaptureMode.Value.Merge;

        Console.WriteLine("Set the UHP capture mode to 'Merge'.");
        Utils.ShowError(currentUserSet.SetEnumValue(MMind.Eye.UhpSetting.CaptureMode.Name, mode));

        camera.Disconnect();
        Console.WriteLine("Disconnected from the camera successfully.");
        Console.WriteLine("Press any key to exit ...");
        Console.ReadKey();
        return 0;
    }
}

