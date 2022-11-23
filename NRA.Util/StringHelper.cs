using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRA.Util
{
    /// <summary>
    ///
    /// </summary>
    public abstract class StringHelper
    {
        /// <summary>
        /// Ensures the starts with.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="prefix">The prefix.</param>
        /// <returns></returns>
        static public string EnsureStartsWith(string s, string prefix)
        {
            if (prefix == null)
                throw new ArgumentNullException("prefix");

            if (s == null || !s.StartsWith(prefix))
                s = string.Concat(prefix, s);

            return s;
        }

        /// <summary>
        /// Ensures the ends with.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="suffix">The suffix.</param>
        /// <returns></returns>
        static public string EnsureEndsWith(string s, string suffix)
        {
            if (suffix == null)
                throw new ArgumentNullException("suffix");

            if (s == null || !s.EndsWith(suffix))
                s = string.Concat(s, suffix);

            return s;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="s"></param>
        /// <param name="prefixsuffix"></param>
        /// <returns></returns>
        static public string EnsureStartsAndEndsWith(string s, string prefixsuffix)
        {
            if (prefixsuffix == null)
                throw new ArgumentNullException("prefixsuffix");

            return EnsureStartsWith(EnsureEndsWith(s, prefixsuffix), prefixsuffix);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="s"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        static public string RemoveStartsWith(string s, string prefix)
        {
            if (!string.IsNullOrEmpty(s) && !string.IsNullOrEmpty(prefix))
            {
                while (s.StartsWith(prefix))
                    s = s.Substring(prefix.Length);
            }

            return s;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="s"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        static public string RemoveEndsWith(string s, string suffix)
        {
            if (!string.IsNullOrEmpty(s) && !string.IsNullOrEmpty(suffix))
            {
                while (s.EndsWith(suffix))
                    s = s.Substring(0, s.Length - suffix.Length);
            }

            return s;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="s"></param>
        /// <param name="prefixsuffix"></param>
        /// <returns></returns>
        static public string RemoveStartsAndEndsWith(string s, string prefixsuffix)
        {
            return RemoveStartsWith(RemoveEndsWith(s, prefixsuffix), prefixsuffix);
        }

        #region Search / Replace

        /// <summary>
        /// Replace the first occurrence of a string within another string
        /// </summary>
        /// <param name="s">The string to be searched</param>
        /// <param name="find">The text to find</param>
        /// <param name="replace">The text to replace it with</param>
        /// <returns>Modified copy of string to be searched</returns>
        public static string ReplaceFirst(string s, string find, string replace)
        {
            if (!string.IsNullOrEmpty(s) && !string.IsNullOrEmpty(find))
            {
                int index = s.IndexOf(find);

                if (index >= 0)
                {
                    s = string.Format("{0}{1}{2}",
                        s.Substring(0, index),
                        replace,
                        s.Substring(index + find.Length));
                }
            }

            return s;
        }

        /// <summary>
        /// Remove all occurrences of a set of characters from a string
        /// </summary>
        /// <param name="text">The string to be searched</param>
        /// <param name="charsToRemove">The characters to be removed</param>
        /// <returns>The string without any of the specified characters</returns>
        public static string RemoveAll(string text, string charsToRemove)
        {
            return RemoveAny(text, charsToRemove.ToCharArray());
        }

        /// <summary>
        /// Remove all occurrences of a set of characters from a string
        /// </summary>
        /// <param name="text">The string to be searched</param>
        /// <param name="charsToRemove">The characters to be removed</param>
        /// <returns>The string without any of the specified characters</returns>
        public static string RemoveAny(string text, char[] charsToRemove)
        {
            foreach (char c in charsToRemove)
            {
                text = text.Replace(c.ToString(), string.Empty);
            }

            return text;
        }

        /// <summary>
        /// Remove all characters from a string, except any in the specified set
        /// </summary>
        /// <param name="text">The string to be searched</param>
        /// <param name="charsToKeep">The characters allowed to be kept</param>
        /// <returns>The string but only containing characters from the specified set</returns>
        public static string RemoveAnyExcept(string text, char[] charsToKeep)
        {
            return RemoveAnyExcept(text, new string(charsToKeep));
        }

        /// <summary>
        /// Remove all characters from a string, except any in the specified set
        /// </summary>
        /// <param name="text">The string to be searched</param>
        /// <param name="charsToKeep">The characters allowed to be kept</param>
        /// <returns>The string but only containing characters from the specified set</returns>
        public static string RemoveAnyExcept(string text, string charsToKeep)
        {
            StringBuilder sb = new StringBuilder(text.Length);

            foreach (char c in text.ToCharArray())
            {
                if (charsToKeep.Contains(c.ToString()))
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Determine if a string only contains the characters specified
        /// </summary>
        /// <param name="text">The string to be tested</param>
        /// <param name="allowedChars">A array of allowed characters</param>
        /// <returns><c>True</c> if string only contains the characters in allowedChars. <c>False</c> otherwise.</returns>
        public static bool ContainsOnly(string text, char[] allowedChars)
        {
            return ContainsOnly(text, new string(allowedChars));
        }

        /// <summary>
        /// Determine if a string only contains the characters specified
        /// </summary>
        /// <param name="text">The string to be tested</param>
        /// <param name="allowedChars">A string of allowed characters</param>
        /// <returns><c>True</c> if string only contains the characters in allowedChars. <c>False</c> otherwise.</returns>
        public static bool ContainsOnly(string text, string allowedChars)
        {
            foreach (char c in text)
            {
                if (!allowedChars.Contains(c.ToString()))
                {
                    return false;
                }
            }

            return true;
        }

        #endregion
    }
}
