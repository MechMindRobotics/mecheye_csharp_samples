/*
With this sample, you can retrieve point clouds in the custom reference frame.
*/

using System;
using MMind.Eye;

class TransformPointCloud
{


    static int Main()
    {
        var camera = new Camera();
        if (!Utils.FindAndConnect(ref camera))
            return -1;

        if (!Utils.ConfirmCapture3D())
        {
            camera.Disconnect();
            return 0;
        }
        // Get the textured point cloud
        var frame = new Frame2DAnd3D();
        // Get the untextured point cloud
        var frame3d = new Frame3D();
        Utils.ShowError(camera.Capture2DAnd3D(ref frame));
        Utils.ShowError(camera.Capture3D(ref frame3d));
        var intrinsics = new CameraIntrinsics();
        Utils.ShowError(camera.GetCameraIntrinsics(ref intrinsics));

        // Obtain the rigid body transformation from the camera reference frame to the custom reference
        // frame
        // The custom reference frame can be adjusted using the "Custom Reference Frame" tool in
        // Mech - Eye Viewer.The rigid body transformations are automatically calculated after the
        // settings in this tool have been applied
        var transformation = PointCloudTransformationForCamera.GetTransformationParams(camera);
        if (!transformation.IsValid())
        {
            Console.WriteLine("Transformation parameters are not set. Please configure the transformation parameters using the custom coordinate system tool in the client.");
        }

        // Transform the reference frame of the untextured point cloud and save the point cloud
        var transformedPointCloud = PointCloudTransformationForCamera.TransformPointCloud(transformation, frame3d.GetUntexturedPointCloud());
        var pointCloudFile = "PointCloud.ply";
        var successMessage = "save the untextured point cloud: " + pointCloudFile;
        Utils.ShowError(Frame3D.SaveUntexturedPointCloud(ref transformedPointCloud, FileFormat.PLY, pointCloudFile), successMessage);

        // Transform the reference frame of the untextured point cloud with normals and save the point cloud
        var transformedPointCloudWithNormals = PointCloudTransformationForCamera.TransformPointCloudWithNormals(transformation, frame3d.GetUntexturedPointCloud());
        var pointCloudWithNormalsFile = "PointCloudWithNormals.ply";
        successMessage = "save the untextured point cloud with normals: " + pointCloudWithNormalsFile;
        Utils.ShowError(Frame3D.SaveUntexturedPointCloudWithNormals(ref transformedPointCloudWithNormals, FileFormat.PLY, pointCloudWithNormalsFile), successMessage);

        // Transform the reference frame of the textured point cloud and save the point cloud
        var transformedTexturedPointCloud = PointCloudTransformationForCamera.TransformTexturedPointCloud(transformation, frame.GetTexturedPointCloud());
        var texturedPointCloudFile = "TexturedPointCloud.ply";
        successMessage = "save the textured point cloud: " + texturedPointCloudFile;
        Utils.ShowError(Frame2DAnd3D.SaveTexturedPointCloud(ref transformedTexturedPointCloud, FileFormat.PLY, texturedPointCloudFile), successMessage);

        // Transform the reference frame of the textured point cloud with normals and save point cloud
        var transformedTexturedPointCloudWithNormals = PointCloudTransformationForCamera.TransformTexturedPointCloudWithNormals(transformation, frame.GetTexturedPointCloud());
        var texturedPointCloudWithNormalsFile = "TexturedPointCloudWithNormals.ply";
        successMessage = "save the textured point cloud with normals: " + texturedPointCloudWithNormalsFile;
        Utils.ShowError(Frame2DAnd3D.SaveTexturedPointCloudWithNormals(ref transformedTexturedPointCloudWithNormals, FileFormat.PLY, texturedPointCloudWithNormalsFile), successMessage);

        camera.Disconnect();
        Console.WriteLine("Disconnected from the camera successfully.");
        Console.WriteLine("Press any key to exit ...");
        Console.ReadKey();

        return 0;
    }

}