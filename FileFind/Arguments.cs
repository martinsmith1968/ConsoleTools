using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NRA.Util.CommandLine;

namespace FileFind
{
    public class Arguments
    {
        #region Main Parameters

        [DefaultArgument(ArgumentType.AtMostOnce, DefaultValue = "", GroupName = "Main", HelpText = "The directory to start searching from")]
        public string Path;

        #endregion

        #region Optional

        [Argument(ArgumentType.MultipleUnique, ShortName = "n", GroupName = "Optional", HelpText = "The File name(s) to search for")]
        public string[] Name = null;

        [Argument(ArgumentType.AtMostOnce, ShortName = "q", GroupName = "Optional", HelpText = "Control output display")]
        public bool Quiet = false;

        [Argument(ArgumentType.AtMostOnce, ShortName = "p", GroupName = "Optional", HelpText = "Show the current folder being scanned")]
        public bool Progress = false;

        #endregion

        #region Standalone

        /// <summary>
        ///
        /// </summary>
        [Argument(ArgumentType.BypassMandatory, DefaultValue = false, ShortName = "?", GroupName = "Standalone", HelpText = "Show the Help page")]
        public bool Help;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance has path.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has path; otherwise, <c>false</c>.
        /// </value>
        public bool HasPath
        {
            get { return !string.IsNullOrEmpty(this.Path); }
        }

        /// <summary>
        /// Gets the full path.
        /// </summary>
        public string FullPath
        {
            get
            {
                string path = this.HasPath
                    ? this.Path
                    : Directory.GetCurrentDirectory();

                DirectoryInfo di = new DirectoryInfo(path);
                return di.FullName;
            }
        }

        /// <summary>
        /// Gets the name count.
        /// </summary>
        public int NameCount
        {
            get { return this.Name == null ? 0 : this.Name.Length; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Fixups this instance.
        /// </summary>
        public void Fixup()
        {
            if (this.HasPath)
            {
                if ((this.Path.Contains("*") || this.Path.Contains("?")) && this.NameCount == 0)
                {
                    this.Name = new string[] { this.Path };
                    this.Path = string.Empty;
                }
            }
        }

        #endregion
    }
}
