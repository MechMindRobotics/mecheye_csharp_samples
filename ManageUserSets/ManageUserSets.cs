/*
This is a simple example of how to find and connect to an available Mech-Eye Device
and then Get and Set the user Sets in the Mech-Eye Device.
The User Set feature allows the user to customize and store the individual Settings.
*/

using System;
using System.Collections.Generic;
using MMind.Eye;

class ManageUserSets
{
    static int Main()
    {
        var profiler = new Profiler();
        if (!Utils.FindAndConnect(ref profiler))
            return -1;

        var userSetManager = profiler.UserSetManager();

        // Obtain the Names of all parameter groups.
        List<string> userSets = new List<string>();
        Utils.ShowError(userSetManager.GetAllUserSetNames(ref userSets));

        Console.WriteLine("All user Sets : ");
        foreach (var userSet in userSets)
        {
            Console.Write(userSet);
            Console.Write("  ");
        }
        Console.WriteLine("");

        // Obtain the Name of the currently selected parameter group.
        var curSettings = userSetManager.CurrentUserSet();
        Console.WriteLine("Current user Set: {0}", curSettings.GetName());

        // Select a parameter group by its Name.
        Utils.ShowError(userSetManager.SelectUserSet(userSets[0]));
        Console.WriteLine("Set \"{0}\" as the current user Set.", userSets[0]);
        Console.WriteLine("");

        // Save all the parameter Settings to the currently selected parameter group.
        Utils.ShowError(curSettings.SaveAllParametersToDevice());
        Console.WriteLine("Save all parameters to current user Set.");

        profiler.Disconnect();
        Console.WriteLine("Disconnected from the Mech-Eye profiler successfully.");
        Console.WriteLine("Press any key to exit ...");
        Console.ReadKey();
        return 0;
    }
}
