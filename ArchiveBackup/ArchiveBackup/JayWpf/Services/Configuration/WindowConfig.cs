// ······································································//
// <copyright file="WindowConfig.cs" company="Jay Bautista Mendoza">     //
//     Copyright (c) Jay Bautista Mendoza. All rights reserved.          //
//     THIS IS PART OF MY PERSONAL OPEN SOURCE WPF WINDOW TEMPLATE.      //
//     THIS IS NOT PRIVATE PROPERTY. FEEL FREE TO MODIFY OR USE IT.      //
// </copyright>                                                          //
// ······································································//

namespace JayWpf.Services.Configuration
{
    using System.Configuration;

    /// <summary>Data Model class for Window WindowConfig.</summary>
    public class WindowConfig
    {
        /// <summary>Initializes a new instance of the <see cref="WindowConfig" /> class.</summary>
        public WindowConfig()
        {
            this.WindowSize = new Size();
            this.WindowPosition = new Position();
            this.WindowTheme = new Theme();
        }

        /// <summary>Gets or sets a value indicating whether the Window On-Top property is true or false.</summary>
        public bool WindowOnTop { get; set; }

        /// <summary>Gets or sets the window position.</summary>
        public Position WindowPosition { get; set; }

        /// <summary>Gets or sets the window size.</summary>
        public Size WindowSize { get; set; }

        /// <summary>Gets or sets the window theme.</summary>
        public Theme WindowTheme { get; set; }

        /// <summary>Save the Windows configuration to the configuration file.</summary>
        public void Save()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["position"].Value = this.WindowPosition.Top + "," + this.WindowPosition.Left;
            config.AppSettings.Settings["size"].Value = this.WindowSize.Width + "," + this.WindowSize.Height;
            
            config.AppSettings.Settings["theme"].Value = (int)this.WindowTheme.Shade + "," + (int)this.WindowTheme.Color;
            config.AppSettings.Settings["ontop"].Value = this.WindowOnTop.ToString();

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        private void Create()
        {
            
        }

        /// <summary>Load the Windows configuration from the configuration file.</summary>
        public void Load()
        {
            string[] theme = ConfigurationManager.AppSettings.Get("theme").Split(',');

            this.WindowTheme.Color = (Theme.Colors)int.Parse(theme[1]);
            this.WindowTheme.Shade = (Theme.Shades)int.Parse(theme[0]);

            string[] size = ConfigurationManager.AppSettings.Get("size").Split(',');
            this.WindowSize.Width = int.Parse(size[0]);
            this.WindowSize.Height = int.Parse(size[1]);

            string[] position = ConfigurationManager.AppSettings.Get("position").Split(',');
            this.WindowPosition.Top = int.Parse(position[0]);
            this.WindowPosition.Left = int.Parse(position[1]);

            string ontop = ConfigurationManager.AppSettings.Get("ontop");

            bool topmost = bool.Parse(ConfigurationManager.AppSettings.Get("ontop"));
            this.WindowOnTop = topmost;
        }
        
        /// <summary>Class for window position.</summary>
        public class Position
        {
            /// <summary>Gets or sets the top position.</summary>
            public double Top { get; set; }

            /// <summary>Gets or sets the left position.</summary>
            public double Left { get; set; }
        }

        /// <summary>Class for window size.</summary>
        public class Size
        {
            /// <summary>Gets or sets the size height.</summary>
            public double Height { get; set; }
            
            /// <summary>Gets or sets the size width.</summary>
            public double Width { get; set; }
        }

        /// <summary>Class for window themes.</summary>
        public class Theme
        {
            /// <summary>Enumeration of theme colors.</summary>
            public enum Colors
            {
                /// <summary>Violet color accent.</summary>
                VIOLET,

                /// <summary>Indigo color accent.</summary>
                INDIGO,

                /// <summary>Blue color accent.</summary>
                BLUE,

                /// <summary>Red color accent.</summary>
                TEAL,
                
                /// <summary>Green color accent.</summary>
                GREEN,
                
                /// <summary>Yellow color accent.</summary>
                YELLOW,

                /// <summary>Orange color accent.</summary>
                ORANGE,
                         
                /// <summary>Red color accent.</summary>
                RED,

                /// <summary>Pink color accent.</summary>
                PINK,

                /// <summary>White color accent.</summary>
                GRAY
            }

            /// <summary>Enumeration of theme shades.</summary>
            public enum Shades
            {
                /// <summary>Dark window scheme.</summary>
                DARK,
                
                /// <summary>Light window scheme.</summary>
                LIGHT
            }

            /// <summary>Gets or sets the theme color.</summary>
            public Colors Color { get; set; }

            /// <summary>Gets or sets the theme shade.</summary>
            public Shades Shade { get; set; }
        }
    }
}
