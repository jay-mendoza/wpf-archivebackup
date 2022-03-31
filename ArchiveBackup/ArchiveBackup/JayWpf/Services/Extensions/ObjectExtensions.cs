// ······································································//
// <copyright file="ObjectExtensions.cs" company="Jay Bautista Mendoza"> //
//     Copyright (c) Jay Bautista Mendoza. All rights reserved.          //
//     THIS IS PART OF MY PERSONAL OPEN SOURCE WPF WINDOW TEMPLATE.      //
//     THIS IS NOT PRIVATE PROPERTY. FEEL FREE TO MODIFY OR USE IT.      //
// </copyright>                                                          //
// ······································································//

namespace JayWpf.Services.Extensions
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>Extension methods for generic objects.</summary>
    public static class ObjectExtensions
    {
        /// <summary>Convert the object to string and remove the casing [uses ToString().ToUpper()].</summary>
        /// <param name="variable">The variable.</param>
        /// <returns>An uncased string.</returns>
        public static string Uncase(this object variable)
        {
            if (variable == null)
            {
                return null;
            }

            return variable.ToString().ToUpper().Trim();
        }

        /// <summary>Case insensitive comparison of two objects converted to string.</summary>
        /// <param name="variable">First object to compare.</param>
        /// <param name="equalToVariable">Second object to compare.</param>
        /// <returns>TRUE if values are equal, otherwise, FALSE.</returns>
        public static bool IsEqualUncasedTo(this object variable, object equalToVariable)
        {
            if (variable == null || equalToVariable == null)
            {
                return false;
            }

            return variable.Uncase() == equalToVariable.Uncase();
        }

        /// <summary>Case insensitive contain comparison of two objects converted to string.</summary>
        /// <param name="variable">First object to compare.</param>
        /// <param name="containsVariable">Second object to compare.</param>
        /// <returns>TRUE if variable contains contains-variable, otherwise, FALSE.</returns>
        public static bool ContainsUncased(this object variable, object containsVariable)
        {
            if (variable == null || containsVariable == null)
            {
                return false;
            }

            return variable.Uncase().Contains(containsVariable.Uncase());
        }

        /// <summary>Determine if a List is NULL or EMPTY.</summary>
        /// <typeparam name="T">Type of List (optional).</typeparam>
        /// <param name="variable">The List to test if NULL or EMPTY.</param>
        /// <returns>TRUE if List is NULL or EMPTY, otherwise, FALSE.</returns>
        public static bool IsNullOrEmpty<T>(this IList<T> variable)
        {
            return variable == null || !variable.Any();
        }

        /// <summary>Determine if a string is NULL or EMPTY.</summary>
        /// <param name="variable">The string to test if NULL or EMPTY.</param>
        /// <returns>TRUE if string is NULL or EMPTY, otherwise, FALSE.</returns>
        public static bool IsNullOrEmpty(this string variable)
        {
            return string.IsNullOrEmpty(variable);
        }

        /// <summary>Remove all invalid filename characters in a string.</summary>
        /// <param name="variable">String to remove from.</param>
        /// <returns>String without any invalid filename characters.</returns>
        public static string RemoveInvalid(this string variable)
        {
            if (string.IsNullOrEmpty(variable))
            {
                return variable;
            }

            return Path.GetInvalidFileNameChars().Aggregate(variable, (current, c) => current.Replace(c.ToString(), string.Empty));
        }

        /// <summary>Replace invalid filename characters in a string (꞉ ," ,∕ ,? only).</summary>
        /// <param name="variable">String to check and replace characters from.</param>
        /// <returns>String with replaced invalid filename characters.</returns>
        public static string ReplaceInvalid(this string variable)
        {
            if (string.IsNullOrEmpty(variable))
            {
                return variable;
            }

            return variable.Replace("\"", "'").Replace(":", "꞉").Replace("/", "∕").Replace("?", "‽");
        }

        /// <summary>Replace invalid filename characters (꞉ ," ,∕ ,? only) and remove the remaining invalid characters.</summary>
        /// <param name="variable">String to check and replace, and remove characters from.</param>
        /// <returns>String with replaced and removed invalid filename characters.</returns>
        public static string ReplaceAndRemoveInvalid(this string variable)
        {
            if (string.IsNullOrEmpty(variable))
            {
                return variable;
            }

            return variable.ReplaceInvalid().RemoveInvalid();
        }

        /// <summary>File or folder complete path compare.</summary>
        /// <param name="variable">First path to compare.</param>
        /// <param name="equalToVariable">Second path to compare.</param>
        /// <returns>TTRUE if values are equal, otherwise, FALSE.</returns>
        public static bool IsPathEqual(this string variable, string equalToVariable)
        {
            return variable.NormalizePath() == equalToVariable.NormalizePath();
        }

        /// <summary>Normalized a path string.</summary>
        /// <param name="variable">Path string to normalize.</param>
        /// <returns>Normalized path string</returns>
        public static string NormalizePath(this string variable)
        {
            return Path.GetFullPath(new System.Uri(variable).LocalPath)
                       .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                       .ToUpperInvariant();
        }
    }
}
