//-----------------------------------------------------------------------
// <copyright file="Favorite.cs" company="Jay Bautista Mendoza">
//     Copyright (c) Jay Bautista Mendoza. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace ArchiveBackup.Services
{
    using System;
    using System.Xml.Serialization;

    /// <summary>Data model class for "Favorite".</summary>
    [Serializable]
    public class Favorite
    {
        /// <summary>Gets or sets the Name attribute.</summary>
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        /// <summary>Gets or sets the Name attribute.</summary>
        [XmlAttribute(AttributeName = "one")]
        public string DirectoryOne { get; set; }

        /// <summary>Gets or sets the Name attribute.</summary>
        [XmlAttribute(AttributeName = "two")]
        public string DirectoryTwo { get; set; }
    }
}
