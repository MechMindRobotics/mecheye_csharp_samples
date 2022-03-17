using System;
using Emgu.CV;


namespace Mechmind_CameraAPI_Csharp
{
    class captureResultToOpenCV 
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

            string save_path = "D:\\";

            Mat color = camera.captureColorImg();
            Mat pointXYZMap = camera.captureCloud();
            if (pointXYZMap == null)
            {
                Console.WriteLine("Empty point cloud.");
                return;
            }
            CvInvoke.WriteCloud(save_path + "pointCloudXYZ.ply", pointXYZMap);
            Console.WriteLine("PointCloudXYZ has : {0} data points.", pointXYZMap.Width * pointXYZMap.Height);

            CvInvoke.WriteCloud(save_path + "pointCloudXYZRGB.ply", pointXYZMap, color);
            Console.WriteLine("PointCloudXYZRGB has : {0} data points.", pointXYZMap.Width, pointXYZMap.Height);
        }
    }
}
