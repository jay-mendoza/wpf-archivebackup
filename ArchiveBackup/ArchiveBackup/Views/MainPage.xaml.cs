//-----------------------------------------------------------------------
// <copyright file="MainPage.xaml.cs" company="Jay Bautista Mendoza">
//     Copyright (c) Jay Bautista Mendoza. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace ArchiveBackup.Views
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using ArchiveBackup.Services;
    using JayWpf.Services.Extensions;
    using SevenZip;

    /// <summary>Interaction logic for MainPage.</summary>
    public partial class MainPage : Page
    {
        #region FIELDS │ PRIVATE │ NON-STATIC │ NON-READONLY

        /// <summary>Declare an instance of Archive Backup class.</summary>
        private ArchiveBackupService archiveBackupService;

        /// <summary>Declare an instance of Folder Browser Dialog class.</summary>
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;

        /// <summary>List of invalid path characters.</summary>
        /// <remarks>Code is used only for optimization.</remarks>
        private char[] invalidPathChars;

        /// <summary>Default path for the targets.</summary>
        private string defaultTargetPath;

        /// <summary>Declare an instance of list of favorites.</summary>
        private List<Favorite> favorites;

        #endregion

        #region CONSTRUCTORS │ PUBLIC │ NON-STATIC

        /// <summary>Initializes a new instance of the <see cref="MainPage" /> class.</summary>
        public MainPage()
        {
            this.InitializeComponent();
            this.archiveBackupService = new ArchiveBackupService();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.invalidPathChars = Path.GetInvalidPathChars();
            this.favorites = this.GetAllFavorites();

            this.defaultTargetPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            this.TargetOnePathTextBox.Text = this.defaultTargetPath;
            this.TargetTwoPathTextBox.Text = this.defaultTargetPath;

            this.ArchiveNoneRadioButton.IsChecked = true;

            this.InitializeFavorites();
            this.SetVisibilityBasedOnPaths();
        }

        #endregion

        #region METHODS │ PRIVATE │ NON-STATIC

        /// <summary>Load all favorites from the XML file.</summary>
        /// <returns>List of all favorites.</returns>
        private List<Favorite> GetAllFavorites()
        {
            FavoritesService favoritesService = new FavoritesService();
            Favorites favorites = favoritesService.LoadFavorites();

            return favorites.Root;
        }

        /// <summary>Draw the favorites controls on the WPF page.</summary>
        private void InitializeFavorites()
        {
            int favoritesCount = this.favorites.Count;

            if (favoritesCount == 0)
            {
                this.FavoritesWrapPanel.Visibility = System.Windows.Visibility.Collapsed;
            }

            for (int index = 0; index < favoritesCount; index++)
            {
                Button button = new Button();
                button.Content = this.favorites[index].Name;
                button.Style = this.FindResource("FavoriteButtonStyle") as Style;
                button.Click += this.AutoButton_Click;

                this.FavoritesWrapPanel.Children.Insert(index + 1, button);
            }
        }

        /// <summary>Checks a file path string for validity.</summary>
        /// <param name="path">File path string to validate.</param>
        /// <returns>True if valid, otherwise False.</returns>
        private bool IsValidPath(string path)
        {
            foreach (char invalid in this.invalidPathChars)
            {
                if (path.Any(m => m == invalid))
                {
                    return false;
                }
            }

            return true;
        }
              
        /// <summary>Show application execution status.</summary>
        /// <param name="status">Status text to show on the TextBox.</param>
        private void ShowStatus(string status)
        {
            Action action = () => this.StatusTextBox.Text = status;
            Dispatcher.Invoke(action);
        }

        /// <summary>Execute a copy process.</summary>
        /// <param name="sources">Source files to copy.</param>
        /// <param name="targets">Target directories to copy to.</param>
        private void StartCopy(List<string> sources, List<string> targets)
        {
            try
            {
                string copyReturn = targets[0] == targets[1]
                    ? this.archiveBackupService.CopyFiles(sources, targets[0])
                    : this.archiveBackupService.CopyFiles(sources, targets);

                if (!string.IsNullOrEmpty(copyReturn))
                {
                    this.ShowStatus(string.Concat("SERVICE ERROR │ ", copyReturn));
                }
                else
                {
                    this.ShowStatus("Successfully copied all the files!".ToUpper());
                }
            }
            catch (Exception exception)
            {
                this.ShowStatus(string.Concat("ERROR │ ", exception.Message));
            }
        }

         /// <summary>Execute an archive process.</summary>
        /// <param name="sources">Source folders to archive.</param>
        /// <param name="targets">Target directories to create archive in.</param>
        /// <param name="compressionLevel">Compression level to use in archive.</param>
        private void StartArchive(List<string> sources, List<string> targets, SevenZip.CompressionLevel compressionLevel)
        {
            try
            {
                string archiveReturn = targets[0].IsPathEqual(targets[1])
                    ? this.archiveBackupService.ArchiveFolder(sources, targets[0], compressionLevel)
                    : this.archiveBackupService.ArchiveFolder(sources, targets, compressionLevel);

                if (!string.IsNullOrEmpty(archiveReturn))
                {
                    this.ShowStatus(string.Concat("SERVICE ERROR │ ", archiveReturn));
                }
                else
                {
                    this.ShowStatus("Successfully archived the folder!".ToUpper());
                }
            }
            catch (Exception exception)
            {
                this.ShowStatus(string.Concat("ERROR │ ", exception.Message));
            }
        }

        /// <summary>Set control visibilities based on validity of path(s).</summary>
        private void SetVisibilityBasedOnPaths()
        {
            string pathOne = this.TargetOnePathTextBox.Text;
            string pathTwo = this.TargetTwoPathTextBox.Text;

            string status = string.Empty;
            bool showDropBox = true;

            if (!this.IsValidPath(pathOne) || !Directory.Exists(pathOne))
            {
                status = "NOTE │ Invalid directory for target one.";
                showDropBox = false;
            }

            if (!this.IsValidPath(pathTwo) || !Directory.Exists(pathTwo))
            {
                status = string.Concat(status, showDropBox ? string.Empty : System.Environment.NewLine, "NOTE │ Invalid directory for target two.");
                showDropBox = false;
            }
            
            if (!showDropBox)
            {
                this.DropGrid.Visibility = Visibility.Collapsed;
                this.ShowStatus(status);
            }
            else
            {
                this.DropGrid.Visibility = Visibility.Visible;
                this.ShowStatus(pathOne.ToUpper() == pathTwo.ToUpper() ? "NOTE │ Directories are the same." : string.Empty);
            }
        }

        #endregion

        #region METHODS (EVENTS) │ PRIVATE │ NON-STATIC

        /// <summary>Drop event for "Drop" Rectangle.</summary>
        /// <param name="sender">Object "sender".</param>
        /// <param name="e">DragEventArgs "e".</param>
        private void DropRectangle_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                try
                {
                    List<string> sources = ((string[])e.Data.GetData(DataFormats.FileDrop, true)).ToList<string>();
                    List<string> targets = new List<string>() { this.TargetOnePathTextBox.Text, this.TargetTwoPathTextBox.Text };

                    if ((bool)this.ArchiveNoneRadioButton.IsChecked)
                    {
                        this.ShowStatus("PLEASE WAIT │ Copying selected files and folders...");
                        new Thread(() => this.StartCopy(sources, targets)).Start();
                    }
                    if ((bool)this.ArchiveStoreRadioButton.IsChecked)
                    {
                        this.ShowStatus("PLEASE WAIT │ Archiving (store) selected folder...");
                        new Thread(() => this.StartArchive(sources, targets, SevenZip.CompressionLevel.Store)).Start();
                    }
                    else if ((bool)this.ArchiveNormalRadioButton.IsChecked)
                    {
                        this.ShowStatus("PLEASE WAIT │ Archiving (normal) selected folder...");
                        new Thread(() => this.StartArchive(sources, targets, SevenZip.CompressionLevel.Normal)).Start();
                    }
                    else if ((bool)this.ArchiveUltraRadioButton.IsChecked)
                    {
                        this.ShowStatus("PLEASE WAIT │ Archiving (ultra) selected folder...");
                        new Thread(() => this.StartArchive(sources, targets, SevenZip.CompressionLevel.Ultra)).Start();
                    }
                }
                catch (Exception exception)
                {
                    this.ShowStatus(string.Concat("ERROR │ ", exception.Message));
                }
            }
        }

        /// <summary>Drop event for "Target-One-Drop" TextBlock.</summary>
        /// <param name="sender">Object "sender".</param>
        /// <param name="e">DragEventArgs "e".</param>
        private void TargetOneDropTextBlock_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                try
                {
                    string source = (e.Data.GetData(DataFormats.FileDrop, true) as string[])[0];
                    this.TargetOnePathTextBox.Text = this.archiveBackupService.IsFolder(source) ? source : Path.GetDirectoryName(source);
                }
                catch (Exception exception)
                {
                    this.ShowStatus(string.Concat("ERROR │ ", exception.Message));
                }
            }
        }

        /// <summary>Text-Changed event for "Target-One-Path" TextBox.</summary>
        /// <param name="sender">Object "sender".</param>
        /// <param name="e">TextChangedEventArgs "e".</param>
        private void TargetOnePathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.SetVisibilityBasedOnPaths();
        }
        
        /// <summary>Click event for "Target-One-Browse" Button.</summary>
        /// <param name="sender">Object "sender".</param>
        /// <param name="e">RoutedEventArgs "e".</param>
        private void TargetOneBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.DialogResult dialogResult = this.folderBrowserDialog.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                this.TargetOnePathTextBox.Text = this.folderBrowserDialog.SelectedPath;
            }
        }

        /// <summary>Drop event for "Target-Two-Drop" TextBlock.</summary>
        /// <param name="sender">Object "sender".</param>
        /// <param name="e">DragEventArgs "e".</param>
        private void TargetTwoDropTextBlock_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                try
                {
                    string source = (e.Data.GetData(DataFormats.FileDrop, true) as string[])[0];
                    this.TargetTwoPathTextBox.Text = this.archiveBackupService.IsFolder(source) ? source : Path.GetDirectoryName(source);
                }
                catch (Exception exception)
                {
                    this.ShowStatus(string.Concat("ERROR │ ", exception.Message));
                }
            }
        }

        /// <summary>Text-Changed event for "Target-Two-Path" TextBox.</summary>
        /// <param name="sender">Object "sender".</param>
        /// <param name="e">TextChangedEventArgs "e".</param>
        private void TargetTwoPathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.SetVisibilityBasedOnPaths();
        }

        /// <summary>Click event for "Target-Two-Browse" Button.</summary>
        /// <param name="sender">Object "sender".</param>
        /// <param name="e">RoutedEventArgs "e".</param>
        private void TargetTwoBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.DialogResult dialogResult = this.folderBrowserDialog.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                this.TargetTwoPathTextBox.Text = this.folderBrowserDialog.SelectedPath;
            }
        }

        /// <summary>Click event for all the "Auto-" Buttons.</summary>
        /// <param name="sender">Object "sender".</param>
        /// <param name="e">RoutedEventArgs "e".</param>
        private void AutoButton_Click(object sender, RoutedEventArgs e)
        {
            string content = (sender as Button).Content.ToString().Trim().ToUpper();
            string text1 = this.defaultTargetPath;
            string text2 = this.defaultTargetPath;

            try
            {
                Favorite favorite = this.favorites.Where(f => f.Name.Trim().ToUpper() == content.Trim().ToUpper()).SingleOrDefault();
                text1 = string.IsNullOrEmpty(favorite.DirectoryOne) ? text1 : favorite.DirectoryOne;
                text2 = string.IsNullOrEmpty(favorite.DirectoryTwo) ? text2 : favorite.DirectoryTwo;
            }
            catch (Exception exception)
            {
                this.ShowStatus(string.Concat("ERROR │ ", exception.Message));
            }

            this.TargetOnePathTextBox.Text = text1;
            this.TargetTwoPathTextBox.Text = text2;
        }

        #endregion
    }
}
