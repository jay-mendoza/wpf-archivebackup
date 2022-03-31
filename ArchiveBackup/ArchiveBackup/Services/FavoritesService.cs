//-----------------------------------------------------------------------
// <copyright file="FavoritesService.cs" company="Jay Bautista Mendoza">
//     Copyright (c) Jay Bautista Mendoza. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace ArchiveBackup.Services
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Xml.Serialization;

    /// <summary>Service class for Favorites.</summary>
    public class FavoritesService
    {
        #region FIELDS │ PRIVATE │ NON-STATIC │ NON-READONLY

        /// <summary>Private instance of the xmlFile.</summary>
        private string xmlFile;

        #endregion

        #region CONSTRUCTORS │ PUBLIC │ NON-STATIC

        /// <summary>Initializes a new instance of the <see cref="FavoritesService" /> class.</summary>
        public FavoritesService()
        {
            this.xmlFile = Process.GetCurrentProcess().MainModule.FileName.Replace(".exe", ".xml");
        }

        #endregion

        #region PROPERTIES │ PUBLIC │ NON-STATIC
        
        /// <summary>Gets the XML file path and name based on the program's name.</summary>
        public string XmlFile
        {
            get
            {
                return this.xmlFile;
            }
        }

        #endregion

        #region METHODS │ PUBLIC │ NON-STATIC

        /// <summary>Create a blank favorites file.</summary>
        /// <param name="favoriteCount">Number of favorite objects to create in the favorites file.</param>
        public void CreateBlankFavoriesFile(int favoriteCount)
        {
            Favorites favorites = new Favorites();

            for (int ctr = 1; ctr <= favoriteCount; ctr++)
            {
                favorites.Root.Add(new Favorite { Name = "Favorite0" + ctr.ToString() });
            }

            this.SaveFavorites(favorites);
        }

        /// <summary>Load the Favorites object from the XML favorites file.</summary>
        /// <returns>Favorites object of the XML favorites file.</returns>
        public Favorites LoadFavorites()
        {
            using (Stream fileStream = new FileStream(this.xmlFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(Favorites));
                Favorites root = (Favorites)deserializer.Deserialize(fileStream);
                return root;
            }
        }

        /// <summary>Save a Favorites object to the XML favorites file.</summary>
        /// <param name="root">Favorites object for the XML favorites file.</param>
        public void SaveFavorites(Favorites root)
        {
            using (Stream fileStream = new FileStream(this.xmlFile, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Favorites));
                serializer.Serialize(fileStream, root);
            }
        }
        #endregion
    }
}
