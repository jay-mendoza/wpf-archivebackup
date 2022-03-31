//-----------------------------------------------------------------------
// <copyright file="ArchiveBackupService.cs" company="Jay Bautista Mendoza">
//     Copyright (c) Jay Bautista Mendoza. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace ArchiveBackup.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows.Documents;
    using SevenZip;

    /// <summary>Service class of Archive Backup.</summary>
    public class ArchiveBackupService
    {
        /// <summary>Determine if a given path is a folder or not.</summary>
        /// <param name="path">Path to determine.</param>
        /// <returns>True if the path is a folder, otherwise, False.</returns>
        public bool IsFolder(string path)
        {
            return (File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory;
        }

        /// <summary>Execute a copy process.</summary>
        /// <param name="sources">Source files and folders to copy.</param>
        /// <param name="target">Single target directory to copy to.</param>
        /// <returns>Null if success, otherwise, the error.</returns>
        public string CopyFiles(List<string> sources, string target)
        {
            return this.CopyFiles(sources, new List<string>() { target });
        }

        /// <summary>Execute a copy process.</summary>
        /// <param name="sources">Source files and folders to copy.</param>
        /// <param name="targets">Target directories to copy to.</param>
        /// <returns>Null if success, otherwise, the error.</returns>
        public string CopyFiles(List<string> sources, List<string> targets)
        {
            try
            {
                foreach (string sourcePath in sources)
                {
                    string name = Path.GetFileName(sourcePath);

                    List<string> targetPaths = new List<string>();

                    for (int count = 0; count < targets.Count; count++)
                    {
                        if (!Directory.Exists(targets[count]))
                        {
                            Directory.CreateDirectory(targets[count]);
                        }

                        targetPaths.Add(Path.Combine(targets[count], name));
                    }

                    if (this.IsFolder(sourcePath))
                    {
                        this.CopyFiles(this.GetFilesAndFolders(sourcePath), targetPaths);
                    }
                    else
                    {
                        foreach (string newTarget in targetPaths)
                        {
                            File.Copy(sourcePath, newTarget);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                return exception.Message;
            }

            return null;
        }    
    
        public string CopyFile(string source, string target)
        {
            try
            {
                string name = Path.GetFileName(source);

                if (!Directory.Exists(target))
                {
                    Directory.CreateDirectory(target);
                }

                File.Copy(source, Path.Combine(target, name));                
            }
            catch (Exception exception)
            {
                return exception.Message;
            }

            return null;
        }

        /// <summary>Combination of PathGetFiles and Path.GetDirectories.</summary>
        /// <param name="path">Path parameter.</param>
        /// <returns>Combined array of all files and folders.</returns>
        public List<string> GetFilesAndFolders(string path)
        {
            return Directory.GetFiles(path).Concat(Directory.GetDirectories(path)).ToList<string>();
        }

        /// <summary>Execute an archive process.</summary>
        /// <param name="sourceFolders">Source folder to archive.</param>
        /// <param name="target">Single target directory to copy to.</param>
        /// <param name="compressionLevel">Compression level to use in archive.</param>
        /// <returns>Null if success, otherwise, the error.</returns>
        public string ArchiveFolder(List<string> sourceFolders, string target, SevenZip.CompressionLevel compressionLevel)
        {
            return this.ArchiveFolder(sourceFolders, new List<string>() { target }, compressionLevel);
        }

        /// <summary>Execute an archive process.</summary>
        /// <param name="sourceFolders">Source folder to archive.</param>
        /// <param name="targets">Target directories to create archive in.</param>
        /// <param name="compressionLevel">Compression level to use in archive.</param>
        /// <returns>Null if success, otherwise, the error.</returns>
        public string ArchiveFolder(List<string> sourceFolders, List<string> targets, SevenZip.CompressionLevel compressionLevel)
        {
            try
            {
                if (sourceFolders.Count() != 1)
                {
                    return "Please drop exactly ONE folder to archive.";
                }

                string sourceFolder = sourceFolders[0];

                if (!this.IsFolder(sourceFolder))
                {
                    return "Not a folder. Please drop a folder for achiving.";
                }

                List<string> archiveDirectories = targets.ToList<string>();
                List<string> archivePaths = new List<string>();
                string archiveFileName = Path.GetFileName(sourceFolder) + ".7z";

                foreach (string archiveDirectory in archiveDirectories)
                {
                    if (!Directory.Exists(archiveDirectory))
                    {
                        Directory.CreateDirectory(archiveDirectory);
                    }

                    archivePaths.Add(Path.Combine(archiveDirectory, archiveFileName));
                }

                while (archivePaths.Any(x => File.Exists(x)))
                {
                    archiveFileName = string.Format("{0}.7z", archiveFileName);

                    for (int count = 0; count < archiveDirectories.Count; count++)
                    {
                        archivePaths[count] = Path.Combine(archiveDirectories[count], archiveFileName);
                    }
                }

                SevenZip sevenZip = new SevenZip();
                List<string> returns = new List<string>();

                foreach (string archivePath in archivePaths)
                {
                    returns.Add(sevenZip.Archive(sourceFolder, archivePath, compressionLevel));
                }

                if (returns.Any(x => !string.IsNullOrEmpty(x)))
                {
                    string compressionError = "COMPRESSION ERROR";

                    for (int count = 1; count <= returns.Count; count++)
                    {
                        compressionError = string.Format("{0} | {1}: {2}", compressionError, count, returns[count]);
                    }

                    return compressionError;
                }
            }
            catch (Exception exception)
            {
                return exception.Message;
            }

            return null;
        }
    }
}
