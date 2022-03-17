using System;
using Emgu.CV;

namespace Mechmind_CameraAPI_Csharp
{
    class connectAndCaptureImage
    {
        static void printDeviceInfo(in string cameraID, in string cameraVersion)
        {
            Console.WriteLine("............................");
            Console.WriteLine("Camera ID:         " + cameraID);
            Console.WriteLine("Firmware Version:  " + cameraVersion);
            Console.WriteLine("............................");
            Console.WriteLine("");
        }
        static void Main()
        {
            CameraClient camera = new CameraClient();
            Console.WriteLine("Enter Camera IP: ");
            string ip = Console.ReadLine();

            if (Status.Error == camera.connect(ip)) return;

            //Get camera ID and version
            string cameraID = camera.getCameraId();
            string cameraVersion = camera.getCameraVersion();
            printDeviceInfo(cameraID, cameraVersion);

            //Capture the color image and depth image and save them.
            Mat color = camera.captureColorImg();
            Console.WriteLine("Color Image Size: {0} * {1}", color.Width, color.Height);

            Mat depth = camera.captureDepthImg();
            Console.WriteLine("Depth Image Size: {0} * {1}", depth.Width, depth.Height);

            Mat pointXYZMap = camera.captureCloud();
            if (pointXYZMap == null)
            {
                Console.WriteLine("Empty point cloud.");
                return;
            }
            Console.WriteLine("PointCloudXYZ has : {0} data points.", pointXYZMap.Width * pointXYZMap.Height);
        }
    }
}
