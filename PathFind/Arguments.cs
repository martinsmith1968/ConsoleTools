using System;
using System.IO;
using NRA.Util.CommandLine;

namespace PathFind
{
    /// <summary>
    /// The Program Arguments
    /// </summary>
    public class Arguments
    {
        #region Main Parameters

        /// <summary>
        /// The FileName
        /// </summary>
        [DefaultArgument(ArgumentType.Required, GroupName = "Main", HelpText = "The file to search for")]
        public string FileName;

        #endregion

        #region Optional

        /// <summary>
        /// The Environment Variable to use as a list of paths
        /// </summary>
        [Argument(ArgumentType.AtMostOnce, ShortName = "v", DefaultValue = "PATH", GroupName = "Optional", HelpText = "The environment variable to expand")]
        public string EnvironmentVariable;

        /// <summary>
        /// The path separator for <seealso cref="EnvironmentVariable"/>
        /// </summary>
        [Argument(ArgumentType.AtMostOnce, ShortName = "s", GroupName = "Optional", HelpText = "The text that separates individual items")]
        public string Separator = Path.PathSeparator.ToString();

        #endregion

        #region Standalone

        /// <summary>
        /// Display Help ?
        /// </summary>
        [Argument(ArgumentType.BypassMandatory, DefaultValue = false, ShortName = "?", GroupName = "Standalone", HelpText = "Show the Help page")]
        public bool Help;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance has file name.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has file name; otherwise, <c>false</c>.
        /// </value>
        public bool HasFileName
        {
            get { return !string.IsNullOrEmpty(this.FileName); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has environment variable.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has environment variable; otherwise, <c>false</c>.
        /// </value>
        public bool HasEnvironmentVariable
        {
            get { return !string.IsNullOrEmpty(EnvironmentVariable); }
        }

        #endregion
    }
}
