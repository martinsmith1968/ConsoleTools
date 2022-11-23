using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace NRA.Util
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class AssemblyHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        static public string GetProgramName()
        {
            return GetProgramName(Assembly.GetEntryAssembly());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asm"></param>
        /// <returns></returns>
        static public string GetProgramName(Assembly asm)
        {
            return Path.GetFileName(asm.Location);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        static public string GetProgramNameWithoutExtension()
        {
            return Path.GetFileNameWithoutExtension(GetProgramName());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        static public string GetTitle()
        {
            return GetTitle(Assembly.GetEntryAssembly());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="asm"></param>
        /// <returns></returns>
        static public string GetTitle(Assembly asm)
        {
            return asm.GetName().Name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        static public Version GetVersion()
        {
            return GetVersion(Assembly.GetEntryAssembly());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="asm"></param>
        /// <returns></returns>
        static public Version GetVersion(Assembly asm)
        {
            return asm.GetName().Version;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        static public string GetVersionText()
        {
            return GetVersionText(Assembly.GetEntryAssembly());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="asm"></param>
        /// <returns></returns>
        static public string GetVersionText(Assembly asm)
        {
            return GetVersionText(asm.GetName().Version);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ver"></param>
        /// <returns></returns>
        static public string GetVersionText(Version ver)
        {
            return string.Format("{0}.{1}.{2}.{3}", ver.Major, ver.Minor, ver.Build, ver.Revision);
        }

        static public string FormatVersionText(string versionText)
        {
            return FormatVersionText(versionText, 2);
        }

        static public string FormatVersionText(string versionText, int minimumBits)
        {
            List<string> versionBits = new List<string>();

            minimumBits = Math.Max(minimumBits, 1);

            if (!string.IsNullOrEmpty(versionText))
            {
                // Split the version text by "."
                versionBits.AddRange(versionText.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));

                // Remove 0s at the end until we find a number or have reached the minimum precision to display
                while (versionBits.Count > minimumBits && versionBits[versionBits.Count - 1] == "0")
                    versionBits.RemoveAt(versionBits.Count - 1);

                // Put it back together
                versionText = string.Join(".", versionBits.ToArray());
            }

            return versionText;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        static public string GetDescription()
        {
            return GetDescription(Assembly.GetEntryAssembly());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="asm"></param>
        /// <returns></returns>
        static public string GetDescription(Assembly asm)
        {
            AssemblyDescriptionAttribute[] atts = (AssemblyDescriptionAttribute[])asm.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), true);
            return (atts.Length > 0) ? atts[0].Description : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        static public string GetCopyright()
        {
            return GetCopyright(Assembly.GetEntryAssembly());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="asm"></param>
        /// <returns></returns>
        static public string GetCopyright(Assembly asm)
        {
            AssemblyCopyrightAttribute[] atts = (AssemblyCopyrightAttribute[])asm.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), true);
            return (atts.Length > 0) ? atts[0].Copyright : null;
        }
    }
}
