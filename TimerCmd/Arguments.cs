// <copyright file="Arguments.cs" company="Thales e-Security Ltd">
// Copyright (c) 2010 Thales e-Security Ltd
// All rights reserved. Company confidential.
// </copyright>
//
// Application Arguments

using NRA.Util.CommandLine;

namespace TimerCmd
{
    #region Supporting Enumerations

    /// <summary>
    ///
    /// </summary>
    public enum TimerCommand
    {
        Start,
        Stop,
        Pause,
        Reset,
        Delete,
        Elapsed
    }

    /// <summary>
    ///
    /// </summary>
    public enum TimerOperation
    {
        Help,
        List,
        ClearAll
    }

    #endregion

    /// <summary>
    /// Application Arguments
    /// </summary>
    public class Arguments
    {
        #region Main Parameters

        /// <summary>
        ///
        /// </summary>
        [DefaultArgument(ArgumentType.Required, GroupName = "Main", HelpText = "The name of the Timer")]
        public string Name;

        /// <summary>
        ///
        /// </summary>
        [Argument(ArgumentType.AtMostOnce, DefaultValue = TimerCommand.Elapsed, ShortName = "c", GroupName = "Main", HelpText = "Timer Command to execute")]
        public TimerCommand Command;

        #endregion

        #region Options

        /// <summary>
        ///
        /// </summary>
        [Argument(ArgumentType.AtMostOnce, DefaultValue = true, ShortName = "r", GroupName = "Options", HelpText = "Reuse any existing Timer")]
        public bool Reuse;

        /// <summary>
        ///
        /// </summary>
        [Argument(ArgumentType.AtMostOnce, DefaultValue = false, ShortName = "q", GroupName = "Options", HelpText = "Suppress informational output")]
        public bool Quiet;

        #endregion

        #region Standalone

        /// <summary>
        ///
        /// </summary>
        [Argument(ArgumentType.BypassMandatory, DefaultValue = false, ShortName = "l", GroupName = "Standalone", HelpText = "List all known Timers")]
        public bool List;

        /// <summary>
        ///
        /// </summary>
        [Argument(ArgumentType.BypassMandatory, DefaultValue = false, ShortName = "z", GroupName = "Standalone", HelpText = "Delete all known Timers")]
        public bool ClearAll;

        [Argument(ArgumentType.BypassMandatory, DefaultValue = false, ShortName = "g", GroupName = "Standalone", HelpText = "Activate Debug output")]
        public bool Debug;

        /// <summary>
        ///
        /// </summary>
        [Argument(ArgumentType.BypassMandatory, DefaultValue = false, ShortName = "?", GroupName = "Standalone", HelpText = "Show the Help page")]
        public bool Help;

        #endregion
    }
}
