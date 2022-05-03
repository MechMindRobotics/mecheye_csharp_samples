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
            private static extern int GetMechEyeDeviceListSize();

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void EnumerateMechEyeDeviceList(ref IntPtr infos, ref int size);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void Connect(out ErrorStatus status, IntPtr devicePtr, MechEyeDeviceInfo info);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void Disconnect(IntPtr devicePtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void GetDeviceInfo(out ErrorStatus status, IntPtr devicePtr, ref MechEyeDeviceInfo info);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void GetDeviceIntri(out ErrorStatus status, IntPtr devicePtr, ref DeviceIntri intri);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void GetDeviceResolution(out ErrorStatus status, IntPtr devicePtr, ref DeviceResolution resolution);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void CaptureColorMap(out ErrorStatus status, IntPtr devicePtr, IntPtr mapPtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void CaptureDepthMap(out ErrorStatus status, IntPtr devicePtr, IntPtr mapPtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void CapturePointXYZMap(out ErrorStatus status, IntPtr devicePtr, IntPtr mapPtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void CapturePointXYZBGRMap(out ErrorStatus status, IntPtr devicePtr, IntPtr mapPtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void SetScan2DExposureMode(out ErrorStatus status, IntPtr devicePtr, int value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void GetScan2DExposureMode(out ErrorStatus status, IntPtr devicePtr, ref int value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void SetScan2DExposureTime(out ErrorStatus status, IntPtr devicePtr, double value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void GetScan2DExposureTime(out ErrorStatus status, IntPtr devicePtr, ref double value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void SetScan2DHDRExposureSequence(out ErrorStatus status, IntPtr devicePtr, double[] values, int size);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void GetScan2DHDRExposureSequence(out ErrorStatus status, IntPtr devicePtr, ref double[] values);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void SetScan2DExpectedGrayValue(out ErrorStatus status, IntPtr devicePtr, int value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void GetScan2DExpectedGrayValue(out ErrorStatus status, IntPtr devicePtr, ref int value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void SetScan2DToneMappingEnable(out ErrorStatus status, IntPtr devicePtr, bool value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void GetScan2DToneMappingEnable(out ErrorStatus status, IntPtr devicePtr, ref bool value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void SetScan2DSharpenFactor(out ErrorStatus status, IntPtr devicePtr, double value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void GetScan2DSharpenFactor(out ErrorStatus status, IntPtr devicePtr, ref double value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void SetScan2DROI(out ErrorStatus status, IntPtr devicePtr, IntPtr roi);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void GetScan2DROI(out ErrorStatus status, IntPtr devicePtr, IntPtr roi);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void SetScan3DExposure(out ErrorStatus status, IntPtr devicePtr, [In, Out] double[] values, int size);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void GetScan3DExposure(out ErrorStatus status, IntPtr devicePtr, ref IntPtr values, ref int size);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void SetScan3DGain(out ErrorStatus status, IntPtr devicePtr, double value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void GetScan3DGain(out ErrorStatus status, IntPtr devicePtr, ref double value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void SetScan3DROI(out ErrorStatus status, IntPtr devicePtr, IntPtr roi);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void GetScan3DROI(out ErrorStatus status, IntPtr devicePtr, IntPtr roi);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void SetDepthRange(out ErrorStatus status, IntPtr devicePtr, IntPtr depthRange);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void GetDepthRange(out ErrorStatus status, IntPtr devicePtr, IntPtr depthRange);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void SetFringeContrastThreshold(out ErrorStatus status, IntPtr devicePtr, int value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void GetFringeContrastThreshold(out ErrorStatus status, IntPtr devicePtr, ref int value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void SetFringeMinThreshold(out ErrorStatus status, IntPtr devicePtr, int value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void GetFringeMinThreshold(out ErrorStatus status, IntPtr devicePtr, ref int value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void SetCloudOutlierFilterMode(out ErrorStatus status, IntPtr devicePtr, int value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void GetCloudOutlierFilterMode(out ErrorStatus status, IntPtr devicePtr, ref int value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void SetCloudSmoothMode(out ErrorStatus status, IntPtr devicePtr, int value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void GetCloudSmoothMode(out ErrorStatus status, IntPtr devicePtr, ref int value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void SetLaserSettings(out ErrorStatus status, IntPtr devicePtr, LaserSettings value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void GetLaserSettings(out ErrorStatus status, IntPtr devicePtr, ref LaserSettings value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void SaveAllSettingsToUserSets(out ErrorStatus status, IntPtr devicePtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void SetCurrentUserSet(out ErrorStatus status, IntPtr devicePtr, string value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void GetCurrentUserSet(out ErrorStatus status, IntPtr devicePtr, [MarshalAs(UnmanagedType.BStr)] ref string value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern int GetUserSetsCount(IntPtr devicePtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void GetAllUserSets(out ErrorStatus status, IntPtr devicePtr, [In, Out] string[] values);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void DeleteUserSet(out ErrorStatus status, IntPtr devicePtr, string value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void AddUserSet(out ErrorStatus status, IntPtr devicePtr, string value);

            public MechEyeDevice()
            {
                _devicePtr = CreateMechEyeDevice();
            }

            public static List<MechEyeDeviceInfo> enumerateMechEyeDeviceList()
            {
                int size = 1;
                MechEyeDeviceInfo[] infos = new MechEyeDeviceInfo[size];
                infos[0] = new MechEyeDeviceInfo();
                int structSize = Marshal.SizeOf(infos[0]);
                IntPtr buffer = Marshal.AllocCoTaskMem(structSize * infos.Length);
                EnumerateMechEyeDeviceList(ref buffer, ref size);
                infos = new MechEyeDeviceInfo[size];
                int offset = 0;
                for (int i = 0; i < size; ++i)
                {
                    infos[i] = Marshal.PtrToStructure<MechEyeDeviceInfo>(new IntPtr(buffer.ToInt32() + offset));
                    offset += structSize;
                }
                return infos.ToList<MechEyeDeviceInfo>();
            }

            public ErrorStatus connect(MechEyeDeviceInfo info)
            {
                ErrorStatus status = new ErrorStatus();
                Connect(out status, _devicePtr, info);
                return status;
            }

            public void disconnect()
            {
                Disconnect(_devicePtr);
            }

            public ErrorStatus getDeviceInfo(ref MechEyeDeviceInfo info)
            {
                info.model = "";
                info.hardwareVersion = "";
                info.firmwareVersion = "";
                info.ipAddress = "";
                info.id = "";
                ErrorStatus status = new ErrorStatus();
                GetDeviceInfo(out status, _devicePtr, ref info);
                return status;
            }

            public ErrorStatus getDeviceIntri(ref DeviceIntri intri)
            {
                ErrorStatus status = new ErrorStatus();
                GetDeviceIntri(out status, _devicePtr, ref intri);
                return status;
            }

            public ErrorStatus getDeviceResolution(ref DeviceResolution resolution)
            {
                ErrorStatus status = new ErrorStatus();
                GetDeviceResolution(out status, _devicePtr, ref resolution);
                return status;
            }

            public ErrorStatus captureColorMap(ref ColorMap map)
            {
                ErrorStatus status = new ErrorStatus();
                CaptureColorMap(out status, _devicePtr, map._mapPtr);
                return status;
            }

            public ErrorStatus captureDepthMap(ref DepthMap map)
            {
                ErrorStatus status = new ErrorStatus();
                CaptureDepthMap(out status, _devicePtr, map._mapPtr);
                return status;
            }

            public ErrorStatus capturePointXYZMap(ref PointXYZMap map)
            {
                ErrorStatus status = new ErrorStatus();
                CapturePointXYZMap(out status, _devicePtr, map._mapPtr);
                return status;
            }

            public ErrorStatus capturePointXYZBGRMap(ref PointXYZBGRMap map)
            {
                ErrorStatus status = new ErrorStatus();
                CapturePointXYZBGRMap(out status, _devicePtr, map._mapPtr);
                return status;
            }

            public ErrorStatus setScan2DExposureMode(Scan2DExposureMode mode)
            {
                ErrorStatus status = new ErrorStatus();
                SetScan2DExposureMode(out status, _devicePtr, (int)mode);
                return status;
            }

            public ErrorStatus getScan2DExposureMode(ref Scan2DExposureMode mode)
            {
                ErrorStatus status = new ErrorStatus();
                int val = (int)mode;
                GetScan2DExposureMode(out status, _devicePtr, ref val);
                mode = (Scan2DExposureMode)val;
                return status;
            }

            public ErrorStatus setScan2DExposureTime(double value)
            {
                ErrorStatus status = new ErrorStatus();
                SetScan2DExposureTime(out status, _devicePtr, value);
                return status;
            }

            public ErrorStatus getScan2DExposureTime(ref double value)
            {
                ErrorStatus status = new ErrorStatus();
                GetScan2DExposureTime(out status, _devicePtr, ref value);
                return status;
            }

            public ErrorStatus setScan2DHDRExposureSequence(List<double> list)
            {
                ErrorStatus status = new ErrorStatus();
                SetScan2DHDRExposureSequence(out status, _devicePtr, list.ToArray(), list.Count);
                return status;
            }

            public ErrorStatus getScan2DHDRExposureSequence(ref List<double> list)
            {
                ErrorStatus status = new ErrorStatus();
                double[] vals = list.ToArray();
                GetScan2DHDRExposureSequence(out status, _devicePtr, ref vals);
                list = vals.ToList<Double>();
                return status;
            }

            public ErrorStatus setScan2DExpectedGrayValue(int value)
            {
                ErrorStatus status = new ErrorStatus();
                SetScan2DExpectedGrayValue(out status, _devicePtr, value);
                return status;
            }

            public ErrorStatus getScan2DExpectedGrayValue(ref int value)
            {
                ErrorStatus status = new ErrorStatus();
                GetScan2DExpectedGrayValue(out status, _devicePtr, ref value);
                return status;
            }

            public ErrorStatus setScan2DToneMappingEnable(bool value)
            {
                ErrorStatus status = new ErrorStatus();
                SetScan2DToneMappingEnable(out status, _devicePtr, value);
                return status;
            }

            public ErrorStatus getScan2DToneMappingEnable(ref bool value)
            {
                ErrorStatus status = new ErrorStatus();
                GetScan2DToneMappingEnable(out status, _devicePtr, ref value);
                return status;
            }

            public ErrorStatus setScan2DSharpenFactor(double value)
            {
                ErrorStatus status = new ErrorStatus();
                SetScan2DSharpenFactor(out status, _devicePtr, value);
                return status;
            }

            public ErrorStatus getScan2DSharpenFactor(ref double value)
            {
                ErrorStatus status = new ErrorStatus();
                GetScan2DSharpenFactor(out status, _devicePtr, ref value);
                return status;
            }

            public ErrorStatus setScan2DROI(ROI roi)
            {
                ErrorStatus status = new ErrorStatus();
                SetScan2DROI(out status, _devicePtr, roi._roiPtr);
                return status;
            }

            public ErrorStatus getScan2DROI(ref ROI roi)
            {
                ErrorStatus status = new ErrorStatus();
                GetScan2DROI(out status, _devicePtr, roi._roiPtr);
                return status;
            }

            public ErrorStatus setScan3DExposure(List<double> list)
            {
                ErrorStatus status = new ErrorStatus();
                SetScan3DExposure(out status, _devicePtr, list.ToArray(), list.Count);
                return status;
            }

            public ErrorStatus getScan3DExposure(ref List<double> list)
            {
                ErrorStatus status = new ErrorStatus();
                int size = 1;
                double[] vals = new double[size];
                vals[0] = 0;
                IntPtr buffer = Marshal.AllocCoTaskMem(Marshal.SizeOf(vals[0]) * vals.Length);
                GetScan3DExposure(out status, _devicePtr, ref buffer, ref size);
                vals = new double[size];
                Marshal.Copy(buffer, vals, 0, size);
                Marshal.FreeCoTaskMem(buffer);
                list = vals.ToList<Double>();
                return status;
            }

            public ErrorStatus setScan3DGain(double value)
            {
                ErrorStatus status = new ErrorStatus();
                SetScan3DGain(out status, _devicePtr, value);
                return status;
            }

            public ErrorStatus getScan3DGain(ref double value)
            {
                ErrorStatus status = new ErrorStatus();
                GetScan3DGain(out status, _devicePtr, ref value);
                return status;
            }

            public ErrorStatus setScan3DROI(ROI roi)
            {
                ErrorStatus status = new ErrorStatus();
                SetScan3DROI(out status, _devicePtr, roi._roiPtr);
                return status;
            }

            public ErrorStatus getScan3DROI(ref ROI roi)
            {
                ErrorStatus status = new ErrorStatus();
                GetScan3DROI(out status, _devicePtr, roi._roiPtr);
                return status;
            }

            public ErrorStatus setDepthRange(DepthRange range)
            {
                ErrorStatus status = new ErrorStatus();
                SetDepthRange(out status, _devicePtr, range._depthRangePtr);
                return status;
            }

            public ErrorStatus getDepthRange(ref DepthRange range)
            {
                ErrorStatus status = new ErrorStatus();
                GetDepthRange(out status, _devicePtr, range._depthRangePtr);
                return status;
            }

            public ErrorStatus setFringeContrastThreshold(int value)
            {
                ErrorStatus status = new ErrorStatus();
                SetFringeContrastThreshold(out status, _devicePtr, value);
                return status;
            }

            public ErrorStatus getFringeContrastThreshold(ref int value)
            {
                ErrorStatus status = new ErrorStatus();
                GetFringeContrastThreshold(out status, _devicePtr, ref value);
                return status;
            }

            public ErrorStatus setFringeMinThreshold(int value)
            {
                ErrorStatus status = new ErrorStatus();
                SetFringeMinThreshold(out status, _devicePtr, value);
                return status;
            }

            public ErrorStatus getFringeMinThreshold(ref int value)
            {
                ErrorStatus status = new ErrorStatus();
                GetFringeMinThreshold(out status, _devicePtr, ref value);
                return status;
            }

            public ErrorStatus setCloudOutlierFilterMode(CloudOutlierFilterMode mode)
            {
                ErrorStatus status = new ErrorStatus();
                SetCloudOutlierFilterMode(out status, _devicePtr, (int)mode);
                return status;
            }

            public ErrorStatus getCloudOutlierFilterMode(ref CloudOutlierFilterMode mode)
            {
                ErrorStatus status = new ErrorStatus();
                int val = (int)mode;
                GetCloudOutlierFilterMode(out status, _devicePtr, ref val);
                mode = (CloudOutlierFilterMode)val;
                return status;
            }

             public ErrorStatus setCloudSmoothMode(CloudSmoothMode mode)
            {
                ErrorStatus status = new ErrorStatus();
                SetCloudSmoothMode(out status, _devicePtr, (int)mode);
                return status;
            }

            public ErrorStatus getCloudSmoothMode(ref CloudSmoothMode mode)
            {
                ErrorStatus status = new ErrorStatus();
                int val = (int)mode;
                GetCloudSmoothMode(out status, _devicePtr, ref val);
                mode = (CloudSmoothMode)val;
                return status;
            }

            public ErrorStatus setLaserSettings(LaserSettings value)
            {
                ErrorStatus status = new ErrorStatus();
                SetLaserSettings(out status, _devicePtr, value);
                return status;
            }

            public ErrorStatus getLaserSettings(ref LaserSettings value)
            {
                ErrorStatus status = new ErrorStatus();
                GetLaserSettings(out status, _devicePtr, ref value);
                return status;
            }

            public ErrorStatus saveAllSettingsToUserSets()
            {
                ErrorStatus status = new ErrorStatus();
                SaveAllSettingsToUserSets(out status, _devicePtr);
                return status;
            }

            public ErrorStatus setCurrentUserSet(string name)
            {
                ErrorStatus status = new ErrorStatus();
                SetCurrentUserSet(out status, _devicePtr, name);
                return status;
            }

            public ErrorStatus getCurrentUserSet(ref string name)
            {
                ErrorStatus status = new ErrorStatus();
                GetCurrentUserSet(out status, _devicePtr, ref name);
                return status;
            }

            public ErrorStatus getAllUserSets(ref List<string> names)
            {
                ErrorStatus status = new ErrorStatus();
                int size = GetUserSetsCount(_devicePtr);
                string[] namesArr = new string[size];
                GetAllUserSets(out status, _devicePtr, namesArr);
                names = namesArr.ToList<String>();
                return status;
            }

            public ErrorStatus deleteUserSet(string name)
            {
                ErrorStatus status = new ErrorStatus();
                DeleteUserSet(out status, _devicePtr, name);
                return status;
            }

            public ErrorStatus addUserSet(string name)
            {
                ErrorStatus status = new ErrorStatus();
                AddUserSet(out status, _devicePtr, name);
                return status;
            }
               
            private IntPtr _devicePtr;
        }
    }
}
