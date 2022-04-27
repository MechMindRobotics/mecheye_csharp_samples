# C# Samples

This repository contains C# samples for Mech-Eye SDK.

## Installation

1. Download and install [Mech-Eye SDK](https://www.mech-mind.com/download/camera-sdk.html).
2. Clone this repository to a specific folder.
3. Open MechEyeCSharpSamples.sln file in Visual Studio (2019 is recommended), build and run it.
    > Make sure .Net Development is installed in Visual Studio.

## Sample list

There are four categoires of samples: **Basic**, **Advanced**, **Util**, and **Laser**.  

- The category **Basic** contains samples that are related to basic connecting and capturing.  
- The category **Advanced** contains samples that use advanced capturing tricks.  
- The category **Util** contains samples that get and print information and set parameters.  
- The category **Laser** contains samples that can only be used on Mech-Eye Laser cameras.  

The samples marked with `(EmguCV)` require [Emgu.CV.runtime.windows](https://www.nuget.org/packages/Emgu.CV.runtime.windows/) to be installed via NuGet.

- **Basic**
  - [ConnectToCamera](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/main/source/Basic/ConnectToCamera)  
    Connects to a Mech-Eye Industrial 3D Camera.
  - [ConnectAndCaptureImage](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/main/source/Basic/ConnectAndCaptureImage)  
    Connects to a Mech-Eye Industrial 3D Camera and capture 2D and 3D data.
  - [CaptureColorMap](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/main/source/Basic/CaptureColorMap) `(EmguCV)`  
    Capture color map data with OpenCV data structure from a camera.
  - [CaptureDepthMap](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/main/source/Basic/CaptureDepthMap) `(EmguCV)`  
    Capture depth map data with OpenCV data structure from a camera.
  - [CapturePointCloud](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/main/source/Basic/CapturePointCloud) `(EmguCV)`  
    Capture point clouds with PCL data structure from a camera.
  - [CaptureHDRPointCloud](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/main/source/Basic/CaptureHDRPointCloud) `(EmguCV)`  
    Capture point clouds in HDR mode with PCL data structure from a camera.
  - [CapturePointCloudROI](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/main/source/Basic/CapturePointCloudROI) `(EmguCV)`  
    Capture point clouds with ROI enabled with PCL data structure from a camera.
- **Advanced**
  - [CaptureCloudFromDepth](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/main/source/Advanced/CaptureCloudFromDepth) `(EmguCV)`  
    Construct point clouds from depth map and color map captured from a camera.
  - [CaptureSequentiallyMultiCamera](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/main/source/Advanced/CaptureSequentiallyMultiCamera) `(EmguCV)`  
    Capture sequentially from multiple cameras.
  - [CaptureSimultaneouslyMultiCamera](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/main/source/Advanced/CaptureSimultaneouslyMultiCamera) `(EmguCV)`  
    Capture simultaneously from multiple cameras.
  - [CaptureTimedAndPeriodically](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/main/source/Advanced/CaptureTimedAndPeriodically) `(EmguCV)`  
    Capture periodically for a specific time from a camera.
- **Util**
  - [GetCameraIntri](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/main/source/Util/GetCameraIntri)  
    Get and print a camera's intrinsic parameters.
  - [PrintDeviceInfo](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/main/source/Util/PrintDeviceInfo)  
    Get and print a camera's information.
  - [SetDepthRange](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/main/source/Util/SetDepthRange)  
    Set the depth range of a camera.
  - [SetParameters](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/main/source/Util/SetParameters)  
    Set and get the parameters from a camera.
  - [SetUserSets](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/main/source/Util/SetUserSets)  
    Get current user set name and available user sets, save settings to a specific user set. The User Set feature allows the user to customize and store the individual settings.
- **Laser**
  - [SetLaserFramePartitionCount](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/main/source/Laser/SetLaserFramePartitionCount)  
    Set the laser scan partition count for a Mech-Eye Laser camera.
  - [SetLaserFrameRange](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/main/source/Laser/SetLaserFrameRange)  
    Set the laser scan range for a Mech-Eye Laser camera.
  - [SetLaserFringeCodingMode](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/main/source/Laser/SetLaserFringeCodingMode)  
    Set the fringe coding mode for a Mech-Eye Laser camera.
  - [SetLaserPowerLevel](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/main/source/Laser/SetLaserPowerLevel)  
    Set the laser power level for a Mech-Eye Laser camera.
    
    
## License

Mech-Eye Samples are distributed under the [BSD license](https://github.com/MechMindRobotics/mecheye_cpp_samples/blob/main/LICENSE).
