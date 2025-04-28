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
        var profiler = new Profiler();
        if (!Utils.FindAndConnect(ref profiler))
            return -1;

        var userSetManager = profiler.UserSetManager();

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
        var successMessage = "Add a new user set with the name of \"" + newSetting+ "\".";
        Utils.ShowError(userSetManager.AddUserSet(newSetting), successMessage);
        Console.WriteLine("");

        // Select a user set by its name.
        successMessage = "Select the \""+ newSetting +"\" user set.";
        Utils.ShowError(userSetManager.SelectUserSet(newSetting), successMessage);
        Console.WriteLine("");

        // Save all the parameter settings to the currently selected user set.
        successMessage = "Save the current parameter settings to the selected user set.";
        Utils.ShowError(curSettings.SaveAllParametersToDevice(), successMessage);

        profiler.Disconnect();
        Console.WriteLine("Disconnected from the profiler successfully.");
        Console.WriteLine("Press any key to exit ...");
        Console.ReadKey();
        return 0;
    }
}
