using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NRA.Util.CommandLine;

namespace PFCreate
{
    public class Arguments
    {
        #region Main Parameters

        [DefaultArgument(ArgumentType.Required, GroupName = "Main", HelpText = "The name of the folder to create")]
        public string FolderName;

        #endregion

        #region Optional

        [Argument(ArgumentType.AtMostOnce, DefaultValue = Environment.SpecialFolder.ProgramFiles, GroupName = "Optional", ShortName = "s", HelpText = "The special folder name to use as a base folder")]
        public Environment.SpecialFolder SpecialFolder;

        [Argument(ArgumentType.AtMostOnce, DefaultValue = true, GroupName = "Optional", ShortName = "c", HelpText = "Request confirmation before creating")]
        public bool Confirm;

        #endregion

        #region Standalone

        [Argument(ArgumentType.BypassMandatory, DefaultValue = false, ShortName = "?", GroupName = "Standalone", HelpText = "Show the Help page")]
        public bool Help;

        #endregion

        #region Properties

        #endregion
    }
}
