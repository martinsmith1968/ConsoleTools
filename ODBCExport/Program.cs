using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.IO;
using System.Text;
using NRA.Util;
using NRA.Util.CommandLine;

namespace ODBCExport
{
    /// <summary>
    /// Program Container
    /// </summary>
    class Program
    {
        static Arguments Arguments = new Arguments();

        public const int    EXECUTESQL_FAILURE   = -1;
        public const string CUSTOMCOMMAND_PREFIX = ":";

        public const string COMMENTLINE = "--------------------------------------------------------------------------------";

        public const string COMMENTPREFIX = "--";

        static private string lastOutputFileName = string.Empty;

        /// <summary>
        /// Mains entry point.
        /// </summary>
        /// <param name="args">The args.</param>
        static void Main(string[] args)
        {
            // Process Command Line
            bool argumentsOK = Parser.ParseArguments(args, Arguments, false);

            // Start display
            if (!Arguments.Quiet)
            {
                ConsoleHelper.DisplayHeader();
                ConsoleHelper.Display();
            }

            // Standalone Commands
            if (Arguments.Help)
            {
                // Usage
                ConsoleHelper.DisplayError(Parser.ArgumentsUsage(typeof(Arguments), Console.WindowWidth, 45), false);

                return;
            }
            // Invalid Command Line ?
            else if (!argumentsOK)
            {
                // Usage
                Parser.ParseArguments(args, Arguments, true);   // Show Errors
                ConsoleHelper.DisplayError(Parser.ArgumentsUsage(typeof(Arguments), Console.WindowWidth, 45), false);
                return;
            }

            // Connect
            Display(string.Format("Connecting to: {0}...", Arguments.DSN), false);
            using (IDbConnection connection = ConnectToDataSource())
            {
                Display(connection == null ? "Failed!" : "OK", true);

                // Use Database ?
                if (Arguments.HasDatabase && Arguments.AllowDBSwitch)
                {
                    Display(string.Format("Using Database: {0}", Arguments.Database));
                    if (ExecuteSQL(connection, string.Format("DATABASE {0}", Arguments.Database)) == EXECUTESQL_FAILURE)
                    {
                        try
                        {
                            connection.ChangeDatabase(Arguments.Database);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex.Message);
                            ConsoleHelper.DisplayError(string.Format("Unable to select Database: {0}", Arguments.Database));
                            return;
                        }
                    }
                }
                Display();

                // Build the list of Table Names to export
                List<string> tableNames = new List<string>();
                if (Arguments.TableName.Contains("|"))
                {
                    tableNames.AddRange(Arguments.TableName.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
                }
                else if (Arguments.TableName.Contains(","))
                {
                    tableNames.AddRange(Arguments.TableName.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
                }
                else
                {
                    tableNames.Add(Arguments.TableName);
                }

                // Iterate Table Name
                foreach (string tableName in tableNames)
                {
                    DumpTable(connection, tableName);
                }




            }

            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Clear Keypresses
                while (Console.KeyAvailable)
                    Console.ReadKey(true);
                ConsoleHelper.Display("Press any key to exit. . .");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Dumps the table.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="tableName">Name of the table.</param>
        static private void DumpTable(IDbConnection connection, string tableName)
        {
            Display(string.Format("Table: {0}", tableName));

            bool newFile = false;

            TextWriter OutputWriter = Console.Out;
            if (Arguments.HasOutputFile)
            {
                string outputFileName = Arguments.OutputFile;

                outputFileName = outputFileName.Replace("{database}", Arguments.Database);
                outputFileName = outputFileName.Replace("{table}", tableName);
                outputFileName = outputFileName.Replace("{format}", Arguments.OutputFormat.ToString());

                newFile = (outputFileName != lastOutputFileName);
                OutputWriter = new StreamWriter(outputFileName, !newFile);

                lastOutputFileName = outputFileName;
            }

            using (OutputWriter)
            {
                if (newFile)
                {
                    WriteFileHeader(OutputWriter);
                }

                WriteTableHeader(OutputWriter, tableName);

                string sql = string.Format("SELECT * FROM {0}{1}{2}",
                    Arguments.Database,
                    string.IsNullOrEmpty(Arguments.Database) ? string.Empty : ":",
                    tableName
                    );
                if (Arguments.HasWhere)
                {
                    sql = string.Format("{0} WHERE {1}", sql, Arguments.Where);
                }
                IDataReader reader = ExecuteQuery(connection, sql);

                if (reader != null)
                {
                    bool doneColumns = false;

                    while (reader.Read())
                    {
                        if (!doneColumns)
                        {
                            WriteTableColumns(OutputWriter, tableName, reader);
                            doneColumns = true;
                        }

                        WriteTableRow(OutputWriter, tableName, reader);
                    }
                }

                WriteTableTrailer(OutputWriter, tableName);

                OutputWriter.Flush();
            }
        }

        /// <summary>
        /// Writes the file header.
        /// </summary>
        /// <param name="writer">The writer.</param>
        static private void WriteFileHeader(TextWriter OutputWriter)
        {
            switch (Arguments.OutputFormat)
            {
                case OutputFormat.SQL:
                    OutputWriter.WriteLine(COMMENTLINE);
                    OutputWriter.WriteLine("{0} Database: {1}", COMMENTPREFIX, Arguments.Database);
                    OutputWriter.WriteLine(COMMENTPREFIX);
                    OutputWriter.WriteLine("{0} Date    : {1} {2}", COMMENTPREFIX, DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString());
                    OutputWriter.WriteLine(COMMENTLINE);
                    break;

                case OutputFormat.XML:
                    OutputWriter.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                    break;
            }
        }

        /// <summary>
        /// Writes the table header.
        /// </summary>
        /// <param name="OutputWriter">The output writer.</param>
        static private void WriteTableHeader(TextWriter OutputWriter, string tableName)
        {
            switch (Arguments.OutputFormat)
            {
                case OutputFormat.SQL:
                    OutputWriter.WriteLine();
                    OutputWriter.WriteLine(COMMENTLINE);
                    OutputWriter.WriteLine("{0} Table: {1}", COMMENTPREFIX, tableName);
                    OutputWriter.WriteLine(COMMENTLINE);
                    OutputWriter.WriteLine();

                    switch (Arguments.SQLDeleteType)
                    {
                        case SQLDeleteType.Truncate:
                            OutputWriter.WriteLine("TRUNCATE TABLE {0};", tableName);
                            break;

                        case SQLDeleteType.Delete:
                            OutputWriter.Write("DELETE FROM {0};", tableName);
                            if (Arguments.HasWhere)
                            {
                                OutputWriter.Write(" WHERE {1};", Arguments.Where);
                            }
                            OutputWriter.WriteLine();
                            break;
                    }

                    break;

                case OutputFormat.XML:
                    OutputWriter.WriteLine("<{0}>", tableName);
                    break;
            }
        }

        /// <summary>
        /// Writes the table trailer.
        /// </summary>
        /// <param name="OutputWriter">The output writer.</param>
        /// <param name="tableName">Name of the table.</param>
        static private void WriteTableTrailer(TextWriter OutputWriter, string tableName)
        {
            switch (Arguments.OutputFormat)
            {
                case OutputFormat.XML:
                    OutputWriter.WriteLine(@"<\{0}>", tableName);
                    break;

            }
        }

        /// <summary>
        /// Writes the table columns.
        /// </summary>
        /// <param name="OutputWriter">The output writer.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="reader">The reader.</param>
        static private void WriteTableColumns(TextWriter OutputWriter, string tableName, IDataReader reader)
        {
            switch (Arguments.OutputFormat)
            {
                case OutputFormat.CSV:
                    for (int i = 0; i < reader.FieldCount; ++i)
                    {
                        if (i > 0)
                            OutputWriter.Write(",");
                        OutputWriter.Write(reader.GetName(i));
                    }
                    OutputWriter.WriteLine();
                    break;

                case OutputFormat.Tab:
                    break;
            }
        }

        /// <summary>
        /// Writes the table row.
        /// </summary>
        /// <param name="OutputWriter">The output writer.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="reader">The reader.</param>
        static private void WriteTableRow(TextWriter OutputWriter, string tableName, IDataReader reader)
        {
            switch (Arguments.OutputFormat)
            {
                case OutputFormat.SQL:
                    OutputWriter.Write("INSERT INTO {0} (", tableName);
                    break;
            }

            for (int i = 0; i < reader.FieldCount; ++i)
            {
                switch (Arguments.OutputFormat)
                {
                    case OutputFormat.SQL:
                        if (i > 0)
                            OutputWriter.Write(", ");
                        OutputWriter.Write(reader.GetName(i));
                        break;
                }
            }

            switch (Arguments.OutputFormat)
            {
                case OutputFormat.SQL:
                    OutputWriter.Write(") VALUES (");
                    break;
            }

            for (int i = 0; i < reader.FieldCount; ++i)
            {
                string value = reader.GetValue(i) == DBNull.Value
                    ? null
                    : Convert.ToString(reader.GetValue(i));

                if (value != null)
                {
                    try
                    {
                        if (reader.GetFieldType(i) == typeof(DateTime))
                        {
                            value = Convert.ToDateTime(reader.GetValue(i))
                                .ToString("yyyy-MM-dd HH:mm:ss.fff")
                                .TrimEnd("0".ToCharArray())
                                .TrimEnd(".".ToCharArray())
                                ;
                        }
                    }
                    catch
                    {
                    }
                }

                switch (Arguments.OutputFormat)
                {
                    case OutputFormat.CSV:
                        if (i > 0)
                            OutputWriter.Write(",");
                        OutputWriter.Write(value);
                        break;

                    case OutputFormat.SQL:
                        if (i > 0)
                            OutputWriter.Write(", ");

                        bool quotes = false;
                        if (value == null)
                        {
                            value = "NULL";
                        }
                        else
                        {
                            quotes = value.Length == 0 || !IsNumeric(value);
                        }

                        if (quotes)
                            OutputWriter.Write(Arguments.QuoteChar);
                        OutputWriter.Write(value);
                        if (quotes)
                            OutputWriter.Write(Arguments.QuoteChar);
                        break;
                }
            }

            switch (Arguments.OutputFormat)
            {
                case OutputFormat.SQL:
                    OutputWriter.WriteLine(");");
                    break;
            }
        }

        static private bool IsNumeric(string value)
        {
            int    intValue    = 0;
            double doubleValue = 0;

            if (int.TryParse(value, out intValue))
                return true;

            if (double.TryParse(value, out doubleValue))
                return true;

            return false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        static private string GetConnectionString()
        {
            OdbcConnectionStringBuilder csb = new OdbcConnectionStringBuilder();

            csb.Dsn = Arguments.DSN;
            if (!string.IsNullOrEmpty(Arguments.Database))
            {
                csb.Add("Database", Arguments.Database);
            }

            return csb.ConnectionString;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        static private IDbConnection ConnectToDataSource()
        {
            try
            {
                IDbConnection conn = new OdbcConnection(GetConnectionString());
                conn.Open();

                return conn;
            }
            catch (Exception ex)
            {
                ConsoleHelper.DisplayError(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Executes the SQL.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="statement">The statement.</param>
        /// <returns></returns>
        static private int ExecuteSQL(IDbConnection connection, string statement)
        {
            try
            {
                using (IDbCommand cmd = connection.CreateCommand())
                {
                    //cmd.CommandType    = CommandType.Text;
                    cmd.CommandText    = statement;
                    //cmd.CommandTimeout = Arguments.ExecutionTimeoutSeconds;

                    return cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return EXECUTESQL_FAILURE;
            }
        }

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="statement">The statement.</param>
        /// <returns></returns>
        static private IDataReader ExecuteQuery(IDbConnection connection, string statement)
        {
            try
            {
                IDbCommand cmd = connection.CreateCommand();
                    //cmd.CommandType    = CommandType.Text;
                    cmd.CommandText    = statement;
                    //cmd.CommandTimeout = Arguments.ExecutionTimeoutSeconds;

                    return cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Displays this instance.
        /// </summary>
        static private void Display()
        {
            if (!Arguments.Quiet)
                ConsoleHelper.Display();
        }

        /// <summary>
        /// Displays the specified s.
        /// </summary>
        /// <param name="s">The s.</param>
        static private void Display(string s)
        {
            Display(s, true);
        }

        /// <summary>
        /// Displays the specified s.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="newline">if set to <c>true</c> [newline].</param>
        static private void Display(string s, bool newline)
        {
            if (!Arguments.Quiet)
                ConsoleHelper.Display(string.Format("{1}", DateTime.Now.ToLongTimeString(), s), newline);
        }
    }
}
