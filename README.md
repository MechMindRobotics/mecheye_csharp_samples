# Mech-Eye C# Samples

This repository contains C# samples for Mech-Eye SDK_1.5.1.

## Installation

1. Download and install [Mech-Eye SDK_1.5.1](https://www.mech-mind.com/download/CameraSDK.html).

2. Clone this repository to a specific folder.

3. Open MechEyeCSharpSamples.sln file in Visual Studio (2019 is recommended), build and run it.

   > Make sure .Net Desktop Development is installed in Visual Studio.

## Sample list

- **[ConnectAndCaptureImage](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/main/samples/connectAndCaptureImage)**

    Connect to the Mech-Eye Camera and capture 2D and 3D data.

- **[SetAndGetParameter](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/main/samples/setAndGetParameter)**

    Set and get the settings and usersets from the Mech-Eye Camera.

- **[CaptureResultToOpenCV](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/main/samples/captureResultToOpenCV)**

    Capture 2D and 3D data with OpenCV data structure from the Mech-Eye Camera.

    > This sample requires [Emgu.CV.runtime.windows](https://www.nuget.org/packages/Emgu.CV.runtime.windows/) to be installed via NuGet.

- **[CaptureResultToPLY](https://github.com/MechMindRobotics/mecheye_csharp_samples/tree/main/samples/captureResultToPLY)**

    Capture 2D and 3D data with PCL data structure from the Mech-Eye Camera.

    > This sample requires [Emgu.CV.runtime.windows](https://www.nuget.org/packages/Emgu.CV.runtime.windows/) to be installed via NuGet.
