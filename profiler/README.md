# C# Samples for Mech-Eye 3D Laser Profiler

This documentation provides descriptions of Mech-Eye API C# samples for Mech-Eye 3D Laser Profiler and instructions for building the samples on Windows.

If you have any questions or have anything to share, feel free to post on the [Mech-Mind Online Community](https://community.mech-mind.com/). The community also contains a [specific category for development with Mech-Eye SDK](https://community.mech-mind.com/c/mech-eye-sdk-development/19).

## Sample List

Currently, the following samples are provided.

The samples marked with `(OpenCV)` require [OpenCV](https://opencv.org/releases/) to be installed.  

* [TriggerWithSoftwareAndFixedRate](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/master/profiler/TriggerWithSoftwareAndFixedRate) `(OpenCV)`  
  Trigger data acquisition with signals input from software, trigger line scans at a fixed rate, and then retrieve and save the acquired data.
* [TriggerWithExternalDeviceAndFixedRate](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/master/profiler/TriggerWithExternalDeviceAndFixedRate) `(OpenCV)`  
  Trigger data acquisition with signals input from the external device, trigger line scans at a fixed rate, and then retrieve and save the acquired data.

* [TriggerWithSoftwareAndEncoder](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/master/profiler/TriggerWithSoftwareAndEncoder) `(OpenCV)`  
  Trigger data acquisition with signals input from software, trigger line scans with signals input from the encoder, and then retrieve and save the acquired data.
* [TriggerWithExternalDeviceAndEncoder](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/master/profiler/TriggerWithExternalDeviceAndEncoder) `(OpenCV)`  
  Trigger data acquisition with signals input from the external device, trigger line scans with signals input from the encoder, and then retrieve and save the acquired data.
* [TriggerMultipleProfilersSimultaneously](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/master/profiler/TriggerMultipleProfilersSimultaneously) `(OpenCV)`  
  Trigger multiple laser profilers to acquire data asynchronously and retrieve the acquired data.
* [BlindSpotFiltering](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/master/profiler/BlindSpotFiltering) `(OpenCV)`  
  Detect and remove the false data caused by blind spots and obtain the filtered profile data.
* [NoiseRemoval](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/master/profiler/NoiseRemoval) `(OpenCV)`  
  Remove the noise in the depth data and obtain the filtered profile data.
* [ProfileAlignment](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/master/profiler/ProfileAlignment) `(OpenCV)`  
  Correct the X-axis and Z-axis vibrations in the profiles (aligning the profiles) and obtain the corrected profile data.
* [RenderDepthMap](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/master/profiler/RenderDepthMap) `(OpenCV)`  
    Obtain and save the depth map rendered with the jet color scheme.
* [TransformPointCloud](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/master/profiler/TransformPointCloud)  
  Obtain and save the point clouds in the custom reference frame.
* [ManageUserSets](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/master/profiler/ManageUserSets)  
  Manage parameter groups, such as obtaining the names of all parameter groups, adding a parameter group, switching the parameter group, and saving parameter settings to the parameter group.
* [PrintProfilerStatus](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/master/profiler/PrintProfilerStatus)  
  Obtain and print the laser profiler's information, such as model, serial number, firmware version, and temperatures.
* [RegisterProfilerEvent](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/master/profiler/RegisterProfilerEvent)  
  Define and register the callback function for monitoring laser profiler events.
* [UseVirtualDevice](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/master/profiler/UseVirtualDevice) `(OpenCV)`  
  Acquire the profile data stored in a virtual device, generate the intensity image and depth map, and save the images.

## Build the Samples

### Install Required Software

Please download and install the required software listed below.

* [Mech-Eye SDK (latest version)](https://downloads.mech-mind.com/?tab=tab-sdk)
* [Visual Studio (version 2017 or above)](https://visualstudio.microsoft.com/vs/community/)

  > Note: When installing, select the following workloads and individual component.
  >
  >* Workloads in the **Desktop & Mobile** category:
  >
  >   * **.NET desktop development**
  >   * **Desktop development with C++**
  >   * **Universal Windows Platform development**
  >
  >* Individual component: **.NET Framework 4.8 targeting pack**

  > Caution: C# Mech-Eye API is developed based on .NET Framework 4.8. If .NET Framework 4.8 is not installed, the samples cannot be built.

* Emgu CV: The **CaptureDepthMap** sample contains functions that depend on the OpenCV software libraries. Therefore, Emgu CV (the .NET wrapper for OpenCV) must be installed through NuGet Package Manager in Visual Studio. For detailed instructions, refer to [the guide provided by Microsoft](https://learn.microsoft.com/en-us/nuget/consume-packages/install-use-packages-visual-studio).

### Instructions

1. Double-click **MechEyeCSharpSamples.sln** in the `profiler` folder.
2. In Visual Studio toolbar, change the solution configuration from **Debug** to **Release**.
3. In the menu bar, select **Build** > **Build Solution**. An EXE format executable file is generated for each sample. The executable files are saved to the `Build` folder, located in the `source` folder.
4. In the **Solution Explorer** panel, right-click a sample, and select **Set as Startup Project**.
5. Click the **Local Windows Debugger** button in the toolbar to run the sample.
6. Enter the index of the laser profiler to which you want to connect, and press the Enter key. The obtained files are saved to the `Build` folder.
