using System;
using System.Collections.Generic;
using System.Text;
using NRA.Util.CommandLine;

namespace ODBCExport
{
    #region Supporting Enumerations

    /// <summary>
    ///
    /// </summary>
    public enum OutputFormat
    {
        /// <summary>
        /// SQL INSERT statements
        /// </summary>
        SQL,

        /// <summary>
        /// Tab Separated
        /// </summary>
        Tab,

        /// <summary>
        /// Comma Separated
        /// </summary>
        CSV,

        /// <summary>
        /// XML
        /// </summary>
        XML
    }

    public enum SQLDeleteType
    {
        None,

        Truncate,

        Delete
    }

    #endregion

    /// <summary>
    /// Program Arguments
    /// </summary>
    public class Arguments
    {
        #region Main Parameters

        [DefaultArgument(ArgumentType.Required, GroupName = "Main", HelpText = "The Table Name to export")]
        public string TableName;

        [Argument(ArgumentType.AtMostOnce, GroupName = "Main", ShortName = "w", HelpText = "A Filter (Where) clause")]
        public string Where;

        #endregion

        #region Connection

        [Argument(ArgumentType.Required, GroupName = "Connection", ShortName = "s", HelpText = "The ODBC Data Source to connect to")]
        public string DSN;

        [Argument(ArgumentType.AtMostOnce, GroupName = "Connection", ShortName = "d", HelpText = "The Database to use")]
        public string Database;

        [Argument(ArgumentType.AtMostOnce, DefaultValue = 0, GroupName = "Connection", ShortName = "ct", HelpText = "Connection Timeout Seconds (0: infinite)")]
        public int ConnectionTimeoutSeconds;

        #endregion

        #region Optional

        [Argument(ArgumentType.AtMostOnce, DefaultValue = false, ShortName = "g", GroupName = "Optional", HelpText = "Debug SQL statements")]
        public bool DebugSQL;

        [Argument(ArgumentType.AtMostOnce, DefaultValue = 30, ShortName = "et", GroupName = "Optional", HelpText = "Execution Timeout seconds (0; infinite)")]
        public int ExecutionTimeoutSeconds;

        [Argument(ArgumentType.AtMostOnce, DefaultValue = false, ShortName = "q", GroupName = "Optional", HelpText = "Quiet Operation")]
        public bool Quiet;

        [Argument(ArgumentType.AtMostOnce, DefaultValue = true, ShortName = "sw", GroupName = "Optional",HelpText = "Whether we are allowed to switch active DB through ODBC")]
        public bool AllowDBSwitch;

        #endregion

        #region Output

        [Argument(ArgumentType.AtMostOnce, DefaultValue = null, ShortName = "of", GroupName = "Output", HelpText = "The file to write output to")]
        public string OutputFile;

        [Argument(ArgumentType.AtMostOnce, DefaultValue = OutputFormat.SQL, ShortName = "ofmt", GroupName = "Output", HelpText = "How to format the output")]
        public OutputFormat OutputFormat;

        [Argument(ArgumentType.AtMostOnce, DefaultValue = "'", ShortName = "qc", GroupName = "Output", HelpText = "The character to use to quote text")]
        public string QuoteChar;

        [Argument(ArgumentType.AtMostOnce, DefaultValue = SQLDeleteType.Truncate, ShortName = "sqldel", GroupName = "Output", HelpText = "The type of statement to write before the data")]
        public SQLDeleteType SQLDeleteType;

        #endregion

        #region Standalone

        [Argument(ArgumentType.BypassMandatory, ShortName = "?", GroupName = "Standalone", HelpText = "Show the Help page")]
        public bool Help = false;

        #endregion

        #region Derived Properties

        /// <summary>
        /// Gets a value indicating whether this instance has database.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has database; otherwise, <c>false</c>.
        /// </value>
        public bool HasDatabase
        {
            get { return !string.IsNullOrEmpty(Database) && Database.Trim().Length > 0; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has output file.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has output file; otherwise, <c>false</c>.
        /// </value>
        public bool HasOutputFile
        {
            get { return !string.IsNullOrEmpty(OutputFile) && OutputFile.Trim().Length > 0; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has where.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has where; otherwise, <c>false</c>.
        /// </value>
        public bool HasWhere
        {
            get { return !string.IsNullOrEmpty(Where) && Where.Trim().Length > 0; }
        }

        #endregion
    }
}
