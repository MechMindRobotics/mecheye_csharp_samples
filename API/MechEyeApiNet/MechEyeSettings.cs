using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace mmind
{
    namespace apiSharp
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Scanning3DSettings
        {
            public List<double> ExposureSequence;
            public double Gain;
            public ROI Scan3DROI;
            public DepthRange DepthRange;
        }

        public enum Scan2DExposureMode
        {
            Timed,
            Auto,
            HDR,
            Flash
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Scanning2DSettings
        {
            public Scan2DExposureMode ExposureMode;
            public double ExposureTime;
            public double SharpenFactor;
            public int ExpectedGrayValue;
            public ROI Scan2DROI;
            public bool ToneMappingEnable;
            List<double> HDRExposureSequence;
        }

        public enum CloudOutlierFilterMode
        {
            Off,
            Weak,
            Normal
        }

        public enum CloudSmoothMode
        {
            Off,
            Normal,
            Weak,
            Strong
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PointCloudProcessingSettings
        {
            public int FringeContrastThreshold;
            public int FringeMinThreshold;
            public CloudOutlierFilterMode OutlierFilterMode;
            public CloudSmoothMode CloudSmoothMode;
        }

        public enum LaserFringeCodingMode
        {
            Fast,
            High
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LaserSettings
        {
            public LaserFringeCodingMode FringeCodingMode;
            public int FrameRangeStart;
            public int FrameRangeEnd;
            public int FramePartitionCount;
            public int PowerLevel;
        }
    }
}
