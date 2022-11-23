using System;
using NRA.Util.CommandLine;

namespace ODBCQueryCmd
{
    #region Supporting Enumerations

    /// <summary>
    ///
    /// </summary>
    public enum ExecutionType
    {
        /// <summary>
        /// Determine automatically
        /// </summary>
        Automatic,

        /// <summary>
        /// DataReader
        /// </summary>
        Results,

        /// <summary>
        /// NonQuery
        /// </summary>
        NoResults,

        /// <summary>
        /// Scalar
        /// </summary>
        SingleValue
    }

    /// <summary>
    ///
    /// </summary>
    public enum OutputFormat
    {
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
        XML,

        /// <summary>
        /// ISQL Character Tables
        /// </summary>
        ISQL
    }

    #endregion

    /// <summary>
    /// Program Arguments
    /// </summary>
    public class Arguments
    {
        #region Main Parameters

        [DefaultArgument(ArgumentType.Required, GroupName = "Main", HelpText = "The SQL statement to execute")]
        public string SQL;

        #endregion

        #region Connection

        [Argument(ArgumentType.Required, GroupName = "Connection", ShortName = "s", HelpText = "The ODBC Data Source to connect to")]
        public string DSN;

        [Argument(ArgumentType.AtMostOnce, GroupName = "Connection", ShortName = "d", HelpText = "The Database to use")]
        public string Database;

        [Argument(ArgumentType.AtMostOnce, DefaultValue = true, GroupName = "Connection", ShortName = "w", HelpText = "Set Wait Mode")]
        public bool WaitMode;

        [Argument(ArgumentType.AtMostOnce, DefaultValue = 0, GroupName = "Connection", ShortName = "ct", HelpText = "Connection Timeout Seconds (0: infinite)")]
        public int ConnectionTimeoutSeconds;

        #endregion

        #region Optional

        [Argument(ArgumentType.AtMostOnce, DefaultValue = ExecutionType.Automatic, ShortName = "e", GroupName = "Optional", HelpText = "How to execute the statements")]
        public ExecutionType ExecutionType;

        [Argument(ArgumentType.AtMostOnce, DefaultValue = "~", ShortName = "p", GroupName = "Optional", HelpText = "The text that separates statements")]
        public string Separator;

        [Argument(ArgumentType.AtMostOnce, DefaultValue = 1, ShortName = "c", GroupName = "Optional", HelpText = "How many times to execute the statement(s) (0: forever)")]
        public int ExecutionCount;

        [Argument(ArgumentType.AtMostOnce, DefaultValue = 1, ShortName = "f", GroupName = "Optional", HelpText = "How many times to read each field")]
        public int FieldReadCount;

        [Argument(ArgumentType.AtMostOnce, DefaultValue = false, ShortName = "g", GroupName = "Optional", HelpText = "Debug SQL statements")]
        public bool DebugSQL;

        [Argument(ArgumentType.AtMostOnce, DefaultValue = 0, ShortName = "et", GroupName = "Optional", HelpText = "Execution Timeout seconds (0; infinite)")]
        public int ExecutionTimeoutSeconds;

        [Argument(ArgumentType.AtMostOnce, DefaultValue = false, ShortName = "q", GroupName = "Optional", HelpText = "Quiet Operation")]
        public bool Quiet;

        #endregion

        #region Output

        [Argument(ArgumentType.AtMostOnce, DefaultValue = true, ShortName = "srs", GroupName = "Output", HelpText = "Display Results")]
        public bool ShowResults;

        [Argument(ArgumentType.AtMostOnce, DefaultValue = true, ShortName = "srr", GroupName = "Output", HelpText = "Display Results Row Count")]
        public bool ShowResultsRowCount;

        [Argument(ArgumentType.AtMostOnce, DefaultValue = true, ShortName = "snr", GroupName = "Output", HelpText = "Display No Results Row Count")]
        public bool ShowNoResultsRowCount;

        [Argument(ArgumentType.AtMostOnce, DefaultValue = true, ShortName = "ssv", GroupName = "Output", HelpText = "Display Single Value")]
        public bool ShowSingleValue;

        [Argument(ArgumentType.AtMostOnce, DefaultValue = null, ShortName = "of", GroupName = "Output", HelpText = "The file to write output to")]
        public string OutputFile;

        [Argument(ArgumentType.AtMostOnce, DefaultValue = OutputFormat.ISQL, ShortName = "ofmt", GroupName = "Output", HelpText = "How to format the output")]
        public OutputFormat OutputFormat;

        #endregion

        #region Standalone

        [Argument(ArgumentType.BypassMandatory, ShortName = "?", GroupName = "Standalone", HelpText = "Show the Help page")]
        public bool Help = false;

        [Argument(ArgumentType.BypassMandatory, ShortName = "shc", GroupName = "Standalone", HelpText = "Show the Custom Commands")]
        public bool ShowCommands = false;

        #endregion

        #region Derived Properties

        /// <summary>
        /// Gets a value indicating whether this instance has database.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has database; otherwise, <c>false</c>.
        /// </value>
        public bool HasDatabase
        {
            get { return !string.IsNullOrEmpty(Database) && Database.Trim().Length > 0; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has output file.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has output file; otherwise, <c>false</c>.
        /// </value>
        public bool HasOutputFile
        {
            get { return !string.IsNullOrEmpty(OutputFile) && OutputFile.Trim().Length > 0; }
        }

        #endregion
    }
}
