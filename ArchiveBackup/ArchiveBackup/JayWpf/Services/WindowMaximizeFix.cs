// ······································································//
// <copyright file="WindowMaximizeFix.cs" company="Jay Bautista Mendoza">//
//     Copyright (c) Jay Bautista Mendoza. All rights reserved.          //
//     THIS IS PART OF MY PERSONAL OPEN SOURCE WPF WINDOW TEMPLATE.      //
//     THIS IS NOT PRIVATE PROPERTY. FEEL FREE TO MODIFY OR USE IT.      //
// </copyright>                                                          //
// ······································································//

namespace JayWpf.Services
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>Class of the Window maximize fix.</summary>
    internal class WindowMaximizeFix
    {
        #region METHODS · PUBLIC · STATIC
        /// <summary>Maximized WindowState fix.</summary>
        /// <param name="hwnd"></param>
        /// <param name="lParam"></param>
        public static void WmGetMinMaxInfo(System.IntPtr hwnd, System.IntPtr lParam)
        {
            MinMaxInfo mmi = (MinMaxInfo)Marshal.PtrToStructure(lParam, typeof(MinMaxInfo));

            // Adjust the maximized size and position to fit the work area of the correct monitor
            int MONITOR_DEFAULTTONEAREST = 0x00000002;
            System.IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

            if (monitor != System.IntPtr.Zero)
            {
                MonitorInfo monitorInfo = new MonitorInfo();
                GetMonitorInfo(monitor, monitorInfo);
                Rect rcWorkArea = monitorInfo.rcWork;
                Rect rcMonitorArea = monitorInfo.rcMonitor;
                mmi.MaxPosition.X = Math.Abs(rcWorkArea.Left - rcMonitorArea.Left);
                mmi.MaxPosition.Y = Math.Abs(rcWorkArea.Top - rcMonitorArea.Top);
                mmi.MaxSize.X = Math.Abs(rcWorkArea.Right - rcWorkArea.Left);
                mmi.MaxSize.Y = Math.Abs(rcWorkArea.Bottom - rcWorkArea.Top);
            }

            Marshal.StructureToPtr(mmi, lParam, true);
        }
        #endregion

        #region METHODS · INTERNAL · STATIC
        [DllImport("user32")]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, MonitorInfo lpmi);

        [DllImport("User32")]
        internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);
        #endregion

        #region METHODS · PRIVATE · STATIC
        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(ref System.Windows.Point lpPoint);
        #endregion

        #region STRUCTS · PUBLIC
        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            #region FIELDS · PUBLIC · NON-STATIC · NON-READONLY
            public int X;
            public int Y;
            #endregion

            #region CONSTRUCTORS · PUBLIC · NON-STATIC
            public Point(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
            #endregion
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MinMaxInfo
        {
            #region FIELDS · PUBLIC · NON-STATIC · NON-READONLY
            public Point Reserved;
            public Point MaxSize;
            public Point MaxPosition;
            public Point MinTrackSize;
            public Point MaxTrackSize;
            #endregion
        }

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct Rect
        {
            #region FIELDS · PUBLIC · STATIC · READONLY
            public static readonly Rect Empty = new Rect(); 
            #endregion

            #region FIELDS · PUBLIC · NON-STATIC · NON-READONLY
            public int Left;
            public int Top;
            public int Right;
            public int Bottom; 
            #endregion

            #region PROPERTIES · PUBLIC · NON-STATIC
            public int Width { get { return Math.Abs(this.Right - this.Left); } }
            public int Height { get { return this.Bottom - this.Top; } }
            public bool IsEmpty { get { return this.Left >= this.Right || this.Top >= this.Bottom; } }
            #endregion

            #region CONSTRUCTORS · PUBLIC · NON-STATIC
            public Rect(int Left, int Top, int Right, int Bottom)
            {
                this.Left = Left;
                this.Top = Top;
                this.Right = Right;
                this.Bottom = Bottom;
            }

            public Rect(Rect rect)
            {
                this.Left = rect.Left;
                this.Top = rect.Top;
                this.Right = rect.Right;
                this.Bottom = rect.Bottom;
            } 
            #endregion

            #region METHODS · PUBLIC · STATIC
            public static bool operator ==(Rect rect1, Rect rect2)
            {
                return (rect1.Left == rect2.Left && rect1.Top == rect2.Top && rect1.Right == rect2.Right && rect1.Bottom == rect2.Bottom);
            }

            public static bool operator !=(Rect rect1, Rect rect2)
            {
                return !(rect1 == rect2);
            }
            #endregion

            #region METHODS · PUBLIC · NON-STATIC
            public override string ToString()
            {
                if (this == Rect.Empty) { return "Rect {Empty}"; }
                return "Rect { Left : " + this.Left + " / Top : " + this.Top + " / Right : " + this.Right + " / Bottom : " + this.Bottom + " }";
            }
            public override bool Equals(object obj)
            {
                if (!(obj is System.Windows.Rect)) { return false; }
                return (this == (Rect)obj);
            }

            public override int GetHashCode()
            {
                return this.Left.GetHashCode() + this.Top.GetHashCode() + this.Right.GetHashCode() + this.Bottom.GetHashCode();
            }
            #endregion
        }
        #endregion

        #region CLASS · PUBLIC · NON-STATIC
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MonitorInfo
        {
            #region FIELDS · PUBLIC · NON-STATIC · NON-READONLY
            public int cbSize;
            public Rect rcMonitor;
            public Rect rcWork;
            public int dwFlags;
            #endregion

            #region CONSTRUCTORS · PUBLIC · NON-STATIC
            public MonitorInfo()
            {
                this.cbSize = Marshal.SizeOf(typeof(MonitorInfo));
                this.rcMonitor = new Rect();
                this.rcWork = new Rect();
                this.dwFlags = 0;
            }
            #endregion

        }
        #endregion
    }
}