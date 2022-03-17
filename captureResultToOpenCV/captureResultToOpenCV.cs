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
            //Capture the color image and depth image and save them.
            Mat color = camera.captureColorImg();
            Mat depth = camera.captureDepthImg();

            if (color == null || depth == null)
            {
                Console.WriteLine("Empty images");
                return;
            }
            else
            {
                CvInvoke.Imwrite(save_path + "color.jpg", color);
                CvInvoke.Imwrite(save_path + "depth.tif", depth);
            }
        }
    }
}
