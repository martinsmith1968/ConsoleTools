using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRA.Util
{
    public abstract class FileHelper
    {
        /// <summary>
        /// Determines if a FileName contains any of the wildcard characters
        /// </summary>
        /// <param name="fileName">The filename</param>
        /// <returns>True if filename contains wildcard characters, otherwise False</returns>
        static public bool IsFileNameWildcard(string fileName)
        {
            return (fileName.IndexOfAny("%*".ToCharArray()) >= 0);
        }
    }
}
