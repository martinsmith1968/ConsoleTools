using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NRA.Util.CommandLine;

namespace SFList
{
    /// <summary>
    /// 
    /// </summary>
    public enum SortType
    {
        None,
        Asc,
        Desc
    }

    /// <summary>
    /// 
    /// </summary>
    public class Arguments
    {
        #region Main Parameters

        [Argument(ArgumentType.AtMostOnce, DefaultValue = SortType.None, GroupName = "Main", ShortName = "s", HelpText = "How to sort the folder names")]
        public SortType SortType;

        #endregion

        #region Standalone

        [Argument(ArgumentType.BypassMandatory, DefaultValue = false, ShortName = "?", GroupName = "Standalone", HelpText = "Show the Help page")]
        public bool Help;

        #endregion
    }
}
