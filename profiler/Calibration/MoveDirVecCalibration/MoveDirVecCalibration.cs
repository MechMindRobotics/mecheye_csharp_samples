/*
With this sample, you can complete the calibration of the movement direction vector for a single laser profiler, obtain the calibrated movement direction vector, the corresponding reprojection error, and correction parameters for downstream processing (including depth maps and pixel offsets obtained from direction correction). The sample aims for engineering reproducibility and result acceptability, demonstrating a complete closed-loop process from acquisition to correction and back to the camera coordinate system.
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



class MoveDirVecCalibration
{
  private const int CV_8UC1 = 0;
  private const int CV_32FC1 = 5;

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

  // Set the calibration mode based on user input
  public static int InputMovementAxis()
  {
    while (true)
    {
      Console.WriteLine("\nEnter the number that represents the movement axis:\n1: X\n2: Y\n3: Z");
      int input = GetInputNumber<int>();
      return input;
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
    Console.WriteLine($"1. X-Axis Resolution (mm): {xResolution / 1000:F3}");
    Console.WriteLine($"2. Y-Axis Resolution (mm): {yResolution / 1000:F3}");
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

    var depthMap = profileBatch.GetDepthMap();
    var intensityImage = profileBatch.GetIntensityImage();

    ProfilerImage result = new ProfilerImage();
    ImageWrapper imgDepth = new ImageWrapper
    {
      Data = depthMap.Data(),
      Rows = captureLineCount,
      Cols = dataWidth,
      Type = 5
    };
    ImageWrapper imgIntensity = new ImageWrapper
    {
      Data = intensityImage.Data(),
      Rows = captureLineCount,
      Cols = dataWidth,
      Type = 0
    };
    result.Depth = imgDepth.Clone();
    result.Intensity = imgIntensity.Clone();

    GC.KeepAlive(depthMap);
    GC.KeepAlive(intensityImage);
    return result;
  }

  /* Discovers and connects multiple Mech-Eye 3D Laser Profilers for calibration.
  * Ensures that all connected profilers are of the same model for calibration.*/
  public static Profiler FindAndConnectProfilerForCalibration()
  {
    Console.WriteLine("Find Mech-Eye 3D Laser Profilers...\n");
    List<ProfilerInfo> profilerInfoList = Profiler.DiscoverProfilers();

    if (profilerInfoList == null || profilerInfoList.Count == 0)
    {
      Console.WriteLine("No Mech-Eye 3D Laser Profilers found.\n");
      return new Profiler();
    }

    for (int i = 0; i < profilerInfoList.Count; i++)
    {
      Console.WriteLine($"Mech-Eye 3D Laser Profiler index :  {i}\n");
      MMind.Eye.Utils.PrintProfilerInfo(profilerInfoList[i]);
    }

    string str = Console.ReadLine();
    int index = 0;
    Console.WriteLine("Please enter the device index you want to choose as major profiler: ");

    if (Regex.IsMatch(str, @"^\d+$") && int.Parse(str) < profilerInfoList.Count)
    {
      index = int.Parse(str);
    }
    else
    {
      Console.WriteLine("Input invalid. Please enter the device index you want to connect (Automatically connect to Profiler 0.): ");
    }

    // Connect to selected profilers
    var profiler = new Profiler();

    ErrorStatus status = profiler.Connect(profilerInfoList[index]);
    if (!status.IsOK())
      MMind.Eye.Utils.ShowError(status);
    return profiler;
  }

  public static Mat WrapImage(ImageWrapper img)
  {
    DepthType depthType;
    int channels = 1;
    int elemSize;

    switch (img.Type)
    {
      case 0: // CV_8UC1
        depthType = DepthType.Cv8U;
        elemSize = 1;
        break;

      case 5: // CV_32FC1
        depthType = DepthType.Cv32F;
        elemSize = 4;
        break;

      default:
        throw new NotSupportedException(
            $"Unsupported image type: {img.Type}");
    }

    return new Mat(
        img.Rows,
        img.Cols,
        depthType,
        channels,
        img.Data,
        img.Cols * elemSize
    );
  }

  private static bool CaptureImages(Profiler profilerOpt, out List<ProfilerImage> profilerImages)
  {
    profilerImages = new List<ProfilerImage>();
    while (true)
    {
      // Confirm capture with the user
      if (!MMind.Eye.Utils.ConfirmCapture())
      {
        profilerOpt.Disconnect();
        break;
      }
      else
        profilerImages.Add(CaptureAsync(profilerOpt));
    }
    return true;
  }

  static int Main()
  {

    // Set the target size
    var targetSize = new TargetSize();
    while (true)
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
      string confirm = Console.ReadLine();
      if (confirm == "y" || confirm == "Y")
      {
        break;
      }
    }

    // Choose the device for calibration
    var profilerOpt = FindAndConnectProfilerForCalibration();

    if (profilerOpt == null)
    {
      Console.WriteLine("No profilers connected.");
      return -1;
    }

    // Get the major device Info by retrieving user input
    Console.WriteLine("\nGet the major deviceInfo");
    var majorDeviceInfo = GetDeviceInfo(profilerOpt);

    // Get the major profiler info
    var majorProfilerInfo = new ProfilerInfo();
    var status = profilerOpt.GetProfilerInfo(ref majorProfilerInfo);
    if (!status.IsOK())
    {
      MMind.Eye.Utils.ShowError(status);
      return -1;
    }

    //Capture images for movement direction calibration
    var profilerImages = new List<ProfilerImage>();
    // Start to capture images for calibration
    if (!CaptureImages(profilerOpt, out profilerImages) || profilerImages.Count() < 2)
    {
      Console.WriteLine("Failed to capture sufficient images for calibration.\n;");
      return -1;
    }

    // Configure calibration parameters
    var targetPoses = new List<TargetPose>();
    var targetPose = new TargetPose();
    bool continuePoseProcess = false;

    while (!continuePoseProcess)
    {
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

    int moveAxis = InputMovementAxis();

    Console.WriteLine("\n++++++++++++++++++++++Starting movement direction calibration+++++++++++++++++++++++++");

    var calibInstance = new ProfilerCalibrationInterfaces();

    calibInstance.SetCalibCameraModel(majorProfilerInfo.DeviceName);

    calibInstance.SetMajorDeviceInfo(majorDeviceInfo);

    calibInstance.SetCalibTargetSize(targetSize);

    calibInstance.SetCalibTargetPoses(targetPoses);

    var errorStatus = calibInstance.CalibrateSingleProfilerMoveDirection(profilerImages[0].Depth, profilerImages[1].Depth, moveAxis, out MoveDirCalibResult calibResult, default, false);

    if (!errorStatus.IsOK())
    {
      PrintError(errorStatus);
      return -1;
    }

    Console.WriteLine("\nCalibration completed successfully.");
    Console.WriteLine($"Movement direct vector is : {{ {calibResult.MoveVec.x}, {calibResult.MoveVec.y}, {calibResult.MoveVec.z} }}");

    Point3f moveVec = new Point3f
    {
      X = calibResult.MoveVec.x,
      Y = calibResult.MoveVec.y,
      Z = calibResult.MoveVec.z
    };

    errorStatus = calibInstance.CorrectProfilerMoveDir(moveVec, profilerImages[0], out TransformedImageInfo correctedResult, default, false);
    if (!errorStatus.IsOK())
    {
      PrintError(errorStatus);
      return -1;
    }

    Console.WriteLine("\nDirection correction completed.");
    Console.WriteLine($"Pixel offset to camera origin: [ {{ {correctedResult.PixelBias.x}, {correctedResult.PixelBias.y} }}(pixels)");


    double dx = -(double)correctedResult.PixelBias.x;
    double dy = -(double)correctedResult.PixelBias.y;

    Mat translationMat = new Mat(2, 3, DepthType.Cv64F, 1);
    translationMat.SetTo(new double[]
    {
            1.0, 0.0, dx,
            0.0, 1.0, dy
    });

    Mat alignedDepth = new Mat();
    ImageWrapper depthWrapper = correctedResult.GetDepth();

    Mat depthMat = new Mat(
        depthWrapper.Rows,
        depthWrapper.Cols,
        DepthType.Cv32F,
        1,
        depthWrapper.Data,
        depthWrapper.Cols * sizeof(float)
    );

    System.Drawing.Size outSize =
        new System.Drawing.Size(depthWrapper.Cols, depthWrapper.Rows);

    CvInvoke.WarpAffine(
        depthMat,                  // IInputArray 
        alignedDepth,
        translationMat,
        outSize,                   // System.Drawing.Size 
        Inter.Nearest,
        Warp.Default,
        BorderType.Constant,
        new Emgu.CV.Structure.MCvScalar(float.NaN)
    );

    //Save results for verification
    using (Mat mat = WrapImage(profilerImages[0].Depth))
    {
      CvInvoke.Imwrite("01_raw_depth.tiff", mat);
    }
    using (Mat mat = WrapImage(depthWrapper))
    {
      CvInvoke.Imwrite("02_corrected_depth_with_offset.tiff", mat);
    }

    CvInvoke.Imwrite("03_aligned_depth_for_stitching.tiff", alignedDepth);

    Console.WriteLine(@"
                Demo completed successfully.
                Saved results:
                 - 01_raw_depth.tiff
                 - 02_corrected_depth_with_offset.tiff
                 - 03_aligned_depth_for_stitching.tiff
                ");

    return 0;

  }

}

