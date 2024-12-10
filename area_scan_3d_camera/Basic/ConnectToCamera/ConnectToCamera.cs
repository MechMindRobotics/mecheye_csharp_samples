/*
With this sample, you can connect to a camera.
*/

using System;
using MMind.Eye;

class ConnectToCamera
{
    static int Main()
    {
        Console.WriteLine("Find Mech-Eye devices...");
        var cameraInfoList = Camera.DiscoverCameras();

        if (cameraInfoList.Count == 0)
        {
            Console.WriteLine("No Mech-Eye camera found.");
            return -1;
        }

        // Display the information of all available cameras.
        for (int i = 0; i < cameraInfoList.Count; ++i)
        {
            Console.WriteLine("Mech-Eye camera index : {0}", i);
            Utils.PrintCameraInfo(cameraInfoList[i]);
        }

        Console.WriteLine("Please enter the camera index you want to connect: ");
        int inputIndex;

        // Enter the index of the camera to be connected and check if the index is valid.
        while (true)
        {
            string input = Console.ReadLine();
            if (int.TryParse(input, out inputIndex) && inputIndex >= 0 && inputIndex < cameraInfoList.Count)
                break;
            Console.WriteLine("Input invalid! Please enter the camera index you want to connect: ");
        }

        var camera = new Camera();
        var status = camera.Connect(cameraInfoList[inputIndex], 10000);

        if (status.ErrorCode != (int)ErrorCode.MMIND_STATUS_SUCCESS)
        {
            Utils.ShowError(status);
            return -1;
        }

        Console.WriteLine("Connected to the camera successfully.");

        camera.Disconnect();
        Console.WriteLine("Disconnected from the camera successfully.");
        Console.WriteLine("Press any key to exit ...");
        Console.ReadKey();
        return 0;
    }
}
