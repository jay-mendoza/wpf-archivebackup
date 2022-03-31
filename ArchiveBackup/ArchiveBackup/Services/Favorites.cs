//-----------------------------------------------------------------------
// <copyright file="Favorites.cs" company="Jay Bautista Mendoza">
//     Copyright (c) Jay Bautista Mendoza. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace ArchiveBackup.Services
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>Serializable class for a collection of Favorite objects.</summary>
    [Serializable]
    [XmlRoot(ElementName = "favorites")]
    public class Favorites
    {
        /// <summary>Initializes a new instance of the <see cref="Favorites" /> class.</summary>
        public Favorites()
        {
            this.Root = new List<Favorite>();
        }

        /// <summary>Gets or sets the list of Favorite objects.</summary>
        [XmlElement(ElementName = "note")]
        public List<Favorite> Root { get; set; }
    }
}
