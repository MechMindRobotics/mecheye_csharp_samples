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

            [MarshalAs(UnmanagedType.BStr)]
            public String errorDescription;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MechEyeDeviceInfo
        {
            [MarshalAs(UnmanagedType.BStr)]
            public String model;

            [MarshalAs(UnmanagedType.BStr)]
            public String id;

            [MarshalAs(UnmanagedType.BStr)]
            public String hardwareVersion;
            
            [MarshalAs(UnmanagedType.BStr)]
            public String firmwareVersion;

            [MarshalAs(UnmanagedType.BStr)]
            public String ipAddress;

            public UInt16 port;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DeviceIntri
        {
            public Double k1;
            public Double k2;
            public Double p1;
            public Double p2;
            public Double k3;
            public Double fx;
            public Double fy;
            public Double cx;
            public Double cy;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DeviceResolution
        {
            public UInt32 colorMapWidth;
            public UInt32 colorMapHeight;
            public UInt32 depthMapWidth;
            public UInt32 depthMapHeight;
        }

        public class ROI
        {
            [DllImport("MechEyeApiWrapper.dll")]
            private static extern IntPtr CreateROIWithoutParameter();

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern IntPtr CreateROIWithParameter(Int32 x, Int32 y, Int32 width, Int32 height);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void DeleteROI(IntPtr roiPtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern UInt32 GetROIX(IntPtr roiPtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void SetROIX(IntPtr roiPtr, UInt32 value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern UInt32 GetROIY(IntPtr roiPtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void SetROIY(IntPtr roiPtr, UInt32 value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern UInt32 GetROIWidth(IntPtr roiPtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void SetROIWidth(IntPtr roiPtr, UInt32 value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern UInt32 GetROIHeight(IntPtr roiPtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void SetROIHeight(IntPtr roiPtr, UInt32 value);

            public UInt32 x
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
            public UInt32 y
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
            public UInt32 width
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
            public UInt32 height
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

            public ROI(Int32 x, Int32 y, Int32 width, Int32 height)
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
            private static extern IntPtr CreateDepthRangeWithParameter(Int32 lower, Int32 upper);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void DeleteDepthRange(IntPtr depthRangePtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern Int32 GetLower(IntPtr depthRangePtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void SetLower(IntPtr depthRangePtr, Int32 value);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern Int32 GetUpper(IntPtr depthRangePtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void SetUpper(IntPtr depthRangePtr, Int32 value);

            public Int32 lower
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

            public Int32 upper
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

            public DepthRange(Int32 lower, Int32 upper)
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
