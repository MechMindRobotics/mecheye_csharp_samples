/*
With this sample, you can import and replace all user sets from a JSON file, and save all user sets to a JSON file.
*/

using System;
using System.Collections.Generic;
using MMind.Eye;

class SaveAndLoadUserSet
{
    static int Main()
    {
        var camera = new Camera();
        if (!Utils.FindAndConnect(ref camera))
            return -1;

        var userSetManager = camera.UserSetManager();

        // Obtain the names of all user sets.
        List<string> userSets = new List<string>();
        Utils.ShowError(userSetManager.GetAllUserSetNames(ref userSets));

        Console.WriteLine("All user sets : ");
        foreach (var userSet in userSets)
        {
            Console.Write(userSet);
            Console.Write("  ");
        }
        Console.WriteLine("");

        Console.WriteLine("Save all user sets to a JSON file.");
        Utils.ShowError(userSetManager.SaveToFile("camera_config.json"));
        Console.WriteLine("");

        Console.WriteLine("Import and replace all user sets from a JSON file.");
        Utils.ShowError(userSetManager.LoadFromFile("camera_config.json"));
        Console.WriteLine("");

        camera.Disconnect();
        Console.WriteLine("Disconnected from the camera successfully.");
        Console.WriteLine("Press any key to exit ...");
        Console.ReadKey();
        return 0;
    }
}
