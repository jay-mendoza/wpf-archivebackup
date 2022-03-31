// ······································································//
// <copyright file="WpfWindow.xaml.cs" company="Jay Bautista Mendoza">   //
//     Copyright (c) Jay Bautista Mendoza. All rights reserved.          //
//     THIS IS PART OF MY PERSONAL OPEN SOURCE WPF WINDOW TEMPLATE.      //
//     THIS IS NOT PRIVATE PROPERTY. FEEL FREE TO MODIFY OR USE IT.      //
// </copyright>                                                          //
// ······································································//

namespace JayWpf.Windows
{
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Shapes;
    using JayWpf.Services;
    using JayWpf.Services.Configuration;

    /// <summary>Interaction logic for WPF-Window XAML.</summary>
    public partial class WpfWindow : Window
    {
        #region CONSTANT FIELDS │ PRIVATE │ NON-STATIC
        
        /// <summary>Path of theme shades resource definition.</summary>
        private const string ThemeShadesPath = "../JayWpf/Themes/Shades/";
        
        /// <summary>Path of theme colors resource definition.</summary>
        private const string ThemeColorsPath = "../JayWpf/Themes/Colors/";
        
        #endregion
        
        #region FIELDS │ PRIVATE │ NON-STATIC │ NON-READONLY
        
        /// <summary>Declare an instance of the WindowConfig.</summary>
        private WindowConfig windowConfig;

        /// <summary>Declare an instance of the Window Service.</summary>
        private WindowService windowsService;

        /// <summary>Double value for the current restored window width.</summary>
        private double restoredWidth;
        
        /// <summary>Double value for the current restored window height.</summary>
        private double restoredHeight;

        /// <summary>Used to store the expanded height when collapsing.</summary>
        private double expandedHeight;

        /// <summary>Used to store the expanded width when collapsing.</summary>
        private double expandedWidth;
        #endregion

        #region CONSTRUCTORS │ PUBLIC │ NON-STATIC

        /// <summary>Initializes a new instance of the <see cref="WpfWindow" /> class.</summary>
        /// <param name="frameSource">Frame source (like WPF Page) to put inside the Window.</param>
        public WpfWindow(string frameSource)
        {
            this.windowsService = new WindowService(this);
            this.windowConfig = new WindowConfig();
            this.InitializeComponent();

            this.MainFrame.Source = new Uri("../../" + frameSource, UriKind.Relative);
            this.DrawMaximizeButton();

            try
            {
                this.windowConfig.Load();

                this.SetTheme();

                this.Width = this.windowConfig.WindowSize.Width;
                this.Height = this.windowConfig.WindowSize.Height;

                this.Top = this.windowConfig.WindowPosition.Top;
                this.Left = this.windowConfig.WindowPosition.Left;

                string ontop = ConfigurationManager.AppSettings.Get("ontop");

                this.Topmost = this.windowConfig.WindowOnTop;
                this.OnTopToggle.IsChecked = this.Topmost;
            }
            catch
            {
                this.DefaultStart();
            }
        }

        #endregion

        #region PROPERTIES │ PUBLIC │ NON-STATIC

        /// <summary>Gets or sets the Window's icon text.</summary>
        public string IconText
        {
            get { return this.WindowIconTextBlock.Text; }
            set { this.WindowIconTextBlock.Text = value; }
        }

        /// <summary>Gets or sets the Window's icon text font.</summary>
        public string IconTextFont
        {
            get { return this.WindowIconTextBlock.FontFamily.ToString(); }
            set { this.WindowIconTextBlock.FontFamily = new System.Windows.Media.FontFamily(new Uri("pack://application:,,,/"), value); }
        }

        /// <summary>Gets or sets the Window's icon text size.</summary>
        public double IconTextSize
        {
            get { return this.WindowIconTextBlock.FontSize; }
            set { this.WindowIconTextBlock.FontSize = value; }
        }

        #endregion

        #region EVENTS │ PRIVATE │ NON-STATIC

        /// <summary>Preview mouse down event for the Resizer rectangle.</summary>
        /// <param name="sender">Object 'sender'.</param>
        /// <param name="e">MouseButtonEventArgs 'e'.</param>
        private void Resizer_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.windowsService.ResizeWindow(sender as Rectangle);
        }

        /// <summary>Mouse enter event for the Resizer rectangle.</summary>
        /// <param name="sender">Object 'sender'.</param>
        /// <param name="e">MouseEventArgs 'e'.</param>
        private void Resizer_MouseEnter(object sender, MouseEventArgs e)
        {
            this.windowsService.SetCursor(sender as Rectangle);
        }

        /// <summary>Mouse leave event for the Resizer rectangle.</summary>
        /// <param name="sender">Object 'sender'.</param>
        /// <param name="e">MouseEventArgs 'e'.</param>
        private void Resizer_MouseLeave(object sender, MouseEventArgs e)
        {
            this.windowsService.ResetCursor();
        }

        /// <summary>State changed event for the Window window.</summary>
        /// <param name="sender">Object 'sender'.</param>
        /// <param name="e">EventArgs 'e'.</param>
        private void Window_StateChanged(object sender, EventArgs e)
        {
            this.DrawMaximizeButton();
        }

        /// <summary>Click event for the Close button..</summary>
        /// <param name="sender">Object 'sender'.</param>
        /// <param name="e">RoutedEventArgs 'e'.</param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.windowsService.CloseWindow();
        }

        /// <summary>Click event for the Maximize button.</summary>
        /// <param name="sender">Object 'sender'.</param>
        /// <param name="e">RoutedEventArgs 'e'.</param>
        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.restoredWidth = this.Width;
            this.restoredHeight = this.Height;
            this.windowsService.MaximizeWindow();
        }

        /// <summary>Click event for the Minimize button.</summary>
        /// <param name="sender">Object 'sender'.</param>
        /// <param name="e">RoutedEventArgs 'e'.</param>
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.windowsService.MinimizeWindow();
        }

        /// <summary>Click event for the Restore button.</summary>
        /// <param name="sender">Object 'sender'.</param>
        /// <param name="e">RoutedEventArgs 'e'.</param>
        private void RestoreButton_Click(object sender, RoutedEventArgs e)
        {
            this.windowsService.RestoreWindow();
        }

        /// <summary>Click event for the Theme Shade button.</summary>
        /// <param name="sender">Object 'sender'.</param>
        /// <param name="e">RoutedEventArgs 'e'.</param>
        private void ThemeShadeButton_Click(object sender, RoutedEventArgs e)
        {
            this.SetThemeShade();
            this.SaveWindowConfigurations();
        }

        /// <summary>Click event for the Theme Color button.</summary>
        /// <param name="sender">Object 'sender'.</param>
        /// <param name="e">RoutedEventArgs 'e'.</param>
        private void ThemeColorButton_Click(object sender, RoutedEventArgs e)
        {
            this.SetThemeColor();
            this.SaveWindowConfigurations();
        }

        /// <summary>Checked event for the On Top Toggle check box.</summary>
        /// <param name="sender">Object 'sender'.</param>
        /// <param name="e">RoutedEventArgs 'e'.</param>
        private void OnTopToggle_Checked(object sender, RoutedEventArgs e)
        {
            this.Topmost = true;
            this.SaveWindowConfigurations();
        }

        /// <summary>Unchecked event for the On Top Toggle check box.</summary>
        /// <param name="sender">Object 'sender'.</param>
        /// <param name="e">RoutedEventArgs 'e'.</param>
        private void OnTopToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            this.Topmost = false;
            this.SaveWindowConfigurations();
        }

        /// <summary>Mouse left button down event for the Title Bar Logo grid.</summary>
        /// <param name="sender">Object 'sender'.</param>
        /// <param name="e">MouseButtonEventArgs 'e'.</param>
        private void TitleBarLogo_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ////if (TitleBarLogo.ContextMenu != null)
            ////{
            ////    this.TitleBarLogo.ContextMenu.PlacementTarget = this.TitleBarLogo;
            ////    this.TitleBarLogo.ContextMenu.IsOpen = true;
            ////}
        }

        /// <summary>Preview mouse down event for the TitleBarLogo Grid.</summary>
        /// <param name="sender">Object 'sender'.</param>
        /// <param name="e">MouseButtonEventArgs 'e'.</param>
        private void TitleBarLogo_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (e.ClickCount == 2)
                {
                    if (this.LL.IsEnabled)
                    {
                        this.CollapseWindow();
                    }
                    ////else
                    ////{
                    ////    this.ExpandWindow();
                    ////}
                }
            }
        }

        /// <summary>Preview mouse down event for the Dragger rectangle.</summary>
        /// <param name="sender">Object 'sender'.</param>
        /// <param name="e">MouseButtonEventArgs 'e'.</param>
        private void Dragger_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (e.ClickCount == 2)
                {
                    if (!this.LL.IsEnabled)
                    {
                        this.ExpandWindow();
                    }
                    else
                    {
                        this.windowsService.ChangeWindowState();
                    }
                }
                else
                {
                    this.windowsService.DragWindow();
                }
            }
        }

        /// <summary>Preview mouse down event for the Resizer rectangle.</summary>
        /// <param name="sender">Object 'sender'.</param>
        /// <param name="e">CancelEventArgs 'e'.</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.CollapseButton.Visibility == Visibility.Collapsed)
            {
                e.Cancel = true;
            }

            this.SaveWindowConfigurations();
        }

        /// <summary>Click event for the Collapse button.</summary>
        /// <param name="sender">Object 'sender'.</param>
        /// <param name="e">RoutedEventArgs 'e'.</param>
        private void CollapseButton_Click(object sender, RoutedEventArgs e)
        {
            this.CollapseWindow();
        }

        /// <summary>Click event for the Expand button.</summary>
        /// <param name="sender">Object 'sender'.</param>
        /// <param name="e">RoutedEventArgs 'e'.</param>
        private void ExpandButton_Click(object sender, RoutedEventArgs e)
        {
            this.ExpandWindow();
        }
        #endregion

        #region METHODS │ PRIVATE │ NON-STATIC

        /// <summary>Saves the current Window configuration to XML.</summary>
        private void SaveWindowConfigurations()
        {
            this.windowConfig.WindowPosition.Top = this.Top;
            this.windowConfig.WindowPosition.Left = this.Left;

            this.windowConfig.WindowOnTop = this.Topmost;
            
            switch (this.WindowState)
            {
                case WindowState.Normal:
                    this.windowConfig.WindowSize.Width = this.Width;
                    this.windowConfig.WindowSize.Height = this.Height;
                    break;
                case WindowState.Maximized:
                    this.windowConfig.WindowSize.Width = this.restoredWidth;
                    this.windowConfig.WindowSize.Height = this.restoredHeight;
                    break;
                default: goto case WindowState.Normal;
            }

            this.windowConfig.Save();
        }

        /// <summary>Set default start parameters.</summary>
        private void DefaultStart()
        {
            this.windowConfig.WindowTheme.Color = (WindowConfig.Theme.Colors)0;
            this.windowConfig.WindowTheme.Shade = (WindowConfig.Theme.Shades)0;

            this.SetTheme();
            this.Width = 400;
            this.Height = 240;
            this.Top = 10;
            this.Left = 10;
        }

        /// <summary>Switches between maximize and restore windows button.</summary>
        private void DrawMaximizeButton()
        {
            ContextMenu cm = this.Resources["WindowControlContextMenu"] as ContextMenu;

            switch (this.WindowState)
            {
                case WindowState.Normal:
                    this.RestoreButton.Visibility = Visibility.Collapsed;
                    this.MaximizeButton.Visibility = Visibility.Visible;
                    ((MenuItem)LogicalTreeHelper.FindLogicalNode(cm, "RestoreContext")).IsEnabled = false;
                    ((MenuItem)LogicalTreeHelper.FindLogicalNode(cm, "MaximizeContext")).IsEnabled = true;
                    this.CollapseExpandGrid.Visibility = Visibility.Visible;
                    break;
                case WindowState.Maximized:
                    this.MaximizeButton.Visibility = Visibility.Collapsed;
                    this.RestoreButton.Visibility = Visibility.Visible;
                    ((MenuItem)LogicalTreeHelper.FindLogicalNode(cm, "MaximizeContext")).IsEnabled = false;
                    ((MenuItem)LogicalTreeHelper.FindLogicalNode(cm, "RestoreContext")).IsEnabled = true;
                    this.CollapseExpandGrid.Visibility = Visibility.Hidden;
                    break;
                default: break;
            }
        }

        /// <summary>Sets or resets the current window theme shade and color.</summary>
        private void SetTheme()
        {
            string shade = string.Concat(ThemeShadesPath, this.windowConfig.WindowTheme.Shade.ToString(), ".xaml");
            string color = string.Concat(ThemeColorsPath, this.windowConfig.WindowTheme.Color.ToString(), ".xaml");

            Application.Current.Resources.MergedDictionaries.Clear();

            ResourceDictionary shadeResource = new ResourceDictionary();
            shadeResource.Source = new Uri(shade, UriKind.Relative);
            Application.Current.Resources.MergedDictionaries.Add(shadeResource);

            ResourceDictionary colorResource = new ResourceDictionary();
            colorResource.Source = new Uri(color, UriKind.Relative);
            Application.Current.Resources.MergedDictionaries.Add(colorResource);
        }

        /// <summary>Cycles through and sets the window theme shade.</summary>
        private void SetThemeShade()
        {
            if (this.windowConfig.WindowTheme.Shade.ToString() == Enum.GetNames(typeof(WindowConfig.Theme.Shades)).Last())
            {
                this.windowConfig.WindowTheme.Shade = (WindowConfig.Theme.Shades)0;
            }
            else
            {
                this.windowConfig.WindowTheme.Shade = (WindowConfig.Theme.Shades)((int)this.windowConfig.WindowTheme.Shade + 1);
            }

            this.SetTheme();
        }

        /// <summary>Cycles through and sets the window theme color.</summary>
        private void SetThemeColor()
        {
            if (this.windowConfig.WindowTheme.Color.ToString() == Enum.GetNames(typeof(WindowConfig.Theme.Colors)).Last())
            {
                this.windowConfig.WindowTheme.Color = (WindowConfig.Theme.Colors)0;
            }
            else
            {
                this.windowConfig.WindowTheme.Color = (WindowConfig.Theme.Colors)((int)this.windowConfig.WindowTheme.Color + 1);
            }

            this.SetTheme();
        }

        /// <summary>Collapse the Window.</summary>
        /// <param name="isCollapsed">[OPTIONAL PARAMETER] [Default=TRUE] Set to FALSE to expand Window instead.</param>
        private void CollapseWindow(bool isCollapsed = true)
        {
            this.LL.IsEnabled =
            this.TL.IsEnabled =
            this.TT.IsEnabled =
            this.TR.IsEnabled =
            this.RR.IsEnabled =
            this.BR.IsEnabled =
            this.BB.IsEnabled =
            this.BL.IsEnabled = !isCollapsed;

            this.Dragger.ContextMenu =
            this.TitleBarLogo.ContextMenu = isCollapsed ? null : (ContextMenu)this.Resources["WindowControlContextMenu"];

            this.TitleBarLogo.Margin = isCollapsed ? new Thickness(2, 0, 0, 0) : new Thickness(5, 0, 5, 0);

            Grid.SetColumn(this.Dragger, isCollapsed ? 0 : 2);
            this.Dragger.Cursor = isCollapsed ? Cursors.SizeAll : Cursors.Arrow;

            this.ExpandButton.Visibility = isCollapsed ? Visibility.Visible : Visibility.Collapsed;

            this.CollapseButton.Visibility =
            this.TitleBar.Visibility =
            this.WindowControlStackPanel.Visibility =
            this.WindowOptionsStackPanel.Visibility = isCollapsed ? Visibility.Collapsed : Visibility.Visible;

            if (isCollapsed)
            {
                this.expandedHeight = this.Height;
                this.expandedWidth = this.Width;
            }

            this.Height = isCollapsed ? 33 : this.expandedHeight;
            this.Width = isCollapsed ? 66 : this.expandedWidth;
        }

        /// <summary>Expand the Window.</summary>
        private void ExpandWindow()
        {
            this.CollapseWindow(false);
        }
        #endregion
    }
}
