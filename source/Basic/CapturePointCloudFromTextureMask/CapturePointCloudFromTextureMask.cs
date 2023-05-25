/*
With this sample program, you can construct and save untextured and textured point clouds (PCL
format) generated from a depth map and masked 2D image.
*/

using System;
using System.Collections.Generic;
using Emgu.CV;
using Emgu.CV.CvEnum;
using mmind.apiSharp;

namespace CapturePointCloudFromTextureMask
{
    class Sample
    {
        static void ShowError(ErrorStatus status)
        {
            if (status.errorCode == (int)ErrorCode.MMIND_STATUS_SUCCESS)
                return;
            Console.WriteLine("Error Code : {0}, Error Description: {1}.", status.errorCode, status.errorDescription);
        }
        static void PrintDeviceInfo(MechEyeDeviceInfo deviceInfo)
        {
            Console.WriteLine("............................");
            Console.WriteLine("Camera Model Name: {0}", deviceInfo.model);
            Console.WriteLine("Camera ID:         {0}", deviceInfo.id);
            Console.WriteLine("Camera IP Address: {0}", deviceInfo.ipAddress);
            Console.WriteLine("Hardware Version:  V{0}", deviceInfo.hardwareVersion);
            Console.WriteLine("Firmware Version:  V{0}", deviceInfo.firmwareVersion);
            Console.WriteLine("............................");
            Console.WriteLine("");
        }
        private static bool Contains(ROI roi, uint x, uint y)
        {
            return x >= roi.x && x <= roi.x + roi.width && y >= roi.y && y <= roi.y + roi.height;
        }
        private static ColorMap GenerateTextureMask(uint width, uint height, ROI roi1, ROI roi2)
        {
            ColorMap colorMask = new ColorMap();
            colorMask.Resize(width, height);
            for (uint r = 0; r < height; ++r)
            {
                for (uint c = 0; c < width; ++c)
                {
                    if (Contains(roi1, c, r) || Contains(roi2, c, r))
                    {
                        colorMask.At(r, c).b = 1;
                        colorMask.At(r, c).g = 1;
                        colorMask.At(r, c).r = 1;
                    }
                }
            }
            return colorMask;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Find Mech-Eye devices...");
            List<MechEyeDeviceInfo> deviceInfoList = MechEyeDevice.EnumerateMechEyeDeviceList();

            if (deviceInfoList.Count == 0)
            {
                Console.WriteLine("No Mech-Eye Device found.");
                return;
            }

            for (int i = 0; i < deviceInfoList.Count; ++i)
            {
                Console.WriteLine("Mech-Eye device index : {0}", i);
                PrintDeviceInfo(deviceInfoList[i]);
            }

            Console.WriteLine("Please enter the device index you want to connect: ");
            int inputIndex = 0;

            while (true)
            {
                string input = Console.ReadLine();
                if (int.TryParse(input, out inputIndex) && inputIndex >= 0 && inputIndex < deviceInfoList.Count)
                    break;
                Console.WriteLine("Input invalid! Please enter the device index you wnat to connect: ");
            }

            ErrorStatus status = new ErrorStatus();
            MechEyeDevice device = new MechEyeDevice();
            status = device.Connect(deviceInfoList[inputIndex], 10000);

            if (status.errorCode != (int)ErrorCode.MMIND_STATUS_SUCCESS)
            {
                ShowError(status);
                return;
            }

            Console.WriteLine("Connected to the Mech-Eye device successfully.");

            MechEyeDeviceInfo deviceInfo = new MechEyeDeviceInfo();
            ShowError(device.GetDeviceInfo(ref deviceInfo));
            PrintDeviceInfo(deviceInfo);

            ColorMap color = new ColorMap();
            ShowError(device.CaptureColorMap(ref color));

            DepthMap depth = new DepthMap();
            ShowError(device.CaptureDepthMap(ref depth));

            DeviceIntri deviceIntri = new DeviceIntri();
            ShowError(device.GetDeviceIntri(ref deviceIntri));

            ROI roi1 = new ROI((int)(color.Width()/ 5), (int)(color.Height() / 5), (int)(color.Width() / 2), (int)(color.Height() / 2));
            ROI roi2 = new ROI((int)(color.Width() * 2 / 5), (int)(color.Height() * 2 / 5), (int)(color.Width() / 2), (int)(color.Height() / 2));
            ColorMap textureMask = GenerateTextureMask((uint)color.Width(), (uint)color.Height(), roi1, roi2);

            PointXYZMap pointXYZMap = new PointXYZMap();
            ShowError(device.GetCloudFromTextureMask(depth, textureMask,
                             deviceIntri,
                             ref pointXYZMap));
            PointXYZBGRMap pointXYZBGRMap = new PointXYZBGRMap();
            ShowError(device.GetBGRCloudFromTextureMask(depth, textureMask, color, 
                             deviceIntri,
                             ref pointXYZBGRMap));


            string pointCloudPath = "PointCloudXYZ.ply";
            Mat depth32FC3 = new Mat(unchecked((int)pointXYZMap.Height()), unchecked((int)pointXYZMap.Width()), DepthType.Cv32F, 3, pointXYZMap.Data(), unchecked((int)pointXYZMap.Width()) * 12);
            CvInvoke.WriteCloud(pointCloudPath, depth32FC3);
            Console.WriteLine("PointCloudXYZ has : {0} data points.", depth32FC3.Rows * depth32FC3.Cols);


            ColorMap colorRoi = new ColorMap();
            colorRoi.Resize(pointXYZBGRMap.Width(), pointXYZBGRMap.Height());
            for (uint i = 0; i < pointXYZBGRMap.Height(); i++)
                for (uint j = 0; j < pointXYZBGRMap.Width(); j++)
                {
                    colorRoi.At(i, j).b = pointXYZBGRMap.At(i, j).b;
                    colorRoi.At(i, j).g = pointXYZBGRMap.At(i, j).g;
                    colorRoi.At(i, j).r = pointXYZBGRMap.At(i, j).r;
                }
            
            Mat color8UC3 = new Mat(unchecked((int)colorRoi.Height()), unchecked((int)colorRoi.Width()), DepthType.Cv8U, 3, colorRoi.Data(), unchecked((int)colorRoi.Width()) * 3);
            string pointCloudBGRPath = "PointCloudXYZBGR.ply";
            CvInvoke.WriteCloud(pointCloudBGRPath, depth32FC3, color8UC3);
            Console.WriteLine("PointCloudXYZRGB has: {0} data points.", depth32FC3.Rows * depth32FC3.Cols);

            device.Disconnect();
            Console.WriteLine("Disconnected from the Mech-Eye device successfully.");

            Console.ReadKey();
            return;
        }
    }
}
