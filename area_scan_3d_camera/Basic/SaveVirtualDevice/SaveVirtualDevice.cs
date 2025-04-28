/*
With this sample, you can save the virtual device file.
Note: The virtual device file can be opened with Mech-Eye Viewer.
*/

using System;
using MMind.Eye;

class SaveVirtualDevice
{
    static int Main()
    {
        var camera = new Camera();
        if (!Utils.FindAndConnect(ref camera))
            return -1;

        Console.WriteLine("Start saving the virtual device file.This may take up to a few minutes.");
        // Enter the name for the virtual device file. Please ensure that the file name is encoded in UTF-8 format. 
        // You can add a path before the name to specify the path for saving the file.
        string virtualFile = "Camera.mraw";
        var successMessage = "The virtual device file has been saved.";
        Utils.ShowError(camera.saveVirtualDeviceFile(virtualFile), successMessage);

        camera.Disconnect();
        Console.WriteLine("Disconnected from the camera successfully.");
        Console.WriteLine("Press any key to exit ...");
        Console.ReadKey();
        return 0;
    }
}
