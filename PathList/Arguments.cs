using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NRA.Util.CommandLine;

namespace PathList
{
    public class Arguments
    {
        #region Main Parameters

        [DefaultArgument(ArgumentType.AtMostOnce, DefaultValue = "PATH", GroupName = "Main", HelpText = "The environment variable to expand")]
        public string EnvironmentVariable;

        #endregion

        #region Optional

        [Argument(ArgumentType.AtMostOnce, ShortName = "s", GroupName = "Optional", HelpText = "The text that separates individual items")]
        public string Separator = Path.PathSeparator.ToString();

        [Argument(ArgumentType.AtMostOnce, DefaultValue = true, ShortName = "v", GroupName = "Optional", HelpText = "Check each item is a valid folder")]
        public bool CheckFolderExists;

        #endregion

        #region Standalone

        [Argument(ArgumentType.BypassMandatory, DefaultValue = false, ShortName = "?", GroupName = "Standalone", HelpText = "Show the Help page")]
        public bool Help;

        #endregion

        #region Properties

        public bool HasEnvironmentVariable
        {
            get { return !string.IsNullOrEmpty(EnvironmentVariable); }
        }

        #endregion
    }
}
