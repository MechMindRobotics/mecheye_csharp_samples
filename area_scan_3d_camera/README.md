# C# Samples for Mech-Eye Industrial 3D Camera

This documentation provides descriptions of Mech-Eye API C# samples for Mech-Eye Industrial 3D Camera and instructions for building the samples on Windows.

If you have any questions or have anything to share, feel free to post on the [Mech-Mind Online Community](https://community.mech-mind.com/). The community also contains a [specific category for development with Mech-Eye SDK](https://community.mech-mind.com/c/mech-eye-sdk-development/19).

## Sample List

Samples are divided into the following categories: **Basic**, **Advanced**, and **Util**.

* **Basic** samples: Connect to the camera and acquire data.
* **Advanced** samples: Acquire data in more complicated manners and set model-specific parameters.
* **Util** samples: Obtain camera information and set common parameters.

The samples marked with `(EmguCV)` require [Emgu.CV.runtime.windows](https://www.nuget.org/packages/Emgu.CV.runtime.windows/) to be installed via NuGet.

* **Basic**
  * [ConnectToCamera](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/master/area_scan_3d_camera/Basic/ConnectToCamera)  
    Connect to a camera.
  * [ConnectAndCaptureImages](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/master/area_scan_3d_camera/Basic/ConnectAndCaptureImages)  
    Connect to a camera and obtain the 2D image, depth map, and point cloud data.
  * [Capture2DImage](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/master/area_scan_3d_camera/Basic/Capture2DImage) `(OpenCV)`  
    Obtain and save the 2D image.
  * [CaptureDepthMap](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/master/area_scan_3d_camera/Basic/CaptureDepthMap) `(OpenCV)`  
    Obtain and save the depth map.
  * [CapturePointCloud](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/master/area_scan_3d_camera/Basic/CapturePointCloud)  
    Obtain and save the untextured and textured point clouds.
  * [CapturePointCloudHDR](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/master/area_scan_3d_camera/Basic/CapturePointCloudHDR)  
    Set multiple exposure times, and then obtain and save the untextured and textured point clouds.
  * [CapturePointCloudWithNormals](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/master/area_scan_3d_camera/Basic/CapturePointCloudWithNormals)  
    Calculate normals and save the untextured and textured point clouds with normals.
* **Advanced**
  * [ConvertDepthMapToPointCloud](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/master/area_scan_3d_camera/Advanced/ConvertDepthMapToPointCloud)  
    Generate a point cloud from the depth map and save the point cloud.
  * [MultipleCamerasCaptureSequentially](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/master/area_scan_3d_camera/Advanced/MultipleCamerasCaptureSequentially)`(OpenCV)`  
    Obtain and save 2D images, depth maps, and point clouds sequentially from multiple cameras.
  * [MultipleCamerasCaptureSimultaneously](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/master/area_scan_3d_camera/Advanced/MultipleCamerasCaptureSimultaneously)`(OpenCV)`  
    Obtain and save 2D images, depth maps, and point clouds simultaneously from multiple cameras.
  * [CapturePeriodically](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/master/area_scan_3d_camera/Advanced/CapturePeriodically)`(OpenCV)`  
    Obtain and save 2D images, depth maps, and point clouds periodically for the specified duration from a camera.
  * [Mapping2DImageToDepthMap](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/master/area_scan_3d_camera/Advanced/Mapping2DImageToDepthMap)  
    Generate untextured and textured point clouds from a masked 2D image and a depth map.
  * [SetParametersOfLaserCameras](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/master/area_scan_3d_camera/Advanced/SetParametersOfLaserCameras)  
    Set the parameters specific to laser cameras.
  * [SetParametersOfUHPCameras](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/master/area_scan_3d_camera/Advanced/SetParametersOfUHPCameras)  
    Set the parameters specific to the UHP series.
  * [RegisterCameraEvent](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/master/area_scan_3d_camera/Advanced/RegisterCameraEvent)  
    Define and register the callback function for monitoring the camera connection status.
  * [CaptureStereo2DImages](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/master/area_scan_3d_camera/Advanced/CaptureStereo2DImages)  
    Obtain and save the 2D images from both 2D cameras.
    > Note: This sample is only applicable to the following models: Deep, Laser L Enhanced, PRO XS, LSR L, LSR S, and DEEP.
* **Util**
  * [GetCameraIntrinsics](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/master/area_scan_3d_camera/Util/GetCameraIntrinsics)  
    Obtain and print the camera intrinsic parameters.
  * [PrintCameraInfo](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/master/area_scan_3d_camera/Util/PrintCameraInfo)  
    Obtain and print the camera information, such as model, serial number, firmware version, and temperatures.
  * [SetScanningParameters](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/master/area_scan_3d_camera/Util/SetScanningParameters)  
    Set the parameters in the **3D Parameters**, **2D Parameters**, and **ROI** categories.
  * [SetDepthRange](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/master/area_scan_3d_camera/Util/SetDepthRange)  
    Set the **Depth Range** parameter.
  * [SetPointCloudProcessingParameters](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/master/area_scan_3d_camera/Util/SetPointCloudProcessingParameters)  
    Set the **Point Cloud Processing** parameters.
  * [ManageUserSets](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/master/area_scan_3d_camera/Util/ManageUserSets)  
    Manage parameter groups, such as obtaining the names of all parameter groups, adding a parameter group, switching the parameter group, and saving parameter settings to the parameter group.
  * [SaveAndLoadUserSet](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/master/area_scan_3d_camera/Util/SaveAndLoadUserSet)  
    Import and replace all parameter groups from a JSON file, and save all parameter groups to a JSON file.

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
  >
  > Caution: C# Mech-Eye API is developed based on .NET Framework 4.8. If .NET Framework 4.8 is not installed, the samples cannot be built.

* Emgu CV: The **CaptureDepthMap** sample contains functions that depend on the OpenCV software libraries. Therefore, Emgu CV (the .NET wrapper for OpenCV) must be installed through NuGet Package Manager in Visual Studio. For detailed instructions, refer to [the guide provided by Microsoft](https://learn.microsoft.com/en-us/nuget/consume-packages/install-use-packages-visual-studio).

### Instructions

1. Double-click **MechEyeCSharpSamples.sln** in the `area_scan_3d_camera` folder.
2. In Visual Studio toolbar, change the solution configuration from **Debug** to **Release**.
3. In the menu bar, select **Build** > **Build Solution**. An EXE format executable file is generated for each sample. The executable files are saved to the `Build` folder, located in the `source` folder.
4. In the **Solution Explorer** panel, right-click a sample, and select **Set as Startup Project**.
5. Click the **Local Windows Debugger** button in the toolbar to run the sample.
6. Enter the index of the camera to which you want to connect, and press the Enter key. The obtained files are saved to the `Build` folder.
