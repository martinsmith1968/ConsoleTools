// <copyright file="Arguments.cs" company="Thales e-Security Ltd">
// Copyright (c) 2010 Thales e-Security Ltd
// All rights reserved. Company confidential.
// </copyright>
//
// The Program Arguments

using System;
using System.Collections.Generic;
using NRA.Util.CommandLine;

namespace AppLockCmd
{
    /// <summary>
    /// Enumeration of Lock type
    /// </summary>
    public enum LockType
    {
        /// <summary>
        /// Acquire Lock
        /// </summary>
        Acquire,

        /// <summary>
        /// Release lock
        /// </summary>
        Release
    }

    /// <summary>
    /// The Program Arguments
    /// </summary>
    public class Arguments
    {
        #region Main Parameters

        /// <summary>
        /// Internal lock type field
        /// </summary>
        [DefaultArgument(ArgumentType.Required, GroupName = "Main", HelpText = "The type of operation to perform on the named lock")]
        public LockType LockType;

        /// <summary>
        /// Internal name field
        /// </summary>
        [DefaultArgument(ArgumentType.Required, GroupName = "Main", HelpText = "The name of the lock")]
        public string Name;

        #endregion

        #region Optional

        /// <summary>
        /// Internal retry interval field
        /// </summary>
        [Argument(ArgumentType.AtMostOnce, ShortName = "r", GroupName = "Optional", DefaultValue = 1000, HelpText = "The interval between retrying (in ms)")]
        public int RetryInterval = 1000;

        /// <summary>
        /// Internal timeout field
        /// </summary>
        [Argument(ArgumentType.AtMostOnce, ShortName = "t", GroupName = "Optional", DefaultValue = 60, HelpText = "The total time to wait before aborting (in sec)")]
        public int Timeout = 60;

        /// <summary>
        /// Internal quiet field
        /// </summary>
        [Argument(ArgumentType.AtMostOnce, ShortName = "q", GroupName = "Optional", HelpText = "Control program output")]
        public bool Quiet = false;

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
        public bool HasLockName
        {
            get { return !string.IsNullOrEmpty(this.Name); }
        }

        #endregion
    }
}
