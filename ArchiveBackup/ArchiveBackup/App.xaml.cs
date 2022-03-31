//-----------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="Jay Bautista Mendoza">
//     Copyright (c) Jay Bautista Mendoza. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace ArchiveBackup
{
    using System;
    using System.Configuration;
    using System.Windows;
    using System.Windows.Media;
    using JayWpf.Windows;

    /// <summary>Interaction logic for App XAML.</summary>
    public partial class App : Application
    {
        // <summary>Startup event of the main App.</summary>
        /// <param name="sender">Object 'sender'.</param>
        /// <param name="e">StartupEventArgs 'e'.</param>
        private void App_Startup(object sender, StartupEventArgs e)
        {
            WpfWindow mainWindow = new WpfWindow("Views/MainPage.xaml");
            mainWindow.Title = "Archive • Backup";
            mainWindow.IconTextFont = @"/JayWpf/Resources/Fonts/#Wingdings";
;

            mainWindow.IconText = "55";
            mainWindow.IconTextSize = 24;

            mainWindow.Show();
        }
    }
}
