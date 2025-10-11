/*
With this sample, perform multi profiler calibration through.
*/

using System;
using MMind.Eye;
using Emgu.CV;
using Emgu.CV.CvEnum;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;

public enum CommandType
{
  Unknown,
  Calibration,
  StitchSingleMajorDevice,
  StitchMultiMajorDevice
}

class MultiProfilerCalibration
{
  private const int CV_8UC1 = 0;
  private const int CV_32FC1 = 5;

  // Get user input as a string
  public static string GetInputCommand()
  {
    string command = Console.ReadLine();
    if (command == null)
      return string.Empty;
    command = command.Trim();
    int spaceIndex = command.IndexOf(' ');
    if (spaceIndex > 0)
    {
      command = command.Substring(0, spaceIndex);
    }
    return command;
  }

  public static T GetInputNumber<T>() where T : IConvertible
  {
    while (true)
    {
      string input = Console.ReadLine();
      try
      {
        if (typeof(T) == typeof(bool))
        {
          input = input.Trim().ToLower();
          if (input == "true" || input == "1")
            return (T)Convert.ChangeType(true, typeof(T));
          if (input == "false" || input == "0")
            return (T)Convert.ChangeType(false, typeof(T));
          throw new Exception();
        }
        return (T)Convert.ChangeType(input, typeof(T));
      }
      catch
      {
        Console.WriteLine($"Invalid input! Please enter a valid number.");
      }
    }
  }

  /*Prompts the user to input a boolean valueand validates the input.
  Accepts "true", "false", "1", or "0" (case-insensitive).*/
  public static bool GetInputBool()
  {
    while (true)
    {
      string inputStr = Console.ReadLine();
      if (inputStr == null)
        continue;
      inputStr = inputStr.Trim().ToLower();
      if (inputStr == "true" || inputStr == "1")
      {
        return true;
      }
      else if (inputStr == "false" || inputStr == "0")
      {
        return false;
      }
      else
      {
        Console.WriteLine("Invalid input. Please enter 'true', 'false', '1', or '0'.");
      }
    }
  }

  // Prompt the user to select a command type
  public static CommandType EnterCommand()
  {
    Console.WriteLine("\nEnter the letter that represents the action you want to perform.");
    Console.WriteLine("C: Calibration and stitching using real-time camera capture.");
    Console.WriteLine("S: Stitching using historical calibration files with single major device.");
    Console.WriteLine("M: Stitching using historical calibration files with multi major device.");
    var command = GetInputCommand();

    if (command == "C" || command == "c")
      return CommandType.Calibration;
    if (command == "S" || command == "s")
      return CommandType.StitchSingleMajorDevice;
    if (command == "M" || command == "m")
      return CommandType.StitchMultiMajorDevice;
    return CommandType.Unknown;
  }

  // Set the calibration mode based on user input
  public static ProfilerCalibrationMode InputCalibType()
  {
    while (true)
    {
      Console.WriteLine("\nEnter the number that represents the calibration types:\n1: Wide\n2: Angle");
      int input = GetInputNumber<int>();
      switch (input)
      {
        case 1: return ProfilerCalibrationMode.Wide;
        case 2: return ProfilerCalibrationMode.Angle;
        default: Console.WriteLine("Invalid input! Please enter 1 or 2."); break;
      }
    }
  }

  // Set the target transformation axis based on user input
  public static MMind.Eye.TargetTranslateAxis InputTargetTransformAxis()
  {
    while (true)
    {
      Console.WriteLine("\nEnter the number that represents the transform axis:\n1: X\n2: Y");
      int input = GetInputNumber<int>();
      switch (input)
      {
        case 1: return MMind.Eye.TargetTranslateAxis.X;
        case 2: return MMind.Eye.TargetTranslateAxis.Y;
        default: Console.WriteLine("Invalid input! Please enter 1 or 2."); break;
      }
    }
  }

  // Set the cloud stitching option
  public static int InputCloudStitchOption()
  {
    while (true)
    {
      Console.WriteLine("\nEnter the number that represents the cloud stitching option:\n1: Refine stitching parameters and do overlap region uniform sampling\n2: Only do overlap region uniform sampling\n3: Do neither of the above processes");
      int input = GetInputNumber<int>();
      switch (input)
      {
        case 1:
          return (int)(ProfilerCalibrationInterfaces.CloudStitchOption.RefineStitchParameters) | (int)(ProfilerCalibrationInterfaces.CloudStitchOption.OverlapRegionUniformSampling);
        case 2:
          return (int)(ProfilerCalibrationInterfaces.CloudStitchOption.OverlapRegionUniformSampling);
        case 3:
          return 0;
        default:
          Console.WriteLine("Invalid input! Please enter 1, 2 or 3.");
          break;
      }
    }
  }

  // Print error messages when stitching fails
  public static void PrintError(MultiProfilerErrorStatus errorStatus)
  {
    if (errorStatus == null)
    {
      Console.WriteLine("\nerrorStatus: null");
      return;
    }
    Console.WriteLine($"\nerrorStatus: {errorStatus.ErrorCode}");
    Console.WriteLine($"\nerrorDescription: {errorStatus.MultiProfilerErrorDescription}");
    Console.WriteLine($"\nerrorSource: {errorStatus.ErrorSource} {errorStatus.GroupID}");
  }

  /* Discovers and connects multiple Mech-Eye 3D Laser Profilers for calibration.
   * Ensures that all connected profilers are of the same model for calibration.*/
  public static List<Profiler> FindAndConnectMultiProfilerForCalibration()
  {
    Console.WriteLine("Find Mech-Eye 3D Laser Profilers...\n");
    List<ProfilerInfo> profilerInfoList = Profiler.DiscoverProfilers();

    if (profilerInfoList == null || profilerInfoList.Count == 0)
    {
      Console.WriteLine("No Mech-Eye 3D Laser Profilers found.\n");
      return new List<Profiler>();
    }

    for (int i = 0; i < profilerInfoList.Count; i++)
    {
      Console.WriteLine($"Mech-Eye 3D Laser Profiler index :  {i}\n");
      MMind.Eye.Utils.PrintProfilerInfo(profilerInfoList[i]);
    }

    var indices = new List<int>();
    var uniqueIndices = new HashSet<int>();
    while (true)
    {
      if (indices.Count == 0)
      {
        Console.WriteLine("Please enter the device index you want to choose as major profiler: ");
      }
      else
      {
        Console.WriteLine("Please enter the device index you want to choose as minor profiler: ");
        Console.WriteLine("Enter the character 'c' to terminate adding devices");
      }

      string str = Console.ReadLine();
      if (str == "c" && indices.Count > 1)
        break;

      if (Regex.IsMatch(str, @"^\d+$") && int.Parse(str) < profilerInfoList.Count)
      {
        int index = int.Parse(str);

        if (indices.Count > 0 &&
            profilerInfoList[index].Model != profilerInfoList[indices[0]].Model)
        {
          Console.WriteLine("Input invalid. Please choose the same model device to connect: \n");
          continue;
        }
        if (uniqueIndices.Add(index))
        {
          indices.Add(index);
        }
      }
      else
      {
        Console.WriteLine("Input invalid. Please enter the device index you want to connect: ");
      }
    }

    // Connect to selected profilers
    var profilerList = new List<Profiler>();
    foreach (var index in indices)
    {
      Profiler profiler = new Profiler();
      ErrorStatus status = profiler.Connect(profilerInfoList[index]);
      if (status.IsOK())
        profilerList.Add(profiler);
      else
        MMind.Eye.Utils.ShowError(status);
    }
    return profilerList;
  }

  // Acquire profile data from a profiler
  public static bool AcquireProfileData(Profiler profiler, ref ProfileBatch totalBatch, int captureLineCount, int dataWidth, bool isSoftwareTrigger)
  {
    Console.WriteLine("Start data acquisition.");
    ErrorStatus status = profiler.StartAcquisition();
    if (!status.IsOK())
    {
      MMind.Eye.Utils.ShowError(status);
      return false;
    }

    if (isSoftwareTrigger)
    {
      status = profiler.TriggerSoftware();
      if (!status.IsOK())
      {
        MMind.Eye.Utils.ShowError(status);
        return false;
      }
    }

    totalBatch.Clear();
    totalBatch.Reserve((ulong)captureLineCount);
    while (totalBatch.Height() < (ulong)captureLineCount)
    {
      ProfileBatch batch = new ProfileBatch((ulong)dataWidth);
      status = profiler.RetrieveBatchData(ref batch);
      if (status.IsOK())
      {
        if (!totalBatch.Append(batch))
          break;
        Thread.Sleep(200);
      }
      else
      {
        MMind.Eye.Utils.ShowError(status);
        return false;
      }
    }

    Console.WriteLine("Stop data acquisition.");
    status = profiler.StopAcquisition();
    if (!status.IsOK())
      MMind.Eye.Utils.ShowError(status);
    return status.IsOK();
  }

  // Get device information and interact with the user for additional parameters
  public static DeviceInfo GetDeviceInfo(Profiler profiler)
  {
    UserSet userSet = profiler.CurrentUserSet();
    ProfilerInfo profilerInfo = new ProfilerInfo();
    ErrorStatus status = profiler.GetProfilerInfo(ref profilerInfo);
    if (!status.IsOK())
    {
      MMind.Eye.Utils.ShowError(status);
      return null;
    }
    MMind.Eye.Utils.PrintProfilerInfo(profilerInfo);

    // Get X xResolution
    double xResolution = 0;
    status = userSet.GetFloatValue(MMind.Eye.PointCloudResolutions.XAxisResolution.Name, ref xResolution);
    if (!status.IsOK())
    {
      MMind.Eye.Utils.ShowError(status);
      return null;
    }

    // Get Y Resolution
    double yResolution = 0;
    status = userSet.GetFloatValue(MMind.Eye.PointCloudResolutions.YResolution.Name, ref yResolution);
    if (!status.IsOK())
    {
      MMind.Eye.Utils.ShowError(status);
      return null;
    }

    // get Profile ROI
    ProfileROI roiValue = new ProfileROI();
    status = userSet.GetProfileRoiValue(MMind.Eye.ProfileROISettings.ROI.Name, ref roiValue);
    if (!status.IsOK())
    {
      MMind.Eye.Utils.ShowError(status);
      return null;
    }

    Console.WriteLine("\nEnter the downsampling interval in the X direction: ");
    uint downsampleX = GetInputNumber<uint>();

    Console.WriteLine("\nEnter the downsampling interval in the Y direction: ");
    uint downsampleY = GetInputNumber<uint>();

    Console.WriteLine("\nEnter the Camera motion direction: ");
    bool directionPositive = GetInputNumber<bool>();

    Console.WriteLine("Please confirm the following device settings:");
    Console.WriteLine("--------------------------------------------");
    Console.WriteLine($"1. X-Axis Resolution (um): {xResolution / 1000:F3}");
    Console.WriteLine($"2. Y-Axis Resolution (um): {yResolution / 1000:F3}");
    Console.WriteLine($"3. Downsampling Factor (X): {downsampleX}");
    Console.WriteLine($"4. Downsampling Factor (Y): {downsampleY}");
    Console.WriteLine($"5. Motion Direction Sign:  {directionPositive}");
    Console.WriteLine($"6. ROI Size (Width , Height): ({roiValue.Width}, {roiValue.Height})");
    Console.WriteLine($"7. ROI Center (X,Y): ({roiValue.XAxisCenter}, 0)");
    Console.WriteLine("--------------------------------------------");

    DeviceInfo deviceInfo = new DeviceInfo();
    deviceInfo.DX = (float)(xResolution / 1000);
    deviceInfo.DY = (float)(yResolution / 1000);
    deviceInfo.DownsampleX = downsampleX;
    deviceInfo.DownsampleY = downsampleY;
    deviceInfo.MoveDirSign = directionPositive;
    deviceInfo.ROISize = new Point2f { X = (float)roiValue.Width, Y = (float)roiValue.Height };
    deviceInfo.ROICenter = new Point2f { X = (float)roiValue.XAxisCenter, Y = 0 };
    return deviceInfo;
  }

  // Asynchronously capture images from a profiler
  public static ProfilerImage CaptureAsync(Profiler profiler)
  {
    ProfilerInfo profilerInfo = new ProfilerInfo();
    MMind.Eye.Utils.ShowError(profiler.GetProfilerInfo(ref profilerInfo));

    // Select "calib" user set to capture image.
    string calibSetting = "calib";
    UserSetManager userSetManager = profiler.UserSetManager();
    string successMessage = $"Set current set as the \"{calibSetting}\" user set.";
    MMind.Eye.Utils.ShowError(userSetManager.SelectUserSet(calibSetting), successMessage);

    UserSet userSet = profiler.CurrentUserSet();

    int dataWidth = 0;
    MMind.Eye.Utils.ShowError(userSet.GetIntValue(MMind.Eye.ScanSettings.DataPointsPerProfile.Name, ref dataWidth));
    int captureLineCount = 0;
    userSet.GetIntValue(MMind.Eye.ScanSettings.ScanLineCount.Name, ref captureLineCount);

    ProfileBatch profileBatch = new ProfileBatch((ulong)dataWidth);

    int dataAcquisitionTriggerSource = 0;
    MMind.Eye.Utils.ShowError(userSet.GetEnumValue(MMind.Eye.TriggerSettings.DataAcquisitionTriggerSource.Name, ref dataAcquisitionTriggerSource));

    //// Adjust the "Trigger Delay" appropriately to avoid interference between devices and ensure
    /// optimal imaging performance.
    // showError(userSet.setEnumValue(mmind::eye::trigger_settings::TriggerDelay::name, 100));
    bool isSoftwareTrigger = dataAcquisitionTriggerSource == (int)MMind.Eye.TriggerSettings.DataAcquisitionTriggerSource.Value.Software;

    if (!AcquireProfileData(profiler, ref profileBatch, captureLineCount, dataWidth, isSoftwareTrigger))
      return null;

    if (profileBatch.CheckFlag(ProfileBatch.BatchFlag.Incomplete))
      Console.WriteLine($"Part of the batch's data is lost, the number of valid profiles is: {profileBatch.ValidHeight()}.");

    ProfilerImage result = new ProfilerImage();
    ImageWrapper imgDepth = new ImageWrapper
    {
      Data = profileBatch.GetDepthMap().Data(),
      Rows = captureLineCount,
      Cols = dataWidth,
      Type = 5
    };
    ImageWrapper imgIntensity = new ImageWrapper
    {
      Data = profileBatch.GetIntensityImage().Data(),
      Rows = captureLineCount,
      Cols = dataWidth,
      Type = 0
    };
    result.Depth = imgDepth.Clone();
    result.Intensity = imgIntensity.Clone();
    return result;
  }

  // Convert a 3x4 matrix to a 4x4 homogeneous matrix
  public static float[,] Convert34To44(float[,] mat34)
  {
    if (mat34.GetLength(0) != 3 || mat34.GetLength(1) != 4)
      throw new ArgumentException("input matrix must 3x4");

    float[,] mat44 = new float[4, 4];

    for (int i = 0; i < 3; ++i)
    {
      for (int j = 0; j < 4; ++j)
      {
        mat44[i, j] = mat34[i, j];
      }
    }

    mat44[3, 0] = 0.0f;
    mat44[3, 1] = 0.0f;
    mat44[3, 2] = 0.0f;
    mat44[3, 3] = 1.0f;

    return mat44;
  }

  // Truncate a 4x4 homogeneous matrix to a 3x4 matrix (remove the last row)
  public static float[,] Convert44To34(float[,] mat44)
  {
    if (mat44.GetLength(0) != 4 || mat44.GetLength(1) != 4)
      throw new ArgumentException("input matrix must 4x4");

    float[,] mat34 = new float[3, 4];

    for (int i = 0; i < 3; ++i)
    {
      for (int j = 0; j < 4; ++j)
      {
        mat34[i, j] = mat44[i, j];
      }
    }

    return mat34;
  }

  // Capture images from each profiler in the list
  public static bool CaptureImagesForEachProfiler(List<Profiler> profilerList,
                                                out ProfilerImage majorImage,
                                                out List<ImageWrapper> minorDepths,
                                                out List<ProfilerImage> minorImages)
  {
    majorImage = null;
    minorDepths = new List<ImageWrapper>();
    minorImages = new List<ProfilerImage>();

    // Validate the profiler list
    if (profilerList == null || profilerList.Count == 0)
    {
      Console.WriteLine("No profilers connected.");
      return false;
    }

    // Confirm capture with the user
    if (!MMind.Eye.Utils.ConfirmCapture())
    {
      foreach (var profiler in profilerList)
        profiler.Disconnect();
      return false;
    }

    var container = new List<Task<ProfilerImage>>();

    // Start asynchronous capture for each profiler
    for (int i = 0; i < profilerList.Count; ++i)
    {
      int index = i;
      container.Add(Task.Run(() => CaptureAsync(profilerList[index])));
    }

    // Process the results
    try
    {
      for (int i = 0; i < container.Count; ++i)
      {
        ProfilerImage result = container[i].Result;
        if (i == 0)
        {
          majorImage = new ProfilerImage();

          // Use clone to ensure data independence
          majorImage.Depth = result.Depth.Clone();

          // Use clone to ensure data independence
          majorImage.Intensity = result.Intensity.Clone();
        }
        else
        {
          ImageWrapper minorDepth = new ImageWrapper();
          minorDepth = result.Depth.Clone();
          minorDepths.Add(minorDepth);
          minorImages.Add(result);
        }
        profilerList[i].Disconnect();
      }
    }
    catch (Exception e)
    {
      Console.WriteLine($"Capture failed: {e.Message}");
      return false;
    }

    return true;
  }



  public static float[,] ComputeTransformProduct(float[,] baseTransform, float[,] additionalTransform)
  {
    float[,] baseTransform44 = Convert34To44(baseTransform);
    float[,] additionalTransform44 = Convert34To44(additionalTransform);

    // Result = baseTransform * additionalTransform
    float[,] result44 = MultiplyMatrix(baseTransform44, additionalTransform44);

    //3x4 
    return Convert44To34(result44);
  }


  public static float[,] MultiplyMatrix(float[,] matrixA, float[,] matrixB)
  {
    int rowsA = matrixA.GetLength(0);
    int colsA = matrixA.GetLength(1);
    int rowsB = matrixB.GetLength(0);
    int colsB = matrixB.GetLength(1);

    if (colsA != rowsB)
      throw new ArgumentException("The dimensions of the matrices do not match, and thus the multiplication operation cannot be performed.");

    float[,] result = new float[rowsA, colsB];

    for (int i = 0; i < rowsA; i++)
    {
      for (int j = 0; j < colsB; j++)
      {
        float sum = 0;
        for (int k = 0; k < colsA; k++)
        {
          sum += matrixA[i, k] * matrixB[k, j];
        }
        result[i, j] = sum;
      }
    }

    return result;
  }

  //Load calibration properties from a file into the calibration interface instance.
  public static bool LoadCalibration(ProfilerCalibrationInterfaces calibInstance, string prompt)
  {
    Console.Write($"{prompt}");
    string folder = Console.ReadLine();
    if (string.IsNullOrEmpty(folder))
      return false;

    var errorStatus = calibInstance.LoadCalibProperties(folder, true);
    if (!errorStatus.IsOK())
    {
      Console.WriteLine($"Failed to load calibration file from {folder}");
      PrintError(errorStatus);
      return false;
    }

    Console.WriteLine($"Successful to load calibration file from {folder}");
    return true;
  }
  private static bool CaptureImages(List<Profiler> profilerList, out ProfilerImage majorImage,
                                      out List<ImageWrapper> minorDepths, out List<ProfilerImage> minorImages)
  {
    return CaptureImagesForEachProfiler(profilerList, out majorImage, out minorDepths, out minorImages);
  }
  private static void StitchAndFuseImages(ProfilerCalibrationInterfaces calibInstance,
                                             ProfilerImage majorImage, List<ProfilerImage> minorImages,
                                             List<CalibResult> calibResults, string filePath, ref bool ifExit)
  {

    // Perform image stitching for Z-parallel calibration
    var stitchStatus = calibInstance.StitchImagesForZParallel(majorImage, minorImages, calibResults, out MultiStitchResultZParallel stitchResults);
    if (!stitchStatus.IsOK())
    {
      PrintError(stitchStatus);
      ifExit = true;
      return;
    }

    // Perform cloud stitching
    var cloudStitchOption = InputCloudStitchOption();
    stitchStatus = calibInstance.StitchPointCloud(majorImage, minorImages, cloudStitchOption, (int)(ProfilerCalibrationInterfaces.StitchParamRefineOption.RefineT), calibResults);

    // Check if stitching was successful
    if (!stitchStatus.IsOK())
    {
      PrintError(stitchStatus);
      ifExit = true;
      return;
    }

    // Prompt whether to save calibration files if stitching parameters were refined
    if ((cloudStitchOption & (int)(ProfilerCalibrationInterfaces.CloudStitchOption.RefineStitchParameters)) != 0)
    {
      Console.WriteLine("Do you want to save the calibration files with refined stitching parameters? Enter 'y' to save the calibration files or any other key to continue without saving.");
      var option = Console.ReadLine();
      if (option == "y" && !calibInstance.SaveCalibFiles(false, filePath))
      {
        Console.WriteLine("Error: Failed to save calibration files! Please ensure the target folder exists at {}");
        ifExit = true;
        return;
      }
    }

    // Save the stitched files to the specified file path
    if (!calibInstance.SaveStitchFilesForZParallel(filePath))
    {
      Console.WriteLine($"Error: Failed to save Z-parallel stitching files! Please ensure the target folder exists at {filePath} and you have write permissions.");
      ifExit = true;
      return;
    }

    // Get and save stitched point cloud
    var errorStatus = calibInstance.GetStitchedPointCloud(out ProfileTexturedPointCloud pointCloud);
    if (!errorStatus.IsOK())
    {
      PrintError(stitchStatus);
      ifExit = true;
      return;
    }
    ProfileBatch.SaveTexturedPointCloud(ref pointCloud, FileFormat.PLY, filePath + "/StitchedPointCloud");

    Console.WriteLine($"Stitch completed successfully. Stitched files saved in: {filePath}");
    Console.WriteLine("Save process finished.");

    // Prompt the user to continue with image fusion or end the calibration
    Console.WriteLine("\nImage fusion can only be performed effectively if the profilers' z-axis are " +
             "parallel and they are not in a bi-directional (opposing) setup. " +
             "Would you like to proceed with image fusion? Enter 'y' to continue or any other " +
             "key to end the calibration."
          );
    string key = Console.ReadLine();

    // Check if the user wants to proceed with image fusion
    if (key != "y")
    {
      ifExit = true;
      return;
    }

    FusionResult fusionResult = new FusionResult();
    List<bool> fusionFlags = new List<bool>();

    // Perform image fusion for Z-parallel calibration
    var fusionStatus = calibInstance.ImageFusionForZParallel(out fusionResult, fusionFlags);

    /* In the "handleStitchMultiMajorDevice" function, consider the following scenario:
     * - Profiler 0 and Profiler 1 are placed side-by-side,
     * - Profiler 2 and Profiler 3 are also placed side-by-side,
     * - Profiler 0 and Profiler 2 are positioned opposite each other.
     * In this case, only the images from Profiler 0 and Profiler 1, as well as Profiler 2 and
     * Profiler 3, can be fused. If all profilers are arranged side-by-side or in opposite
     * positions, then all images can be fused.
     */
    //// Fuse images of profiler 2 and 3
    // auto fusionStatus = calibInstance.imageFusionForZParallel(
    //     stitchResults.minorStitchResults[1].stitchResultImage,
    //     { stitchResults.minorStitchResults[2] }, stitchResults.minorStitchResults[1].bias,
    //     fusionResult);

    // Check if fusion was successful
    if (!fusionStatus.IsOK())
    {
      PrintError(fusionStatus);
      ifExit = true;
      return;
    }

    // Save the fused depth image to the specified file path
    Console.WriteLine("Fusion Success.");
    string fusionFilePath = filePath + "/fusionResult.tiff";
    ProfilerImage outimg = fusionResult.CombinedImage;
    using (Mat mat = new Mat(outimg.Depth.Rows, outimg.Depth.Cols, (DepthType)outimg.Depth.Type, ((outimg.Depth.Type >> 3) & 0xF) + 1, outimg.Depth.Data, 0))
    {
      CvInvoke.Imwrite(fusionFilePath, mat);
    }
    ifExit = false;
  }

  public static void HandleCalibration(ref bool ifExit)
  {

    // Set the target size
    var targetSize = new TargetSize();
    bool continueProcess = false;

    while (!continueProcess)
    {
      Console.WriteLine("\nEnter the target top length: ");
      targetSize.TopLength = GetInputNumber<float>();

      Console.WriteLine("\nEnter the target bottom length: ");
      targetSize.BottomLength = GetInputNumber<float>();

      Console.WriteLine("\nEnter the target height: ");
      targetSize.Height = GetInputNumber<float>();

      // Show the input values for confirmation
      Console.WriteLine("\nYou entered the following values:");
      Console.WriteLine($"Top Length:  {targetSize.TopLength}");
      Console.WriteLine($"Bottom Length: {targetSize.BottomLength}");
      Console.WriteLine($"Height:  {targetSize.Height}");

      // Ask user if they want to continue or reinput
      Console.Write("\nContinue with the process? (y to continue, any other key to reinput): ");
      string choice = Console.ReadLine();
      continueProcess = (choice?.ToLower() == "y");
    }

    // Choose the device for calibration
    var profilerList = FindAndConnectMultiProfilerForCalibration();

    if (profilerList.Count == 0)
    {
      Console.WriteLine("No profilers connected.");
      ifExit = true;
      return;
    }

    // Get the major device Info by retrieving user input
    Console.WriteLine("\nGet the major deviceInfo");
    var majorDeviceInfo = GetDeviceInfo(profilerList[0]);
    var minorDeviceInfos = new List<DeviceInfo>();

    // Get the minor device Infos by retrieving user input
    for (int i = 1; i < profilerList.Count; i++)
    {
      Console.WriteLine($"\nGet the minor deviceInfo {i}");
      var deviceInfo = GetDeviceInfo(profilerList[i]);
      minorDeviceInfos.Add(deviceInfo);
    }

    // Set the target poses
    var targetPoses = new List<TargetPose>();
    for (int i = 0; i < minorDeviceInfos.Count; i++)
    {
      var targetPose = new TargetPose();
      bool continuePoseProcess = false;

      while (!continuePoseProcess)
      {
        Console.WriteLine($"\nEnter the pose between targets major and minor-{i}");
        Console.WriteLine("\nEnter the distance between targets: ");
        targetPose.TranslateDistance = GetInputNumber<float>();

        Console.WriteLine("\nEnter the rotation angle between targets:");
        targetPose.RotateAngleInDegree = GetInputNumber<float>();

        Console.WriteLine("\nEnter the rotation radius between targets: ");
        targetPose.RotateRadius = GetInputNumber<float>();

        // Show the input values for confirmation
        Console.WriteLine("\nYou entered the following values:");
        Console.WriteLine($"Distance:  {targetPose.TranslateDistance}");
        Console.WriteLine($"Rotation Angle: {targetPose.RotateAngleInDegree}");
        Console.WriteLine($"Rotation Radius:  {targetPose.RotateRadius}");

        // Ask user if they want to continue or reinput
        Console.Write("\nContinue with the process? (y to continue, any other key to reinput):");
        string choice = Console.ReadLine();
        continuePoseProcess = (choice?.ToLower() == "y");
      }

      // Set the calib mode
      var calibMode = InputCalibType();

      // Set the transform axis
      var transformAxis = InputTargetTransformAxis();

      // Set the translation/rotation axis parameters based on the calibration mode type
      targetPose.Mode = calibMode;

      switch (calibMode)
      {
        case ProfilerCalibrationMode.Wide:
          targetPose.TranslateAxis = (TargetTranslateAxis)((int)transformAxis);
          targetPose.RotateAxis = TargetRotateAxis.NullAxis;
          break;

        case ProfilerCalibrationMode.Angle:
          targetPose.RotateAxis = (TargetRotateAxis)((int)transformAxis);
          targetPose.TranslateAxis = TargetTranslateAxis.NullAxis;
          break;
      }
      targetPoses.Add(targetPose);
    }

    Console.WriteLine("\n++++++++++++++++++++++Start calibration+++++++++++++++++++++++++");

    // Get the major profiler info
    var majorProfilerInfo = new ProfilerInfo();
    var status = profilerList[0].GetProfilerInfo(ref majorProfilerInfo);
    if (!status.IsOK())
    {
      MMind.Eye.Utils.ShowError(status);
      ifExit = true;
      return;
    }

    // Start to calibrate.
    ProfilerImage majorImage;
    List<ImageWrapper> minorDepths;
    List<ProfilerImage> minorImages;


    if (!CaptureImages(profilerList, out majorImage, out minorDepths, out minorImages))
    {
      ifExit = true;
      return;
    }

    var calibInstance = new ProfilerCalibrationInterfaces();


    calibInstance.SetCalibCameraModel(majorProfilerInfo.DeviceName);


    calibInstance.SetMajorDeviceInfo(majorDeviceInfo);


    calibInstance.SetMinorDeviceInfos(minorDeviceInfos);


    calibInstance.SetCalibTargetSize(targetSize);


    calibInstance.SetCalibTargetPoses(targetPoses);


    // Create ImageWrapper for the depth image of the main device.
    var majorDepthWrapper = majorImage.Depth;

    var errorStatus = calibInstance.CalculateCalibration(majorDepthWrapper, minorDepths,
                                                   out List<CalibResult> calibResults);

    if (!errorStatus.IsOK())
    {
      PrintError(errorStatus);
      ifExit = true;
      Console.ReadKey();
      return;
    }

    Console.WriteLine("\nCalibration completed successfully.");

    // saveCalibFiles
    string filePath = null;
    while (true)
    {
      Console.Write("Enter the path to save the calibration files (please use English characters for " +
                   "the path): ");

      filePath = Console.ReadLine();

      if (string.Equals(filePath, "q", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(filePath, "exit", StringComparison.OrdinalIgnoreCase))
      {
        ifExit = true;
        Console.WriteLine("The user has exited the program.");
        return;
      }

      if (string.IsNullOrEmpty(filePath))
      {
        if (filePath == null)
        {
          Console.WriteLine("The filePath is not set.");
        }
        else if (filePath.Length == 0)
        {
          Console.WriteLine("The filePath is empty.");
        }
        continue;
      }
      break;
    }

    bool saveSuccess = calibInstance.SaveCalibFiles(true, filePath);
    if (!saveSuccess)
    {
      Console.WriteLine($"Error: Failed to save calibration files! Please ensure the target folder exists at {filePath} and you have write permissions.");
      ifExit = true;
      return;
    }

    Console.WriteLine($"Files saved successfully. Path: {filePath}");

    // Proceed with image stitching
    Console.WriteLine("\nWould you like to proceed with image stitching? Enter 'y' to continue or any other key to end the calibration.");
    string key = Console.ReadLine();
    if (key?.ToLower() == "y")
    {
      StitchAndFuseImages(calibInstance, majorImage, minorImages, calibResults, filePath, ref ifExit);
    }
    ifExit = false;
  }

  public static void HandleStitchSingleMajorDevice(ref bool ifExit)
  {

    // Choose the device for stitching.
    var profilerList = FindAndConnectMultiProfilerForCalibration();


    ProfilerImage majorImage;
    List<ImageWrapper> minorDepths;
    List<ProfilerImage> minorImages;

    if (!CaptureImages(profilerList, out majorImage, out minorDepths, out minorImages))
    {
      ifExit = true;
      return;
    }

    var calibInstance = new ProfilerCalibrationInterfaces();

    Console.Write("Enter the calibration file path (please use English characters for the path): ");
    string filePath = Console.ReadLine();
    if (string.IsNullOrEmpty(filePath))
    {
      if (filePath == null)
      {
        Console.WriteLine("The filePath is not set.");
      }
      else if (filePath.Length == 0)
      {
        Console.WriteLine("The filePath is empty.");
      }
      ifExit = true;
      return;
    }

    var loadErrorStatus = calibInstance.LoadCalibProperties(filePath, ifExit);

    // Check if calibration properties were loaded successfully
    if (!loadErrorStatus.IsOK())
    {
      PrintError(loadErrorStatus);
      return;
    }

    // Retrieve current calibration results
    var calibResults = calibInstance.GetCurrentCalibResults();

    if (!string.IsNullOrEmpty(filePath))
    {
      StitchAndFuseImages(calibInstance, majorImage, minorImages, calibResults, filePath, ref ifExit);
    }

  }

  public static void HandleStitchMultiMajorDevice(ref bool ifExit)
  {


    var calibInstance01 = new ProfilerCalibrationInterfaces();
    var calibInstance02 = new ProfilerCalibrationInterfaces();
    var calibInstance23 = new ProfilerCalibrationInterfaces();


    var success01 = LoadCalibration(
    calibInstance01, "Enter the calibration file path (camera 0 and camera 1): ");
    var success02 = LoadCalibration(
calibInstance02, "Enter the calibration file path (camera 0 and camera 2): ");
    var success23 = LoadCalibration(
calibInstance23, "Enter the calibration file path (camera 2 and camera 3): ");
    if (success01 && success02 && success23)
    {
      Console.WriteLine("All calibration files loaded successfully!");
    }
    else
    {
      if (!success01)
        Console.WriteLine("Failed to load calibration file for camera 0 and camera 1.");
      if (!success02)
        Console.WriteLine("Failed to load calibration file for camera 0 and camera 2.");
      if (!success23)
        Console.WriteLine("Failed to load calibration file for camera 2 and camera 3.");
      Console.WriteLine("Please check the file paths and try again.");
      ifExit = true;
      return;
    }

    CalibResult calibResults01 = calibInstance01.GetCurrentCalibResults()[0];
    CalibResult calibResults02 = calibInstance02.GetCurrentCalibResults()[0];
    CalibResult calibResults23 = calibInstance23.GetCurrentCalibResults()[0];

    var calibResults = new List<CalibResult>();
    calibResults.Add(calibResults01);
    calibResults.Add(calibResults02);
    CalibResult calibResults03 = calibResults23;

    // The transformation matrix between profiler 2 and 3 needs to be converted to the
    // transformation matrix between 0 and 3
    CalibResultParams outParams03 = new CalibResultParams();
    outParams03.MinorMoveDirVec = calibResults23.Params.MinorMoveDirVec;
    outParams03.MajorMoveDirVec = calibResults01.Params.MajorMoveDirVec;
    outParams03.MatrixRT = ComputeTransformProduct(calibResults02.Params.MatrixRT,
                                                               calibResults23.Params.MatrixRT);
    calibResults03.Params = outParams03;
    // calibResults23 transform to calibResults03
    calibResults.Add(calibResults03);

    // Collect minor device information and target poses from calibration instances

    var minorDeviceInfos = new List<DeviceInfo>();
    var targetPoses = new List<TargetPose>();
    minorDeviceInfos.Add(calibInstance01.GetMinorDeviceInfos()[0]);
    minorDeviceInfos.Add(calibInstance02.GetMinorDeviceInfos()[0]);
    minorDeviceInfos.Add(calibInstance23.GetMinorDeviceInfos()[0]);

    targetPoses.Add(calibInstance01.GetTargetPoses()[0]);
    targetPoses.Add(calibInstance02.GetTargetPoses()[0]);
    targetPoses.Add(calibInstance23.GetTargetPoses()[0]);

    // Initialize the Calibration instance using the new constructor to reduce code volume
    // Parameters in order:
    // 1. calibInstance01.getCameraModel(): Set the camera model, same as profiler 0
    // 2. calibInstance01.getMajorDeviceInfo(): Set the major device info, same as profiler 0
    // 3. minorDeviceInfos: Set the minor device infos, containing profiler 1, 2, 3
    // 4. calibInstance01.getTargetSize(): Set the calibration target size, same as profiler 0
    // 5. targetPoses: Set the poses of the calibration target
    var calibInstance = new ProfilerCalibrationInterfaces();


    calibInstance.SetCalibCameraModel(calibInstance01.GetCameraModel());


    calibInstance.SetMajorDeviceInfo(calibInstance01.GetMajorDeviceInfo());


    calibInstance.SetMinorDeviceInfos(minorDeviceInfos);


    calibInstance.SetCalibTargetSize(calibInstance01.GetTargetSize());


    calibInstance.SetCalibTargetPoses(calibInstance01.GetTargetPoses());

    // Choose the device for stitching.
    var profilerList = FindAndConnectMultiProfilerForCalibration();


    ProfilerImage majorImage;
    List<ImageWrapper> minorDepths;
    List<ProfilerImage> minorImages;

    if (!CaptureImages(profilerList, out majorImage, out minorDepths, out minorImages))
    {
      ifExit = true;
      return;
    }
    Console.Write("Enter the path you want to save the stitch result (please use English characters for the path): ");
    string filePath = Console.ReadLine();

    if (!string.IsNullOrEmpty(filePath))
    {
      StitchAndFuseImages(calibInstance, majorImage, minorImages, calibResults, filePath, ref ifExit);
    }

  }

  static void Main()
  {

    bool ifExit = false;
    while (!ifExit)
    {
      try
      {
        var command = EnterCommand();
        switch (command)
        {
          case CommandType.Unknown:
            ifExit = true;
            break;
          case CommandType.Calibration:
            HandleCalibration(ref ifExit);
            break;
          case CommandType.StitchSingleMajorDevice:
            HandleStitchSingleMajorDevice(ref ifExit);
            break;
          case CommandType.StitchMultiMajorDevice:
            HandleStitchMultiMajorDevice(ref ifExit);
            break;
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Exception: {ex.Message}");
        Console.WriteLine("Do you want to exit? Enter '0' to quit or any other key to continue : ");
        if (!GetInputBool())
          ifExit = true;
      }
    }
  }

}

