/*
With this sample, you can set the "Depth Range" parameter.
*/

using System;
using MMind.Eye;

class SetDepthRange
{
    static int Main()
    {
        var camera = new Camera();
        if (!Utils.FindAndConnect(ref camera))
            return -1;

        var currentUserSet = camera.CurrentUserSet();
        var depthRangeName = MMind.Eye.Scanning3DSetting.DepthRange.Name;

        // Set the range of depth values to 100–1000 mm.
        var range = new IntRange(100, 1000);
        Utils.ShowError(currentUserSet.SetRangeValue(depthRangeName, range));
        Utils.ShowError(currentUserSet.GetRangeValue(depthRangeName, ref range));
        Console.WriteLine("3D Scanning depth lower limit : {0} mm, depth upper limit : {1} mm.", range.Min, range.Max);

        camera.Disconnect();
        Console.WriteLine("Disconnected from the camera successfully.");
        Console.WriteLine("Press any key to exit ...");
        Console.ReadKey();
        return 0;
    }
}
