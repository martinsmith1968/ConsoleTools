// <copyright file="Arguments.cs" company="Thales e-Security Ltd">
// Copyright (c) 2010 Thales e-Security Ltd
// All rights reserved. Company confidential.
// </copyright>
// 
// TODO: [Description of Arguments]

using System;
using System.Collections.Generic;
using NRA.Util.CommandLine;

namespace VideoThumbnailGenerator
{
    /// <summary>
    /// TODO: [Description of Arguments]
    /// </summary>
    public class Arguments
    {
        #region Main Parameters

        [DefaultArgument(ArgumentType.Required, GroupName = "Main", HelpText = "The input filename or wildcard")]
        public string filespec;

        #endregion

        #region Optional

        [Argument(ArgumentType.AtMostOnce, DefaultValue = "", GroupName = "Optional", ShortName = "o", HelpText = "The output folder (Defaults to same folder as input)")]
        public string OutputFolder;

        [Argument(ArgumentType.AtMostOnce, DefaultValue = 12, GroupName = "Optional", ShortName = "c", HelpText = "How many total images to generate")]
        public int ImageCount;

        [Argument(ArgumentType.AtMostOnce, DefaultValue = 3, GroupName = "Optional", ShortName = "h", HelpText = "How many images to generate horizontally")]
        public int ImagesHorizontal;

        [Argument(ArgumentType.AtMostOnce, DefaultValue = 4, GroupName = "Optional", ShortName = "v", HelpText = "How many images to generate vertically")]
        public int ImagesVertical;

        [Argument(ArgumentType.AtMostOnce, DefaultValue = 200, GroupName = "Optional", ShortName = "w", HelpText = "The width of individual images")]
        public int ImageWidth;

        #endregion

        #region Standalone

        [Argument(ArgumentType.BypassMandatory, DefaultValue = false, ShortName = "?", GroupName = "Standalone", HelpText = "Show the Help page")]
        public bool Help;

        #endregion

        #region Properties

        #endregion
    }
}
