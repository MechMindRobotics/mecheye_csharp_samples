using System;
using System.Runtime.InteropServices;

namespace mmind
{
    namespace apiSharp
    {
        public enum ErrorCode
        {
            MMIND_STATUS_SUCCESS = 0,
            MMIND_STATUS_INVALID_DEVICE = -1,
            MMIND_STATUS_DEVICE_OFFLINE = -2,
            MMIND_STATUS_FIRMWARE_NOT_SUPPORTED = -3,
            MMIND_STATUS_PARAMETER_SET_ERROR = -4,
            MMIND_STATUS_PARAMETER_GET_ERROR = -5,
            MMIND_STATUS_CAPTURE_NO_FRAME = -6
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ErrorStatus
        {
            public ErrorCode errorCode;
            public string errorDescription;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MechEyeDeviceInfo
        {
            public string model;
            public string id;
            public string hardwareVersion;
            public string firmwareVersion;
            public string ipAddress;
            public ushort port;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DeviceIntri
        {
            public double k1;
            public double k2;
            public double p1;
            public double p2;
            public double k3;
            public double fx;
            public double fy;
            public double cx;
            public double cy;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DeviceResolution
        {
            public uint colorMapWidth;
            public uint colorMapHeight;
            public uint depthMapWidth;
            public uint depthMapHeight;
        }

        public class ROI
        {
            [DllImport("MechEyeApiWrapper.dll")]
            private static extern IntPtr CreateROIWithoutParameter();

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern IntPtr CreateROIWithParameter(int x, int y, int width, int height);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void DeleteROI(IntPtr roiPtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern uint GetROIX(IntPtr roiPtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void SetROIX(IntPtr roiPtr, uint value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern uint GetROIY(IntPtr roiPtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void SetROIY(IntPtr roiPtr, uint value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern uint GetROIWidth(IntPtr roiPtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void SetROIWidth(IntPtr roiPtr, uint value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern uint GetROIHeight(IntPtr roiPtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void SetROIHeight(IntPtr roiPtr, uint value);

            public uint x
            {
                get
                {
                    return GetROIX(_roiPtr);
                }
                set
                {
                    SetROIX(_roiPtr, value);
                }
            }
            public uint y
            {
                get
                {
                    return GetROIY(_roiPtr);
                }
                set
                {
                    SetROIY(_roiPtr, value);
                }
            }
            public uint width
            {
                get
                {
                    return GetROIWidth(_roiPtr);
                }
                set
                {
                    SetROIWidth(_roiPtr, value);
                }
            }
            public uint height
            {
                get
                {
                    return GetROIHeight(_roiPtr);
                }
                set
                {
                    SetROIHeight(_roiPtr, value);
                }
            }

            public ROI()
            {
                _roiPtr = CreateROIWithoutParameter();
            }

            public ROI(int x, int y, int width, int height)
            {
                _roiPtr = CreateROIWithParameter(x, y, width, height);
            }

            ~ROI()
            {
                DeleteROI(_roiPtr);
            }

            public readonly IntPtr _roiPtr;
        }

        public class DepthRange
        {
            [DllImport("MechEyeApiWrapper.dll")]
            private static extern IntPtr CreateDepthRangeWithoutParameter();

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern IntPtr CreateDepthRangeWithParameter(int lower, int upper);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void DeleteDepthRange(IntPtr depthRangePtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern int GetLower(IntPtr depthRangePtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void SetLower(IntPtr depthRangePtr, int value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern int GetUpper(IntPtr depthRangePtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void SetUpper(IntPtr depthRangePtr, int value);

            public int lower
            {
                get
                {
                    return GetLower(_depthRangePtr);
                }
                set
                {
                    SetLower(_depthRangePtr, value);
                }
            }

            public int upper
            {
                get
                {
                    return GetUpper(_depthRangePtr);
                }
                set
                {
                    SetUpper(_depthRangePtr, value);
                }
            }

            public DepthRange()
            {
                _depthRangePtr = CreateDepthRangeWithoutParameter();
            }

            public DepthRange(int lower, int upper)
            {
                _depthRangePtr = CreateDepthRangeWithParameter(lower, upper);
            }

            ~DepthRange()
            {
                DeleteDepthRange(_depthRangePtr);
            }

            public readonly IntPtr _depthRangePtr;
        }
    }
}
