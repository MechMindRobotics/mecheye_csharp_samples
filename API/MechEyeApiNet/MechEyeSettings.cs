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
            public List<Double> ExposureSequence;
            public Double Gain;
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
            public Double ExposureTime;
            public Double SharpenFactor;
            public Int32 ExpectedGrayValue;
            public ROI Scan2DROI;
            public Boolean ToneMappingEnable;
            List<Double> HDRExposureSequence;
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
            public Int32 FringeContrastThreshold;
            public Int32 FringeMinThreshold;
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
            public Int32 FrameAmplitude;
            public Int32 FrameOffset;
            public Int32 FramePartitionCount;
            public Int32 PowerLevel;
        }
    }
}
