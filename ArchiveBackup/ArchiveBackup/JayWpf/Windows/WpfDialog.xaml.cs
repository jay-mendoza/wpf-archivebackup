// ······································································//
// <copyright file="WpfDialog.xaml.cs" company="Jay Bautista Mendoza">   //
//     Copyright (c) Jay Bautista Mendoza. All rights reserved.          //
//     THIS IS PART OF MY PERSONAL OPEN SOURCE WPF WINDOW TEMPLATE.      //
//     THIS IS NOT PRIVATE PROPERTY. FEEL FREE TO MODIFY OR USE IT.      //
// </copyright>                                                          //
// ······································································//

namespace JayWpf.Windows
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Shapes;
    using JayWpf.Services;
    using JayWpf.Services.Configuration;

    /// <summary>Interaction logic for WPF-Dialog XAML.</summary>
    public partial class WpfDialog : Window
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

        #endregion

        #region CONSTRUCTORS │ PUBLIC │ NON-STATIC

        /// <summary>Initializes a new instance of the <see cref="WpfDialog" /> class.</summary>
        /// <param name="frameSource">Frame source (like WPF Page) to put inside the dialog.</param>
        public WpfDialog(string frameSource)
        {
            this.windowsService = new WindowService(this);
            this.windowConfig = new WindowConfig();
            this.InitializeComponent();
            
            this.MainFrame.Source = new Uri("../../" + frameSource, UriKind.Relative);
            
            try
            {
                this.windowConfig.Load();

                this.SetTheme();

                this.Width = this.windowConfig.WindowSize.Width;
                this.Height = this.windowConfig.WindowSize.Height;

                this.Top = this.windowConfig.WindowPosition.Top + 10;
                this.Left = this.windowConfig.WindowPosition.Left + 10;

                string ontop = ConfigurationManager.AppSettings.Get("ontop");

                this.Topmost = this.windowConfig.WindowOnTop;
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
            set { this.WindowIconTextBlock.FontFamily = new System.Windows.Media.FontFamily(value); }
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

        /// <summary>Click event for the Close button..</summary>
        /// <param name="sender">Object 'sender'.</param>
        /// <param name="e">RoutedEventArgs 'e'.</param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.windowsService.CloseWindow();
        }
        
        /// <summary>Mouse left button down event for the Title Bar Logo grid.</summary>
        /// <param name="sender">Object 'sender'.</param>
        /// <param name="e">MouseButtonEventArgs 'e'.</param>
        private void TitleBarLogo_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ////if (e.ClickCount == 2) { this.windowsService.CloseWindow(); }
            this.TitleBarLogo.ContextMenu.PlacementTarget = this.TitleBarLogo;
            this.TitleBarLogo.ContextMenu.IsOpen = true;
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
                    this.windowsService.ChangeWindowState();
                }
                else
                {
                    this.windowsService.DragWindow();
                }
            }
        }
        #endregion

        #region METHODS │ PRIVATE │ NON-STATIC

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

        #endregion
    }
}
