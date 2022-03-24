using System;
using System.Runtime.InteropServices;

namespace mmind
{
    namespace apiSharp
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct ElementDepth
        {
            public float d;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ElementColor
        {
            public Byte b;
            public Byte g;
            public Byte r;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ElementPointXYZ
        {
            public float x;
            public float y;
            public float z;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ElementPointXYZBGR
        {
            public Byte b;
            public Byte g;
            public Byte r;
            public float x;
            public float y;
            public float z;
        }

        public class ColorMap
        {
            [DllImport("MechEyeApiWrapper.dll")]
            private static extern IntPtr CreateColorMap();

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void DeleteColorMap(IntPtr mapPtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern UInt32 ColorMapWidth(IntPtr mapPtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern UInt32 ColorMapHeight(IntPtr mapPtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern bool ColorMapEmpty(IntPtr mapPtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern IntPtr ColorMapData(IntPtr mapPtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ref ElementColor ColorMapAt(IntPtr mapPtr, UInt32 row, UInt32 col);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void ColorMapResize(IntPtr mapPtr, UInt32 width, UInt32 height);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void ColorMapRelease(IntPtr mapPtr);

            public readonly IntPtr _mapPtr;

            public ColorMap()
            {
                _mapPtr = CreateColorMap();
            }

            ~ColorMap()
            {
                release();
                DeleteColorMap(_mapPtr);
            }

            public UInt32 width()
            {
                return ColorMapWidth(_mapPtr);
            }

            public UInt32 height()
            {
                return ColorMapHeight(_mapPtr);
            }

            public Boolean empty()
            {
                return ColorMapEmpty(_mapPtr);
            }

            public IntPtr data()
            {
                return ColorMapData(_mapPtr);
            }

            public ref ElementColor at(UInt32 row, UInt32 col)
            {
                return ref ColorMapAt(_mapPtr, row, col);
            }

            public void resize(UInt32 width, UInt32 height)
            {
                ColorMapResize(_mapPtr, width, height);
            }

            public void release()
            {
                ColorMapRelease(_mapPtr);
            }
        }
        public class DepthMap
        {
            [DllImport("MechEyeApiWrapper.dll")]
            private static extern IntPtr CreateDepthMap();

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void DeleteDepthMap(IntPtr mapPtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern UInt32 DepthMapWidth(IntPtr mapPtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern UInt32 DepthMapHeight(IntPtr mapPtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern bool DepthMapEmpty(IntPtr mapPtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern IntPtr DepthMapData(IntPtr mapPtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ref ElementDepth DepthMapAt(IntPtr mapPtr, UInt32 row, UInt32 col);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void DepthMapResize(IntPtr mapPtr, UInt32 width, UInt32 height);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void DepthMapRelease(IntPtr mapPtr);

            public readonly IntPtr _mapPtr;

            public DepthMap()
            {
                _mapPtr = CreateDepthMap();
            }

            ~DepthMap()
            {
                release();
                DeleteDepthMap(_mapPtr);
            }

            public UInt32 width()
            {
                return DepthMapWidth(_mapPtr);
            }

            public UInt32 height()
            {
                return DepthMapHeight(_mapPtr);
            }

            public Boolean empty()
            {
                return DepthMapEmpty(_mapPtr);
            }

            public IntPtr data()
            {
                return DepthMapData(_mapPtr);
            }

            public ref ElementDepth at(UInt32 row, UInt32 col)
            {
                return ref DepthMapAt(_mapPtr, row, col);
            }

            public void resize(UInt32 width, UInt32 height)
            {
                DepthMapResize(_mapPtr, width, height);
            }

            public void release()
            {
                DepthMapRelease(_mapPtr);
            }
        }
        public class PointXYZMap
        {
            [DllImport("MechEyeApiWrapper.dll")]
            private static extern IntPtr CreatePointXYZMap();

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void DeletePointXYZMap(IntPtr mapPtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern UInt32 PointXYZMapWidth(IntPtr mapPtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern UInt32 PointXYZMapHeight(IntPtr mapPtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern bool PointXYZMapEmpty(IntPtr mapPtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern IntPtr PointXYZMapData(IntPtr mapPtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ref ElementPointXYZ PointXYZMapAt(IntPtr mapPtr, UInt32 row, UInt32 col);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void PointXYZMapResize(IntPtr mapPtr, UInt32 width, UInt32 height);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void PointXYZMapRelease(IntPtr mapPtr);

            public readonly IntPtr _mapPtr;

            public PointXYZMap()
            {
                _mapPtr = CreatePointXYZMap();
            }

            ~PointXYZMap()
            {
                release();
                DeletePointXYZMap(_mapPtr);
            }

            public UInt32 width()
            {
                return PointXYZMapWidth(_mapPtr);
            }

            public UInt32 height()
            {
                return PointXYZMapHeight(_mapPtr);
            }

            public Boolean empty()
            {
                return PointXYZMapEmpty(_mapPtr);
            }

            public IntPtr data()
            {
                return PointXYZMapData(_mapPtr);
            }

            public ref ElementPointXYZ at(UInt32 row, UInt32 col)
            {
                return ref PointXYZMapAt(_mapPtr, row, col);
            }

            public void resize(UInt32 width, UInt32 height)
            {
                PointXYZMapResize(_mapPtr, width, height);
            }

            public void release()
            {
                PointXYZMapRelease(_mapPtr);
            }
        }
        public class PointXYZBGRMap
        {
            [DllImport("MechEyeApiWrapper.dll")]
            private static extern IntPtr CreatePointXYZBGRMap();

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void DeletePointXYZBGRMap(IntPtr mapPtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern UInt32 PointXYZBGRMapWidth(IntPtr mapPtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern UInt32 PointXYZBGRMapHeight(IntPtr mapPtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern bool PointXYZBGRMapEmpty(IntPtr mapPtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern IntPtr PointXYZBGRMapData(IntPtr mapPtr);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern ref ElementPointXYZBGR PointXYZBGRMapAt(IntPtr mapPtr, UInt32 row, UInt32 col);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void PointXYZBGRMapResize(IntPtr mapPtr, UInt32 width, UInt32 height);

            [DllImport("MechEyeApiWrapper.dll")]
            private static extern void PointXYZBGRMapRelease(IntPtr mapPtr);

            public readonly IntPtr _mapPtr;

            public PointXYZBGRMap()
            {
                _mapPtr = CreatePointXYZBGRMap();
            }

            ~PointXYZBGRMap()
            {
                release();
                DeletePointXYZBGRMap(_mapPtr);
            }

            public UInt32 width()
            {
                return PointXYZBGRMapWidth(_mapPtr);
            }

            public UInt32 height()
            {
                return PointXYZBGRMapHeight(_mapPtr);
            }

            public Boolean empty()
            {
                return PointXYZBGRMapEmpty(_mapPtr);
            }

            public IntPtr data()
            {
                return PointXYZBGRMapData(_mapPtr);
            }

            public ref ElementPointXYZBGR at(UInt32 row, UInt32 col)
            {
                return ref PointXYZBGRMapAt(_mapPtr, row, col);
            }

            public void resize(UInt32 width, UInt32 height)
            {
                PointXYZBGRMapResize(_mapPtr, width, height);
            }

            public void release()
            {
                PointXYZBGRMapRelease(_mapPtr);
            }
        }

    }
}
