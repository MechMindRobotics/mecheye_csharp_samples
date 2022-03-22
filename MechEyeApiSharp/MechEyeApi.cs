using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace mmind
{
    namespace apiSharp
    {
        public class MechEyeDevice
        {
            [DllImport("MechEyeApiWrapper.dll")]
            private static extern IntPtr CreateMechEyeDevice();

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern Int32 GetMechEyeDeviceListSize();

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void EnumerateMechEyeDeviceList([In, Out] MechEyeDeviceInfo[] list);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ErrorStatus Connect(IntPtr devicePtr, MechEyeDeviceInfo info);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void Disconnect(IntPtr devicePtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ErrorStatus GetDeviceInfo(IntPtr devicePtr, ref MechEyeDeviceInfo info);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ErrorStatus GetDeviceIntri(IntPtr devicePtr, ref DeviceIntri info);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ErrorStatus GetDeviceResolution(IntPtr devicePtr, ref DeviceResolution info);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ErrorStatus CaptureColorMap(IntPtr devicePtr, IntPtr mapPtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ErrorStatus CaptureDepthMap(IntPtr devicePtr, IntPtr mapPtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ErrorStatus CapturePointXYZMap(IntPtr devicePtr, IntPtr mapPtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ErrorStatus CapturePointXYZBGRMap(IntPtr devicePtr, IntPtr mapPtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ErrorStatus SetScan2DExposureMode(IntPtr devicePtr, Int32 value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ErrorStatus GetScan2DExposureMode(IntPtr devicePtr, ref Int32 value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ErrorStatus SetScan2DExposureTime(IntPtr devicePtr, Double value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ErrorStatus GetScan2DExposureTime(IntPtr devicePtr, ref Double value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ErrorStatus SetScan2DHDRExposureSequence(IntPtr devicePtr, Double[] values, Int32 size);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ErrorStatus GetScan2DHDRExposureSequence(IntPtr devicePtr, ref Double[] values);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ErrorStatus SetScan2DExpectedGrayValue(IntPtr devicePtr, Int32 value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ErrorStatus GetScan2DExpectedGrayValue(IntPtr devicePtr, ref Int32 value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ErrorStatus SetScan2DToneMappingEnable(IntPtr devicePtr, Boolean value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ErrorStatus GetScan2DToneMappingEnable(IntPtr devicePtr, ref Boolean value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ErrorStatus SetScan2DSharpenFactor(IntPtr devicePtr, Double value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ErrorStatus GetScan2DSharpenFactor(IntPtr devicePtr, ref Double value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ErrorStatus SetScan2DROI(IntPtr devicePtr, IntPtr roi);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ErrorStatus GetScan2DROI(IntPtr devicePtr, IntPtr roi);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ErrorStatus SetScan3DExposure(IntPtr devicePtr, Double[] values, Int32 size);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern Int32 GetScan3DExposureSize(IntPtr devicePtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ErrorStatus GetScan3DExposure(IntPtr devicePtr, [In, Out] Double[] values);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ErrorStatus SetScan3DGain(IntPtr devicePtr, Double value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ErrorStatus GetScan3DGain(IntPtr devicePtr, ref Double value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ErrorStatus SetScan3DROI(IntPtr devicePtr, IntPtr roi);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ErrorStatus GetScan3DROI(IntPtr devicePtr, IntPtr roi);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ErrorStatus SetDepthRange(IntPtr devicePtr, IntPtr depthRange);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ErrorStatus GetDepthRange(IntPtr devicePtr, IntPtr depthRange);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ErrorStatus SetFringeContrastThreshold(IntPtr devicePtr, Int32 value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ErrorStatus GetFringeContrastThreshold(IntPtr devicePtr, ref Int32 value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ErrorStatus SetFringeMinThreshold(IntPtr devicePtr, Int32 value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ErrorStatus GetFringeMinThreshold(IntPtr devicePtr, ref Int32 value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ErrorStatus SetCloudOutlierFilterMode(IntPtr devicePtr, Int32 value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ErrorStatus GetCloudOutlierFilterMode(IntPtr devicePtr, ref Int32 value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ErrorStatus SetCloudSmoothMode(IntPtr devicePtr, Int32 value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ErrorStatus GetCloudSmoothMode(IntPtr devicePtr, ref Int32 value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ErrorStatus SetLaserSettings(IntPtr devicePtr, LaserSettings value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ErrorStatus GetLaserSettings(IntPtr devicePtr, ref LaserSettings value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ErrorStatus SaveAllSettingsToUserSets(IntPtr devicePtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ErrorStatus SetCurrentUserSet(IntPtr devicePtr, String value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ErrorStatus GetCurrentUserSet(IntPtr devicePtr, ref String value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern Int32 GetUserSetsCount(IntPtr devicePtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ErrorStatus GetAllUserSets(IntPtr devicePtr, [In, Out] String[] values);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ErrorStatus DeleteUserSet(IntPtr devicePtr, String value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ErrorStatus AddUserSet(IntPtr devicePtr, String value);

            public MechEyeDevice()
            {
                _devicePtr = CreateMechEyeDevice();
            }

            public static List<MechEyeDeviceInfo> enumerateMechEyeDeviceList()
            {
                Int32 size = GetMechEyeDeviceListSize();
                MechEyeDeviceInfo[] arr = new MechEyeDeviceInfo[size];
                EnumerateMechEyeDeviceList(arr);
                return arr.ToList<MechEyeDeviceInfo>();
            }

            public ErrorStatus connect(MechEyeDeviceInfo info)
            {
                return Connect(_devicePtr, info);
            }

            public void disconnect()
            {
                Disconnect(_devicePtr);
            }

            public ErrorStatus getDeviceInfo(ref MechEyeDeviceInfo info)
            {
                return GetDeviceInfo(_devicePtr, ref info);
            }

            public ErrorStatus getDeviceIntri(ref DeviceIntri intri)
            {
                return GetDeviceIntri(_devicePtr, ref intri);
            }

            public ErrorStatus getDeviceResolution(ref DeviceResolution resolution)
            {
                return GetDeviceResolution(_devicePtr, ref resolution);
            }

            public ErrorStatus captureColorMap(ref ColorMap map)
            {
                return CaptureColorMap(_devicePtr, map._mapPtr);
            }

            public ErrorStatus captureDepthMap(ref DepthMap map)
            {
                return CaptureDepthMap(_devicePtr, map._mapPtr);
            }

            public ErrorStatus capturePointXYZMap(ref PointXYZMap map)
            {
                return CapturePointXYZMap(_devicePtr, map._mapPtr);
            }

            public ErrorStatus capturePointXYZBGRMap(ref PointXYZBGRMap map)
            {
                return CapturePointXYZBGRMap(_devicePtr, map._mapPtr);
            }

            public ErrorStatus setScan2DExposureMode(Scan2DExposureMode mode)
            {
                return SetScan2DExposureMode(_devicePtr, (int)mode);
            }

            public ErrorStatus getScan2DExposureMode(ref Scan2DExposureMode mode)
            {
                int val = (int)mode;
                ErrorStatus status = GetScan2DExposureMode(_devicePtr, ref val);
                mode = (Scan2DExposureMode)val;
                return status;
            }

            public ErrorStatus setScan2DExposureTime(double value)
            {
                return SetScan2DExposureTime(_devicePtr, value);
            }

            public ErrorStatus getScan2DExposureTime(ref double value)
            {
                return GetScan2DExposureTime(_devicePtr, ref value);
            }

            public ErrorStatus setScan2DHDRExposureSequence(List<Double> list)
            {
                return SetScan2DHDRExposureSequence(_devicePtr, list.ToArray(), list.Count);
            }

            public ErrorStatus getScan2DHDRExposureSequence(ref List<Double> list)
            {
                Double[] vals = list.ToArray();
                ErrorStatus status = GetScan2DHDRExposureSequence(_devicePtr, ref vals);
                list = vals.ToList<Double>();
                return status;
            }

            public ErrorStatus setScan2DExpectedGrayValue(Int32 value)
            {
                return SetScan2DExpectedGrayValue(_devicePtr, value);
            }

            public ErrorStatus getScan2DExpectedGrayValue(ref Int32 value)
            {
                return GetScan2DExpectedGrayValue(_devicePtr, ref value);
            }

            public ErrorStatus setScan2DToneMappingEnable(Boolean value)
            {
                return SetScan2DToneMappingEnable(_devicePtr, value);
            }

            public ErrorStatus getScan2DToneMappingEnable(ref Boolean value)
            {
                return GetScan2DToneMappingEnable(_devicePtr, ref value);
            }

            public ErrorStatus setScan2DSharpenFactor(Double value)
            {
                return SetScan2DSharpenFactor(_devicePtr, value);
            }

            public ErrorStatus getScan2DSharpenFactor(ref Double value)
            {
                return GetScan2DSharpenFactor(_devicePtr, ref value);
            }

            public ErrorStatus setScan2DROI(ROI roi)
            {
                return SetScan2DROI(_devicePtr, roi._roiPtr);
            }

            public ErrorStatus getScan2DROI(ref ROI roi)
            {
                return GetScan2DROI(_devicePtr, roi._roiPtr);
            }

            public ErrorStatus setScan3DExposure(List<Double> list)
            {
                return SetScan3DExposure(_devicePtr, list.ToArray(), list.Count);
            }

            public ErrorStatus getScan3DExposure(ref List<Double> list)
            {
                int size = GetScan3DExposureSize(_devicePtr);
                Double[] vals = new Double[size];
                ErrorStatus status = GetScan3DExposure(_devicePtr, vals);
                list = vals.ToList<Double>();
                return status;
            }

            public ErrorStatus setScan3DGain(Double value)
            {
                return SetScan3DGain(_devicePtr, value);
            }

            public ErrorStatus getScan3DGain(ref Double value)
            {
                return GetScan3DGain(_devicePtr, ref value);
            }

            public ErrorStatus setScan3DROI(ROI roi)
            {
                return SetScan3DROI(_devicePtr, roi._roiPtr);
            }

            public ErrorStatus getScan3DROI(ref ROI roi)
            {
                return GetScan3DROI(_devicePtr, roi._roiPtr);
            }

            public ErrorStatus setDepthRange(DepthRange range)
            {
                return SetDepthRange(_devicePtr, range._depthRangePtr);
            }

            public ErrorStatus getDepthRange(ref DepthRange range)
            {
                return GetDepthRange(_devicePtr, range._depthRangePtr);
            }

            public ErrorStatus setFringeContrastThreshold(int value)
            {
                return SetFringeContrastThreshold(_devicePtr, value);
            }

            public ErrorStatus getFringeContrastThreshold(ref int value)
            {
                return GetFringeContrastThreshold(_devicePtr, ref value);
            }

            public ErrorStatus setFringeMinThreshold(int value)
            {
                return SetFringeMinThreshold(_devicePtr, value);
            }

            public ErrorStatus getFringeMinThreshold(ref int value)
            {
                return GetFringeMinThreshold(_devicePtr, ref value);
            }

            public ErrorStatus setCloudOutlierFilterMode(CloudOutlierFilterMode mode)
            {
                return SetCloudOutlierFilterMode(_devicePtr, (int)mode);
            }

            public ErrorStatus getCloudOutlierFilterMode(ref CloudOutlierFilterMode mode)
            {
                int val = (int)mode;
                ErrorStatus status = GetCloudOutlierFilterMode(_devicePtr, ref val);
                mode = (CloudOutlierFilterMode)val;
                return status;
            }

             public ErrorStatus setCloudSmoothMode(CloudSmoothMode mode)
            {
                return SetCloudSmoothMode(_devicePtr, (int)mode);
            }

            public ErrorStatus getCloudSmoothMode(ref CloudSmoothMode mode)
            {
                int val = (int)mode;
                ErrorStatus status = GetCloudSmoothMode(_devicePtr, ref val);
                mode = (CloudSmoothMode)val;
                return status;
            }

            public ErrorStatus setLaserSettings(LaserSettings value)
            {
                return SetLaserSettings(_devicePtr, value);
            }

            public ErrorStatus getLaserSettings(ref LaserSettings value)
            {
                return GetLaserSettings(_devicePtr, ref value);
            }

            public ErrorStatus saveAllSettingsToUserSets()
            {
                return SaveAllSettingsToUserSets(_devicePtr);
            }

            public ErrorStatus setCurrentUserSet(String name)
            {
                return SetCurrentUserSet(_devicePtr, name);
            }

            public ErrorStatus getCurrentUserSet(ref String name)
            {
                return GetCurrentUserSet(_devicePtr, ref name);
            }

            public ErrorStatus getAllUserSets(ref List<String> names)
            {
                Int32 size = GetUserSetsCount(_devicePtr);
                String[] namesArr = new String[size];
                ErrorStatus status = GetAllUserSets(_devicePtr, namesArr);
                names = namesArr.ToList<String>();
                return status;
            }

            public ErrorStatus deleteUserSet(String name)
            {
                return DeleteUserSet(_devicePtr, name);
            }

            public ErrorStatus addUserSet(String name)
            {
                return AddUserSet(_devicePtr, name);
            }
               
            private IntPtr _devicePtr;
        }
    }
}
