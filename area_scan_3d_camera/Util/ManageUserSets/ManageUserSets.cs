/*
With this sample, you can manage user sets, such as obtaining the names of all user sets, adding a user set, switching the user set, and saving parameter settings to the user set.
*/

using System;
using System.Collections.Generic;
using MMind.Eye;

class ManageUserSets
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

        // Obtain the name of the currently selected user set.
        var curSettings = userSetManager.CurrentUserSet();
        Console.WriteLine("The current user set: {0}", curSettings.GetName());

        // Add a new user set.
        string newSetting = "NewSetting";
        Utils.ShowError(userSetManager.AddUserSet(newSetting));
        Console.WriteLine("Add a new user set with the name of \"{0}\".", newSetting);
        Console.WriteLine("");

        // Select a user set by its name.
        Utils.ShowError(userSetManager.SelectUserSet(newSetting));
        Console.WriteLine("Select the \"{0}\" user set.", newSetting);
        Console.WriteLine("");

        // Save all the parameter settings to the currently selected user set.
        Utils.ShowError(curSettings.SaveAllParametersToDevice());
        Console.WriteLine("Save the current parameter settings to the selected user set.");

        camera.Disconnect();
        Console.WriteLine("Disconnected from the camera successfully.");
        Console.WriteLine("Press any key to exit ...");
        Console.ReadKey();
        return 0;
    }
}
