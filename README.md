# C# Samples

This documentation provides descriptions of Mech-Eye API C# samples and instructions for building all the samples at once.

If you have any questions or have anything to share, feel free to post on the [Mech-Mind Online Community](https://community.mech-mind.com/). The community also contains a [specific category for development with Mech-Eye SDK](https://community.mech-mind.com/c/mech-eye-sdk-development/19).

## Sample List

Currently, the following samples are provided.

The samples marked with `(EmguCV)` require [Emgu.CV.runtime.windows](https://www.nuget.org/packages/Emgu.CV.runtime.windows/) to be installed.  

- **AcquireProfileData** `(EmguCV)`
  Acquires the profile data, generates the intensity image and depth map, and saves the images.
- **AcquireProfileDataUsingCallback** `(EmguCV)`
  Acquires the profile data using a callback function, generates the intensity image and depth map, and saves the images.
- **AcquireRawImage**
  Acquires and saves the raw image.
- **AcquirePointCloud**
  Acquires the profile data, generates the point cloud, and saves the point cloud in the CSV format.
- **ManageUserSets**
  Performs functions related to parameter groups, such as obtaining the names of all available parameter groups, selecting a parameter group, and saving the parameter values to the current parameter group. The parameter group feature allows user to save and quickly apply a set of parameter values.

## Build the Samples

The instructions provided here allow you to build all the samples at once.

### Prerequisites

The following software are required to build the samples. Please download and install these software.

* [Mech-Eye SDK (latest version)](https://downloads.mech-mind.com/?tab=tab-sdk)
* [Visual Studio (version 2017 or above)](https://visualstudio.microsoft.com/vs/community/)

### Instructions

1. Open MechEyeCSharpSamples.sln file in Visual Studio (2019 recommended).
2. In Visual Studio, change the Solution Configuration from **Debug** to **Release**.
3. Right-click **Solution 'MechEyeCsharpSamples'** in **Solution Explorer**, and select **Build Solution**.
9. Navigate to the `Build` folder under the .sln file's directory, and run a sample.
10. Enter the index of the camera to which you want to connect, and press the Enter key. The obtained files are saved to the `Build` folder.