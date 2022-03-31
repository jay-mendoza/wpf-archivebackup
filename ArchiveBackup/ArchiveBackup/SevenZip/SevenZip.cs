//-----------------------------------------------------------------------
// <copyright file="SevenZip.cs" company="Jay Bautista Mendoza">
//     Copyright (c) Jay Bautista Mendoza. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace SevenZip
{
    using System;
    using System.Diagnostics;

    /// <summary>Service class for 7-ZIP.</summary>
    public class SevenZip
    {
        #region FIELDS │ PRIVATE │ NON-STATIC │ NON-READONLY

        /// <summary>Declare an instance pf ProcessStartInfo for the 7-ZIP process.</summary>
        private ProcessStartInfo processInfo;

        #endregion

        #region CONSTRUCTORS │ PUBLIC │ NON-STATIC
        
        /// <summary>Initializes a new instance of the <see cref="SevenZip" /> class.</summary>
        public SevenZip()
        {
            this.processInfo = new ProcessStartInfo();
            this.processInfo.FileName = @"SevenZip\7za.exe";
            this.processInfo.WindowStyle = ProcessWindowStyle.Hidden;
        }

        #endregion

        #region ENUMS │ PUBLIC

        /// <summary>Compression level for archiving.</summary>
        public enum CompressionLevel
        {
            /// <summary>No compression, just copy.</summary>
            Store,

            /// <summary>Normal compression.</summary>
            Normal,

            /// <summary>Best compression level.</summary>
            Ultra
        }

        #endregion

        #region METHODS │ PRIVATE │ NON-STATIC

        /// <summary>Adds a system folder to an archived folder.</summary>
        /// <param name="sourceFolder">The system folder path to add to archive.</param>
        /// <param name="targetArchive">The archived folder path.</param>
        /// <param name="compressionLevel">Compression level to use in archive.</param>
        public string Archive(string sourceFolder, string targetArchive, CompressionLevel compressionLevel)
        {
            try
            {
                string argumentSwitch = string.Empty;
                
                switch (compressionLevel)
                {
                    case CompressionLevel.Store:
                        argumentSwitch = "-mx=0";
                        break;
                    case CompressionLevel.Normal:
                        argumentSwitch = "-mx=5 -ms=2g -m0=LZMA:d16m";
                        break;
                    case CompressionLevel.Ultra:
                        argumentSwitch = "-mx=9 -ms=4g -m0=LZMA2:d64m";
                        break;
                    default:
                        break;
                }

                this.processInfo.Arguments = string.Format("a -t7z \"{1}\" \"{0}\" {2}", sourceFolder, targetArchive, argumentSwitch);
                
                Process process = Process.Start(this.processInfo);
                process.WaitForExit();
            }
            catch (Exception exception)
            {
                return exception.Message;
            }

            return null;
        }

        /// <summary>Extracts an archived folder to a specified system folder.</summary>
        /// <param name="sourceArchive">Source archive folder to extract.</param>
        /// <param name="targetFolder">Target system folder to extract to.</param>
        public string Extract(string sourceArchive, string targetFolder = null)
        {
            try
            {
                this.processInfo.Arguments = string.IsNullOrEmpty(targetFolder)
                    ? this.processInfo.Arguments = string.Format("e \"{0}\"", sourceArchive)
                    : this.processInfo.Arguments = string.Format("e \"{0}\" -o\"{1}\"", sourceArchive, targetFolder);

                Process process = Process.Start(this.processInfo);
                process.WaitForExit();
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
            return null;

        }

        #endregion
    }
}
