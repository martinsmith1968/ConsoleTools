using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NRA.Util.CommandLine;

namespace TextViewer
{
    public class Arguments
    {
        #region Main Parameters

        [DefaultArgument(ArgumentType.Required, GroupName = "Main", HelpText = "The filename to view")]
        public string FileName;

        #endregion

        #region Optional

        [Argument(ArgumentType.AtMostOnce, GroupName = "Optional", DefaultValue = 0, ShortName = "l", HelpText = "The line number to place at the top of the screen")]
        public int Line;

        [Argument(ArgumentType.AtMostOnce, GroupName = "Optional", DefaultValue = 0, ShortName = "c", HelpText = "The column number to place at the left of the screen")]
        public int Column;

        #endregion

        #region Standalone

        [Argument(ArgumentType.BypassMandatory, DefaultValue = false, ShortName = "?", GroupName = "Standalone", HelpText = "Show the Help page")]
        public bool Help;

        #endregion

        #region Properties

        #endregion
    }
}
