using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NRA.Util.CommandLine;

namespace SETList
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

        [Argument(ArgumentType.AtMostOnce, DefaultValue = SortType.Asc, GroupName = "Main", ShortName = "s", HelpText = "How to sort the variable names")]
        //[DefaultArgument(ArgumentType.AtMostOnce, DefaultValue = SortType.Asc, GroupName = "Main", HelpText = "How to sort the variable names")]
        public SortType SortType;

        #endregion

        #region Formatting

        [Argument(ArgumentType.AtMostOnce, DefaultValue = -1, GroupName = "Formatting", ShortName = "w", HelpText = "The width of the Variable Name column (-1: Auto)")]
        public int VariableNameWidth;

        [Argument(ArgumentType.AtMostOnce, DefaultValue = true, GroupName = "Formatting", ShortName = "a", HelpText = "Align values in the same column")]
        public bool AlignValues;

        [Argument(ArgumentType.AtMostOnce, DefaultValue = " = ", GroupName = "Formatting", ShortName = "p", HelpText = "The string used to separate names from values")]
        public string NameSeparator;

        #endregion

        #region Optional

        [Argument(ArgumentType.AtMostOnce, DefaultValue = "", ShortName = "v", GroupName = "Optional", HelpText = "The text that separates individual values")]
        public string ValueSeparator;

        #endregion

        #region Standalone

        [Argument(ArgumentType.BypassMandatory, DefaultValue = false, ShortName = "?", GroupName = "Standalone", HelpText = "Show the Help page")]
        public bool Help;

        #endregion

        #region Properties

        public bool HasValueSeparator
        {
            get { return !string.IsNullOrEmpty(ValueSeparator); }
        }

        public char ValueSeparatorChar
        {
            get { return HasValueSeparator && ValueSeparator.Length > 0 ? ValueSeparator[0] : char.MaxValue; }
        }

        #endregion
    }
}
