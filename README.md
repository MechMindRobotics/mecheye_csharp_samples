# Mech-Eye C# Samples

This repository contains C# samples with the Mech-Eye Industrial 3D camera. 

## Installation

1. Download and install [Mech-Eye SDK_1.5.0](https://www.mech-mind.com.cn/down.aspx?TypeId=31&fid=t14:31:14).

2. Clone this repository to a location where you want.

3. Open MechEyeNETSamples.sln file in Visual Studio (2019 is recommended), build and run it.

   > Make sure .Net Desktop Development is installed in Visual Studio.

## Sample list

- **[ConnectAndCaptureImage](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/main/samples/connectAndCaptureImage)**

    Connect the Mech-Eye Camera and capture 2D and 3D data.

- **[SetAndGetParameter](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/main/samples/setAndGetParameter)**

    Set and get the settings and usersets from the Mech-Eye Camera.

- **[CaptureResultToOpenCV](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/main/samples/captureResultToOpenCV)**

    Capture 2D and 3D data with OpenCV data structure from the Mech-Eye Camera.

    > This sample requires [Emgu.CV.runtime.windows](https://www.nuget.org/packages/Emgu.CV.runtime.windows/) to be installed via NuGet.

- **[CaptureResultToPLY](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/main/samples/captureResultToPLY)**

    Capture 2D and 3D data with PCL data structure from the Mech-Eye Camera.

    > This sample requires [Emgu.CV.runtime.windows](https://www.nuget.org/packages/Emgu.CV.runtime.windows/) to be installed via NuGet.
