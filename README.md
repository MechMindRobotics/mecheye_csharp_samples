# C# Samples

This repository contains C# samples for Mech-Eye SDK.

## Installation

1. Download and install [Mech-Eye SDK](https://www.mech-mind.com/download/softwaredownloading.html).
2. Clone this repository to a specific folder.
3. Open MechEyeCSharpSamples.sln file in Visual Studio (2019 recommended), build it, and run it.
     For software requirements, please refer to [Mech-Eye API User Manual](https://docs.mech-mind.net/latest/en-GB/MechEye/MechEyeAPI/Samples/Samples.html)
     
## Sample List

Samples are divided into five categories, **Basic**, **Advanced**, **Util**, **Laser** and **UHP**.

- **Basic**: camera connection and basic capturing functions.
- **Advanced**: advanced capturing functions.
- **Util**: obtain information from a camera and set camera parameters.
- **Laser**: for Laser, LSR and DEEP series cameras only.
- **UHP**: for UHP series cameras only. 

The samples marked with `(EmguCV)` require [Emgu.CV.runtime.windows](https://www.nuget.org/packages/Emgu.CV.runtime.windows/) to be installed via NuGet.

- **Basic**
  - [ConnectToCamera](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/main/source/Basic/ConnectToCamera)  
    Connect to a Mech-Eye Industrial 3D Camera.
  - [ConnectAndCaptureImage](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/main/source/Basic/ConnectAndCaptureImage)  
    Connect to a camera and obtain the 2D image, depth map and point cloud data.
  - [CaptureColorMap](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/main/source/Basic/CaptureColorMap) `(EmguCV)`  
    Obtain and save the 2D image from a camera.
  - [CaptureDepthMap](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/main/source/Basic/CaptureDepthMap) `(EmguCV)`  
    Obtain and save the depth map from a camera.
  - [CapturePointCloud](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/main/source/Basic/CapturePointCloud) `(EmguCV)`  
    Obtain and save untextured and textured point clouds generated from images captured with a single exposure time.
  - [CaptureHDRPointCloud](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/main/source/Basic/CaptureHDRPointCloud) `(EmguCV)`  
    Obtain and save untextured and textured point clouds generated from images captured with multiple exposure times.
  - [CapturePointCloudROI](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/main/source/Basic/CapturePointCloudROI) `(EmguCV)`  
    Obtain and save untextured and textured point clouds of the objects in the ROI from a camera.
  - [CapturePointCloudFromTextureMask](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/main/source/Basic/CapturePointCloudFromTextureMask) `(EmguCV)`  
    Construct and save untextured and textured point clouds generated from a depth map and masked 2D image.
- **Advanced**
  - [CaptureCloudFromDepth](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/main/source/Advanced/CaptureCloudFromDepth) `(EmguCV)`  
    Construct and save point clouds from the depth map and 2D image obtained from a camera.
  - [CaptureSequentiallyMultiCamera](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/main/source/Advanced/CaptureSequentiallyMultiCamera) `(EmguCV)`  
    Obtain and save 2D images, depth maps and point clouds sequentially from multiple cameras.
  - [CaptureSimultaneouslyMultiCamera](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/main/source/Advanced/CaptureSimultaneouslyMultiCamera) `(EmguCV)`  
    Obtain and save 2D images, depth maps and point clouds simultaneously from multiple cameras.
  - [CaptureTimedAndPeriodically](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/main/source/Advanced/CaptureTimedAndPeriodically) `(EmguCV)`  
    Obtain and save 2D images, depth maps and point clouds periodically for the specified duration from a camera.
- **Util**
  - [GetCameraIntri](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/main/source/Util/GetCameraIntri)  
    Get and print a camera's intrinsic parameters.
  - [PrintDeviceInfo](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/main/source/Util/PrintDeviceInfo)  
    Get and print a camera's information such as model, serial number and firmware version.
  - [SetDepthRange](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/main/source/Util/SetDepthRange)  
    Set the range of depth values to be retained by a camera.
  - [SetParameters](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/main/source/Util/SetParameters)  
    Set specified parameters to a camera.
  - [SetUserSets](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/main/source/Util/SetUserSets)  
    Perform functions related to parameter groups, such as getting the names of available parameter groups, switching parameter group, and saving the current parameter values to a specific parameter group. The parameter group feature allows user to save and quickly apply a set of parameter values.
- **Laser**
  - [SetLaserFramePartitionCount](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/main/source/Laser/SetLaserFramePartitionCount)  
    Divide the projector FOV into partitions and project structured light in one partition at a time. The output of the entire FOV is composed from images of all partitions.
  - [SetLaserFrameRange](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/main/source/Laser/SetLaserFrameRange)  
    Set the projection range of the structured light. The entire projector FOV is from 0 to 100.
  - [SetLaserFringeCodingMode](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/main/source/Laser/SetLaserFringeCodingMode)  
    Set the coding mode of the structured light pattern.
  - [SetLaserPowerLevel](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/main/source/Laser/SetLaserPowerLevel)  
    Set the output power of the laser projector in percentage of max power. This affects the intensity of the laser light.
- **UHP**
  - [SetUHPCaptureMode](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/master/source/UHP/SetUHPCaptureMode)  
    Set the capture mode (capture images with camera 1, with camera 2, or with both 2D cameras and compose the outputs).
  - [SetUHPFringeCodingMode](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/master/source/UHP/SetUHPFringeCodingMode)  
    Set the coding mode of the structured light pattern.