/*
With this sample, you can perform hand-eye calibration and obtain the extrinsic parameters.
This document contains instructions for building the sample program and using the sample program to
complete hand-eye calibration.
*/

using System;
using System.IO;
using System.Globalization;
using Emgu.CV;
using Emgu.CV.CvEnum;
using MMind.Eye;


public class HandEyeCalibrationSample
{
    private Camera camera = new Camera();
    private HandEyeCalibration calibration = new HandEyeCalibration();
    private HandEyeCalibration.CameraMountingMode mountingMode;
    private HandEyeCalibration.CalibrationBoardModel boardModel;
    private int eulerTypeCode;
    private int poseIndex;
    private double poseX, poseY, poseZ, poseR1, poseR2, poseR3;

    static int Main()
    {
        var sample = new HandEyeCalibrationSample();
        if (!Utils.FindAndConnect(ref sample.camera))
        {
            Console.WriteLine("Failed to connect to the camera.");
            Console.WriteLine("Press any key to exit ...");
            Console.ReadKey();
            return -1;
        }

        sample.InputCalibType();
        sample.InputBoardType();

        var errorStatus = sample.calibration.InitializeCalibration(ref sample.camera, sample.mountingMode, sample.boardModel);
        Utils.ShowError(errorStatus);

        if (!errorStatus.IsOK())
        {
            Console.WriteLine("Failed to initialize calibration.");
            Console.WriteLine("Press any key to exit ...");
            Console.ReadKey();
            return -1;
        }

        sample.InputEulerType();
        sample.Calibrate();
        sample.camera.Disconnect();
        Console.WriteLine("Disconnected from the camera successfully.");
        Console.WriteLine("Press any key to exit ...");
        Console.ReadKey();
        return 0;
    }

    private void InputCalibType()
    {
        Console.WriteLine("\nEnter the number that represents the camera mounting method:");
        Console.WriteLine("1: Eye-in-hand");
        Console.WriteLine("2: Eye-to-hand");
        int input = GetInputInt(1, 2, "Invalid input. Please enter 1 or 2.");

        mountingMode = (HandEyeCalibration.CameraMountingMode)(input - 1);
    }

    private void InputBoardType()
    {
        Console.WriteLine("\nEnter the number that represent the model of your calibration board (the model is labeled on the calibration board)");
        Console.WriteLine("1: BDB-5\n2:BDB-6\n3:BDB-7");
        Console.WriteLine("4: CGB-020\n5: CGB-035\n6: CGB-050");
        Console.WriteLine("7: OCB-005\n8: OCB-010\n9: OCB-015\n10: OCB-020");
        int input = GetInputInt(1, 10, "Invalid input. Please enter a number between 1 and 10.");
        switch (input)
        {
            case 1:
                boardModel = HandEyeCalibration.CalibrationBoardModel.BDB_5;
                break;
            case 2:
                boardModel = HandEyeCalibration.CalibrationBoardModel.BDB_6;
                break;
            case 3:
                boardModel = HandEyeCalibration.CalibrationBoardModel.BDB_7;
                break;
            case 4:
                boardModel = HandEyeCalibration.CalibrationBoardModel.CGB_20;
                break;
            case 5:
                boardModel = HandEyeCalibration.CalibrationBoardModel.CGB_35;
                break;
            case 6:
                boardModel = HandEyeCalibration.CalibrationBoardModel.CGB_50;
                break;
            case 7:
                boardModel = HandEyeCalibration.CalibrationBoardModel.OCB_5;
                break;
            case 8:
                boardModel = HandEyeCalibration.CalibrationBoardModel.OCB_10;
                break;
            case 9:
                boardModel = HandEyeCalibration.CalibrationBoardModel.OCB_15;
                break;
            case 10:
                boardModel = HandEyeCalibration.CalibrationBoardModel.OCB_20;
                break;
        }
    }

    private void InputEulerType()
    {
        Console.WriteLine("\nEnter the Euler angle convention of your robot:");
        Console.WriteLine("1: Z-Y'-X'' (intrinsic rotations)");
        Console.WriteLine("2: Z-Y'-Z''");
        Console.WriteLine("3: X-Y'-Z''");
        Console.WriteLine("4: Z-X'-Z''");
        Console.WriteLine("5: X-Y-Z (extrinsic rotations)");
        eulerTypeCode = GetInputInt(1, 5, "Invalid input. Please enter a number between 1 and 5.");
    }

    private int GetInputInt(int min, int max, string warningMessage)
    {
        while (true)
        {
            string input = Console.ReadLine();
            if (int.TryParse(input, out int value) && value >= min && value <= max)
                return value;

            Console.WriteLine(warningMessage);
        }
    }

    private double GetInputDouble(string prompt)
    {
        while (true)
        {
            Console.WriteLine(prompt);
            string input = Console.ReadLine();
            if (double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
                return value;

            Console.WriteLine("Invalid input. Please enter a valid number.");
        }
    }

    private void ShowAndSaveImage(Color2DImage colorImage, string fileName, string windowName)
    {
        if (colorImage.IsEmpty())
            return;
        var colorMat = new Mat(unchecked((int)colorImage.Height()), unchecked((int)colorImage.Width()), DepthType.Cv8U, 3, colorImage.Data(), unchecked((int)colorImage.Width()) * 3);
        //CvInvoke.NamedWindow(windowName);
        //CvInvoke.Imshow(windowName, colorMat);
        //Console.WriteLine("Press any key to close the image");
        //CvInvoke.WaitKey();
        //CvInvoke.DestroyAllWindows();
        CvInvoke.Imwrite(fileName, colorMat);
    }

    private void ShowAndSaveImage(DepthMap depthMap, string fileName, string windowName)
    {
        if (depthMap.IsEmpty())
            return;
        var depthMat = new Mat(unchecked((int)depthMap.Height()), unchecked((int)depthMap.Width()), DepthType.Cv32F, 1, depthMap.Data(), unchecked((int)depthMap.Width()) * 4);
        //CvInvoke.NamedWindow(windowName);
        //CvInvoke.Imshow(windowName, depthMat);
        //Console.WriteLine("Press any key to close the image");
        //CvInvoke.WaitKey();
        //CvInvoke.DestroyAllWindows();
        CvInvoke.Imwrite(fileName, depthMat);
    }

    private void Calibrate()
    {
        Console.WriteLine("\nExtrinsic parameter calculation requires at least 15 robot poses.");
        Console.WriteLine("Ensure you enter sufficient robot poses during calibration.");
        poseIndex = 1;
        bool calibrate = false;

        while (!calibrate)
        {
            string command = InputCommand();
            switch (command.ToUpper())
            {
                case "P":
                    var frame2D = new Frame2D();
                    Utils.ShowError(camera.Capture2D(ref frame2D));
                    ShowAndSaveImage(frame2D.GetColorImage(), "Original2DImage_" + poseIndex + ".png", "Original 2D Image");
                    break;
                case "T":
                    var frame3D = new Frame3D();
                    Utils.ShowError(camera.Capture3D(ref frame3D));
                    ShowAndSaveImage(frame3D.GetDepthMap(), "DepthMap_" + poseIndex + ".tiff", "Depth");
                    var testImage = new Color2DImage();
                    Utils.ShowError(calibration.TestRecognition(ref camera, ref testImage));
                    ShowAndSaveImage(testImage, "FeatureRecognitionResultForTest_" + poseIndex + ".png", "Feature Recognition Result For Test");
                    break;
                case "A":
                    var robotPose = InputRobotPose();
                    var colorImage = new Color2DImage();
                    var errorStatus = calibration.AddPoseAndDetect(ref camera, in robotPose, ref colorImage);
                    Utils.ShowError(errorStatus);
                    ShowAndSaveImage(colorImage, "FeatureRecognitionResult_" + poseIndex + ".png", "Feature Recognition Result");
                    if (errorStatus.IsOK())
                        poseIndex++;
                    break;
                case "C":
                    calibrate = true;
                    var cameraToBase = new HandEyeCalibration.Transformation();
                    var calibrationStatus = calibration.CalculateExtrinsics(ref camera, ref cameraToBase);
                    Utils.ShowError(calibrationStatus);

                    if (calibrationStatus.IsOK())
                    {
                        Console.WriteLine("The extrinsic parameters are:");
                        Console.WriteLine(cameraToBase.ToString());
                        SaveExtrinsicParameters(cameraToBase.ToString());
                    }
                    break;
                default:
                    Console.WriteLine("Unknown command.");
                    break;
            }
        }
    }

    private string InputCommand()
    {
        Console.WriteLine("\nEnter the action you want to perform:");
        Console.WriteLine("P: Obtain the original 2D image");
        Console.WriteLine("T: Obtain the 2D image with feature recognition result");
        Console.WriteLine("A: Enter the current robot pose");
        Console.WriteLine("C: Calculate extrinsic parameters");
        return Console.ReadLine();
    }

    private HandEyeCalibration.Transformation InputRobotPose()
    {
        Console.WriteLine("\nEnter the robot pose in the following format:");
        while (true)
        {
            poseX = GetInputDouble("X translational component (in mm): ");
            poseY = GetInputDouble("Y translational component (in mm): ");
            poseZ = GetInputDouble("Z translational component (in mm): ");
            poseR1 = GetInputDouble("Rotation component 1 (in degrees): ");
            poseR2 = GetInputDouble("Rotation component 2 (in degrees): ");
            poseR3 = GetInputDouble("Rotation component 3 (in degrees): ");

            var robotPose = EulerToQuaternion();
            Console.WriteLine("The current pose index is {0}", poseIndex);
            Console.WriteLine("If the above pose is correct, enter y; otherwise, press any key to enter the pose again.");
            var userInput = Console.ReadLine();
            if (userInput == "y" || userInput == "Y")
                return robotPose;
            else
                Console.WriteLine("Enter the pose again.");
        }
    }

    private HandEyeCalibration.Transformation EulerToQuaternion()
    {
        const double PI = Math.PI;

        double a1 = poseR1 * PI / 180 / 2;
        double a2 = poseR2 * PI / 180 / 2;
        double a3 = poseR3 * PI / 180 / 2;

        double quadW = 0, quadX = 0, quadY = 0, quadZ = 0;

        switch (eulerTypeCode)
        {
            case 1: // Z-Y'-X''
                quadW = Math.Sin(a1) * Math.Sin(a2) * Math.Sin(a3) + Math.Cos(a1) * Math.Cos(a2) * Math.Cos(a3);
                quadX = -Math.Sin(a1) * Math.Sin(a2) * Math.Cos(a3) + Math.Sin(a3) * Math.Cos(a1) * Math.Cos(a2);
                quadY = Math.Sin(a1) * Math.Sin(a3) * Math.Cos(a2) + Math.Sin(a2) * Math.Cos(a1) * Math.Cos(a3);
                quadZ = Math.Sin(a1) * Math.Cos(a2) * Math.Cos(a3) - Math.Sin(a2) * Math.Sin(a3) * Math.Cos(a1);
                break;
            case 2: // Z-Y'-Z''
                quadW = Math.Cos(a2) * Math.Cos(a1 + a3);
                quadX = -Math.Sin(a2) * Math.Sin(a1 - a3);
                quadY = Math.Sin(a2) * Math.Cos(a1 - a3);
                quadZ = Math.Cos(a2) * Math.Sin(a1 + a3);
                break;
            case 3: // X-Y'-Z''
                quadW = -Math.Sin(a1) * Math.Sin(a2) * Math.Sin(a3) + Math.Cos(a1) * Math.Cos(a2) * Math.Cos(a3);
                quadX = Math.Sin(a1) * Math.Cos(a2) * Math.Cos(a3) + Math.Sin(a2) * Math.Sin(a3) * Math.Cos(a1);
                quadY = -Math.Sin(a1) * Math.Sin(a3) * Math.Cos(a2) + Math.Sin(a2) * Math.Cos(a1) * Math.Cos(a3);
                quadZ = Math.Sin(a1) * Math.Sin(a2) * Math.Cos(a3) + Math.Sin(a3) * Math.Cos(a1) * Math.Cos(a2);
                break;
            case 4: // Z-X'-Z''
                quadW = Math.Cos(a2) * Math.Cos(a1 + a3);
                quadX = Math.Sin(a2) * Math.Cos(a1 - a3);
                quadY = Math.Sin(a2) * Math.Sin(a1 - a3);
                quadZ = Math.Cos(a2) * Math.Sin(a1 + a3);
                break;
            case 5: // X-Y-Z
                a1 = poseR3 * PI / 180 / 2;
                a3 = poseR1 * PI / 180 / 2;
                quadW = Math.Sin(a1) * Math.Sin(a2) * Math.Sin(a3) + Math.Cos(a1) * Math.Cos(a2) * Math.Cos(a3);
                quadX = -Math.Sin(a1) * Math.Sin(a2) * Math.Cos(a3) + Math.Sin(a3) * Math.Cos(a1) * Math.Cos(a2);
                quadY = Math.Sin(a1) * Math.Sin(a3) * Math.Cos(a2) + Math.Sin(a2) * Math.Cos(a1) * Math.Cos(a3);
                quadZ = Math.Sin(a1) * Math.Cos(a2) * Math.Cos(a3) - Math.Sin(a2) * Math.Sin(a3) * Math.Cos(a1);
                break;
            default:
                throw new ArgumentException("Invalid Euler type code.");
        }

        Console.WriteLine("\nThe entered pose is:");
        Console.WriteLine($"{poseX}, {poseY}, {poseZ}, {poseR1}, {poseR2}, {poseR3}");
        Console.WriteLine("The converted pose (Euler angles --> quaternions) is:");
        Console.WriteLine($"{poseX}, {poseY}, {poseZ}, {quadW}, {quadX}, {quadY}, {quadZ}");

        return new HandEyeCalibration.Transformation(poseX, poseY, poseZ, quadW, quadX, quadY, quadZ);
    }

    private void SaveExtrinsicParameters(string parameters)
    {
        string fileName = $"ExtrinsicParameters_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
        File.WriteAllText(fileName, parameters);
        Console.WriteLine($"Parameters saved to {fileName}");
    }
}
