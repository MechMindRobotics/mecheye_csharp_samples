using System;
using MMind.Eye;
using System.Windows.Forms;

class AcquireRawImage
{
    static int Main()
    {
        var profiler = new Profiler();
        if (!Utils.FindAndConnect(ref profiler))
            return -1;

        // Set Exposure Time
        var currentUserSet = profiler.CurrentUserSet();
        Utils.ShowError(currentUserSet.SetIntValue(MMind.Eye.BrightnessSettings.ExposureTime.Name, 60));

        // Set ZNarrowing 
        Utils.ShowError(currentUserSet.SetEnumValue(MMind.Eye.ZDirectionROI.ZDirectionRoi.Name, (int)(MMind.Eye.ZDirectionROI.ZDirectionRoi.Value.ImageHeight_1_1)));

        var rawImage = new RawImage();
        Utils.ShowError(profiler.CaptureRawImage(ref rawImage));
        string imageFile = "LineScanImage.png";
        var rawData = rawImage.GetData();
        var bitmap = rawData.ToBitmap();
        var form = new Form
        {
            Text = "Image Viewer",
            Size = bitmap.Size
        };
        PictureBox pictureBox = new PictureBox
        {
            Image = bitmap,
            Dock = DockStyle.Fill
        };
        form.Controls.Add(pictureBox);
        Application.Run(form);

        Console.WriteLine("Capture and save LineScan raw image : {0}", imageFile);
        bitmap.Save(imageFile);

        profiler.Disconnect();
        Console.WriteLine("Disconnected from the Mech-Eye profiler successfully.");
        Console.WriteLine("Press any key to exit ...");
        Console.ReadKey();
        return 0;
    }
}

