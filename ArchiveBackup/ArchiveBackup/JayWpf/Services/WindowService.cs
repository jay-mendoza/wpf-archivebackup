// ······································································//
// <copyright file="WindowService.cs" company="Jay Bautista Mendoza">    //
//     Copyright (c) Jay Bautista Mendoza. All rights reserved.          //
//     THIS IS PART OF MY PERSONAL OPEN SOURCE WPF WINDOW TEMPLATE.      //
//     THIS IS NOT PRIVATE PROPERTY. FEEL FREE TO MODIFY OR USE IT.      //
// </copyright>                                                          //
// ······································································//

namespace JayWpf.Services
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Interop;
    using System.Windows.Media;
    using System.Windows.Shapes;

    public class WindowService
    {
        #region CONSTANT FIELDS · PRIVATE · NON-STATIC
        private const int WM_SYSCOMMAND = 0x112;
        #endregion

        #region FIELDS · PRIVATE · NON-STATIC · NON-READONLY
        private HwndSource hwndSource;
        private Window activeWindow;
        #endregion

        #region CONSTRUCTORS · PUBLIC · NON-STATIC
        /// <summary></summary>
        public WindowService(Window activeWindow)
        {
            this.activeWindow = activeWindow as Window;
            this.activeWindow.SourceInitialized += new EventHandler(InitializeWindowSource);

        }
        #endregion

        #region METHODS · PUBLIC · NON-STATIC
        /// <summary>This function controls the resize mechanisms of the window.</summary>
        /// <param name="sender">The rectangle that is being dragged.</param>
        public void ResizeWindow(Rectangle sender)
        {
            switch (sender.Name.ToUpper())
            {
                case "LL": this.activeWindow.Cursor = Cursors.SizeWE; this.ResizeWindow(1); break;
                case "RR": this.activeWindow.Cursor = Cursors.SizeWE; this.ResizeWindow(2); break;
                case "TT": this.activeWindow.Cursor = Cursors.SizeNS; this.ResizeWindow(3); break;
                case "TL": this.activeWindow.Cursor = Cursors.SizeNWSE; this.ResizeWindow(4); break;
                case "TR": this.activeWindow.Cursor = Cursors.SizeNESW; this.ResizeWindow(5); break;
                case "BB": this.activeWindow.Cursor = Cursors.SizeNS; this.ResizeWindow(6); break;
                case "BL": this.activeWindow.Cursor = Cursors.SizeNESW; this.ResizeWindow(7); break;
                case "BR": this.activeWindow.Cursor = Cursors.SizeNWSE; this.ResizeWindow(8); break;
                default: break;
            }
        }

        /// <summary>This function controls which cursor to display when previewing resize.</summary>
        /// <param name="sender">The rectangle that is being hovered on.</param>
        public void SetCursor(Rectangle sender)
        {
            switch (sender.Name.ToUpper())
            {
                case "TT": this.activeWindow.Cursor = Cursors.SizeNS; break;
                case "BB": this.activeWindow.Cursor = Cursors.SizeNS; break;
                case "LL": this.activeWindow.Cursor = Cursors.SizeWE; break;
                case "RR": this.activeWindow.Cursor = Cursors.SizeWE; break;
                case "TL": this.activeWindow.Cursor = Cursors.SizeNWSE; break;
                case "TR": this.activeWindow.Cursor = Cursors.SizeNESW; break;
                case "BL": this.activeWindow.Cursor = Cursors.SizeNESW; break;
                case "BR": this.activeWindow.Cursor = Cursors.SizeNWSE; break;
                default: break;
            }
        }

        /// <summary>This function resets the cursor to normal.</summary>
        public void ResetCursor()
        {
            if (Mouse.LeftButton != MouseButtonState.Pressed)
            { 
                this.activeWindow.Cursor = Cursors.Arrow;
            }
        }

        /// <summary>This function closes the window.</summary>
        public void CloseWindow()
        {
            this.activeWindow.Close();
        }

        /// <summary>This function controls the drag mechanism of the window.</summary>
        public void DragWindow()
        {
            this.activeWindow.DragMove();
        }

        /// <summary>This function minimizes the window.</summary>
        public void MinimizeWindow()
        {
            this.activeWindow.WindowState = WindowState.Minimized;
        }

        /// <summary>This function maximizes the window.</summary>
        public void MaximizeWindow()
        {
            this.activeWindow.WindowState = WindowState.Maximized;
        }

        /// <summary>This function restores the window.</summary>
        public void RestoreWindow()
        {
            this.activeWindow.WindowState = WindowState.Normal;
        }

        /// <summary>This function maximizes or restores the window.</summary>
        public void ChangeWindowState()
        {
            switch (this.activeWindow.WindowState)
            {
                case WindowState.Normal: this.activeWindow.WindowState = WindowState.Maximized; break;
                case WindowState.Maximized: this.activeWindow.WindowState = WindowState.Normal; break;
                default: break;
            }
        }
        #endregion

        #region METHODS · PRIVATE · STATIC
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        #endregion

        #region METHODS · PRIVATE · NON-STATIC
        private void InitializeWindowSource(object sender, EventArgs e)
        {
            this.hwndSource = PresentationSource.FromVisual((Visual)sender) as HwndSource;
            this.hwndSource.AddHook(new HwndSourceHook(WndProc));
        }

        private void ResizeWindow(int direction)
        {
            WindowService.SendMessage(hwndSource.Handle, WM_SYSCOMMAND, (IntPtr)(61440 + direction), IntPtr.Zero);
        }
        
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == 0x0024) /*MAXIMIZE FIX*/
            {
                WindowMaximizeFix.WmGetMinMaxInfo(hwnd, lParam);
                handled = true;
            }

            return IntPtr.Zero;
        }
        #endregion
    }
}