/*
With this sample, you can set the parameters specific to laser cameras (the DEEP and LSR series).
*/

using System;
using MMind.Eye;
class SetParametersOfLaserCameras
{
    static int Main()
    {
        var camera = new Camera();
        if (!Utils.FindAndConnect(ref camera))
            return -1;

        var currentUserSet = camera.CurrentUserSet();

        // Set the "Laser Power" parameter, which is the output power of the projector as a percentage of the maximum output power. This affects the
        // intensity of the projected structured light.
        int powerLevel = 80;
        Console.WriteLine("Set the output power of the laser projector to {0}% of the maximum power.", powerLevel);
        Utils.ShowError(currentUserSet.SetIntValue(MMind.Eye.LaserSetting.PowerLevel.Name, powerLevel));

        // Set the laser scan range. The entire projector FOV is from 0 to 100.
        IntRange range = new IntRange(20, 80);
        Console.WriteLine("Set the laser scan range from {0} to {1}.", range.Min, range.Max);
        Utils.ShowError(currentUserSet.SetRangeValue(MMind.Eye.LaserSetting.FrameRange.Name, range));

        // Set the "Fringe Coding Mode" parameter, which controls the pattern of the structured light. The "Fast" mode enhances the 
        // capture speed but provides lower depth data accuracy. The "Accurate" mode provides better depth data accuracy but reduces the capture speed.
        int mode = (int)MMind.Eye.LaserSetting.FringeCodingMode.Value.Accurate;
        Console.WriteLine("Set laser fringe coding mode of the projector to 'Accurate'.", mode);
        Utils.ShowError(currentUserSet.SetEnumValue(MMind.Eye.LaserSetting.FringeCodingMode.Name, mode));

        // Set the laser scan partition count. If the set value is greater than 1, the scan of the entire FOV 
        // will be partitioned into multiple parts. It is recommended to use multiple parts for
        // extremely dark objects.
        int framePartitionCount = 2;
        Console.WriteLine("Set the laser scan partition count to {0}.", framePartitionCount);
        Utils.ShowError(currentUserSet.SetIntValue(MMind.Eye.LaserSetting.FramePartitionCount.Name, framePartitionCount));

        camera.Disconnect();
        Console.WriteLine("Disconnected from the camera successfully.");
        Console.WriteLine("Press any key to exit ...");
        Console.ReadKey();
        return 0;
    }
}

