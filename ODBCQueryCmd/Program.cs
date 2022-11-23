using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.IO;
using System.Text;
using NRA.Util;
using NRA.Util.CommandLine;

// TODO: Add UserName / Password but could be different depending upon ODBC Driver (no specific CSB property)

namespace ODBCQueryCmd
{
    /// <summary>
    ///
    /// </summary>
    class Program
    {
        static Arguments Arguments = new Arguments();

        public const int EXECUTESQL_FAILURE = -1;
        public const string CUSTOMCOMMAND_PREFIX = ":";

        static private TextWriter OutputWriter = Console.Out;

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
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
            else if (Arguments.ShowCommands)
            {
                string header = string.Format("Custom Commands (Prefix with {0})", CUSTOMCOMMAND_PREFIX);

                ConsoleHelper.DisplayError(header);
                ConsoleHelper.DisplayError(new string('-', header.Length));

                foreach (Type t in BaseCommand.GetCustomCommands())
                {
                    ConsoleHelper.DisplayError(string.Format("{0}", BaseCommand.GetCommandName(t)));
                }

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
            Display(string.Format("Connecting to : {0}", Arguments.DSN));
            using (IDbConnection connection = ConnectToDataSource())
            {
                // Use Database ?
                if (Arguments.HasDatabase)
                {
                    Display(string.Format("Using Database: {0}", Arguments.Database));
                    if (ExecuteSQL(connection, string.Format("DATABASE {0}", Arguments.Database), ExecutionType.NoResults, true) == EXECUTESQL_FAILURE)
                        return;
                }
                Display();

                // Use File ?
                if (Arguments.HasOutputFile)
                {
                    try
                    {
                        OutputWriter = new StreamWriter(Arguments.OutputFile, false, Encoding.UTF8, 8192);
                    }
                    catch
                    {
                        ConsoleHelper.DisplayError(string.Format("Unable to write to file: {0}", Arguments.OutputFile));
                        return;
                    }
                }

                // Set Wait Mode
                string waitMode = Arguments.WaitMode
                    ? string.Format("WAIT {0}", Arguments.ConnectionTimeoutSeconds > 0 ? Arguments.ConnectionTimeoutSeconds.ToString() : string.Empty).Trim()
                    : "NOT WAIT";
                Display(string.Format("Setting Lock Mode: {0}", waitMode));
                var ret = ExecuteSQL(connection, string.Format("SET LOCK MODE TO {0}", waitMode), ExecutionType.NoResults, true);
                if (ret != -1)
                {
                    ConsoleHelper.DisplayError(string.Format("Unable to set Lock Mode - {0}", ret));
                    return;
                }
                Display();

                // Get Statements to execute
                string[] splitter = new string[] { Arguments.Separator };

                List<string> statements = new List<string>();
                statements.AddRange(Arguments.SQL.Split(splitter, StringSplitOptions.RemoveEmptyEntries));

                // Loop for each execution count
                int execCount = 0;
                int maxLength = Arguments.ExecutionCount.ToString().Length;
                while ((++execCount <= Arguments.ExecutionCount || Arguments.ExecutionCount <= 0) && !Console.KeyAvailable)
                {
                    // Iterate Statements
                    foreach (string statement in statements)
                    {
                        if (statement.StartsWith(CUSTOMCOMMAND_PREFIX))
                        {
                            ICustomCommand command = BaseCommand.CreateCommand(statement);
                            if (command != null)
                            {
                                if (Arguments.ExecutionCount == 1)
                                    Display(string.Format("CMD: {0}", command.ToString()));
                                else
                                    Display(string.Format("CMD [{1}]: {0}", command.ToString(), execCount.ToString().PadLeft(maxLength, '0')));

                                command.Execute();
                            }
                        }
                        else
                        {
                            if (Arguments.ExecutionCount == 1)
                                Display(string.Format("SQL: {0}", statement));
                            else
                                Display(string.Format("SQL [{1}]: {0}", statement, execCount.ToString().PadLeft(maxLength, '0')));

                            ExecuteSQL(connection, statement);
                        }
                    }
                }

                // Clear Keypresses
                while (Console.KeyAvailable)
                    Console.ReadKey(true);
            }

            if (System.Diagnostics.Debugger.IsAttached)
            {
                ConsoleHelper.DisplayError("Press any key to exit. . .");
                Console.ReadKey();
            }
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
        ///
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="defaultExecutionType"></param>
        /// <returns></returns>
        static private ExecutionType GetExecutionType(string statement, ExecutionType defaultExecutionType)
        {
            if (defaultExecutionType != ExecutionType.Automatic)
                return defaultExecutionType;

            if (statement.Trim().ToUpper().StartsWith("SELECT "))
            {
                string expr = NRA.Util.StringHelper.RemoveStartsWith(statement.Trim().ToUpper(), "SELECT ");

                if (expr.Contains(" FROM "))
                {
                    expr = expr.Substring(0, expr.IndexOf(" FROM ")).Trim();
                    if (expr == "*" || expr.Contains(","))
                        return ExecutionType.Results;
                }

                return ExecutionType.SingleValue;
            }
            else
                return ExecutionType.NoResults;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        static private int CountRows(IDataReader dataReader)
        {
            int rowCount = 0;

            try
            {
                while(dataReader.Read())
                    ++rowCount;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            return rowCount;
        }

        /// <summary>
        /// Displays the data reader.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <param name="rowCount">The row count.</param>
        static private void DisplayDataReader(IDataReader dataReader, out int rowCount)
        {
            rowCount = 0;

            switch (Arguments.OutputFormat)
            {
                case OutputFormat.Tab:
                    DisplayDataReaderTAB(dataReader, out rowCount);
                    break;

                case OutputFormat.CSV:
                    DisplayDataReaderCSV(dataReader, out rowCount);
                    break;

                case OutputFormat.XML:
                    DisplayDataReaderXML(dataReader, out rowCount);
                    break;

                case OutputFormat.ISQL:
                    DisplayDataReaderISQL(dataReader, out rowCount);
                    break;
            }

            OutputWriter.Flush();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dataReader"></param>
        static private void DisplayDataReaderTAB(IDataReader dataReader, out int rowCount)
        {
            rowCount = 0;

            try
            {
                // Header
                for (int i = 0; i < dataReader.FieldCount; ++i)
                {
                    if (i > 0)
                        OutputWriter.Write("\t");

                    OutputWriter.Write(dataReader.GetName(i));
                }
                OutputWriter.WriteLine();

                for (int i = 0; i < dataReader.FieldCount; ++i)
                {
                    if (i > 0)
                        OutputWriter.Write("\t");

                    OutputWriter.Write(new string('-', dataReader.GetName(i).Length));
                }
                OutputWriter.WriteLine();

                // Row Data
                rowCount = 0;
                while(dataReader.Read())
                {
                    ++rowCount;
                    for (int f = 0; f < Arguments.FieldReadCount; ++f)
                    {
                        for (int i = 0; i < dataReader.FieldCount; ++i)
                        {
                            if (i > 0)
                                OutputWriter.Write("\t");

                            object o = GetFieldData(dataReader, i);

                            OutputWriter.Write(o);
                        }
                    }
                    OutputWriter.WriteLine();
                }
            }
            catch(Exception ex)
            {
                ConsoleHelper.DisplayError(ex.Message);
            }
        }

        /// <summary>
        /// Displays the data reader CSV.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <param name="rowCount">The row count.</param>
        static private void DisplayDataReaderCSV(IDataReader dataReader, out int rowCount)
        {
            rowCount = 0;

            try
            {
                // Header
                for (int i = 0; i < dataReader.FieldCount; ++i)
                {
                    if (i > 0)
                        OutputWriter.Write(",");

                    OutputWriter.Write(dataReader.GetName(i));
                }
                OutputWriter.WriteLine();

                // Row Data
                rowCount = 0;
                while(dataReader.Read())
                {
                    ++rowCount;
                    for (int f = 0; f < Arguments.FieldReadCount; ++f)
                    {
                        for (int i = 0; i < dataReader.FieldCount; ++i)
                        {
                            if (i > 0)
                                OutputWriter.Write(",");

                            object o = GetFieldData(dataReader, i);

                            OutputWriter.Write(o);
                        }
                    }
                    OutputWriter.WriteLine();
                }
            }
            catch(Exception ex)
            {
                ConsoleHelper.DisplayError(ex.Message);
            }
        }

        /// <summary>
        /// Displays the data reader XML.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <param name="rowCount">The row count.</param>
        static private void DisplayDataReaderXML(IDataReader dataReader, out int rowCount)
        {
            rowCount = 0;
            int indent = 0;
            string indentString = GetIndentString(indent);

            try
            {
                // Header
                OutputWriter.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");

                OutputWriter.WriteLine(indentString + "<Data>");

                ++indent;
                indentString = GetIndentString(indent);

                // Columns
                OutputWriter.WriteLine(indentString + "<Columns>");

                ++indent;
                indentString = GetIndentString(indent);

                for (int i = 0; i < dataReader.FieldCount; ++i)
                {
                    OutputWriter.WriteLine(indentString + "<Column Name=\"{0}\" Type=\"{1}\" />", dataReader.GetName(i), dataReader.GetFieldType(i).Name);
                }

                --indent;
                indentString = GetIndentString(indent);

                OutputWriter.WriteLine(indentString + "</Columns>");
                OutputWriter.Flush();

                // Rows
                OutputWriter.WriteLine(indentString + "<Rows>");

                ++indent;
                indentString = GetIndentString(indent);

                rowCount = 0;
                while (dataReader.Read())
                {
                    ++rowCount;
                    for (int f = 0; f < Arguments.FieldReadCount; ++f)
                    {
                        if (Arguments.FieldReadCount > 1)
                        {
                            OutputWriter.WriteLine(indentString + "<Iteration Count=\"{0}\">", f);

                            ++indent;
                            indentString = GetIndentString(indent);
                        }

                        OutputWriter.WriteLine(indentString + "<Row Count=\"{0}\">", rowCount);

                        ++indent;
                        indentString = GetIndentString(indent);

                        for (int i = 0; i < dataReader.FieldCount; ++i)
                        {
                            object o = GetFieldData(dataReader, i);

                            OutputWriter.WriteLine(indentString + "<{0}>{1}</{0}>", dataReader.GetName(i), o);
                        }

                        if (Arguments.FieldReadCount > 1)
                        {
                            --indent;
                            indentString = GetIndentString(indent);

                            OutputWriter.WriteLine(indentString + "</Iteration>");
                        }

                        --indent;
                        indentString = GetIndentString(indent);

                        OutputWriter.WriteLine(indentString + "</Row>");
                        OutputWriter.Flush();
                    }
                }

                --indent;
                indentString = GetIndentString(indent);

                OutputWriter.WriteLine(indentString + "</Rows>");

                // RowCount
                OutputWriter.WriteLine(indentString + "<Summary Columns=\"{0}\" Rows=\"{1}\" />", dataReader.FieldCount, rowCount);

                // Footer
                --indent;
                indentString = GetIndentString(indent);

                OutputWriter.WriteLine(indentString + "</Data>");
            }
            catch (Exception ex)
            {
                ConsoleHelper.DisplayError(ex.Message);
            }
        }

        /// <summary>
        /// Displays the data reader ISQL.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <param name="rowCount">The row count.</param>
        static private void DisplayDataReaderISQL(IDataReader dataReader, out int rowCount)
        {
            rowCount = 0;

            List<int> columnSizes = new List<int>();
            StringBuilder headerLine  = new StringBuilder();
            StringBuilder headerNames = new StringBuilder();

            try
            {
                // Build Column Sizes

                // Start with field size
                DataTable schemaTable = dataReader.GetSchemaTable();
                if (schemaTable != null)
                {
                    int rowIndex = 0;
                    foreach (DataRow row in schemaTable.Rows)
                    {
                        while (columnSizes.Count <= rowIndex)
                            columnSizes.Add(0);

                        int columnSize       = Convert.ToInt32(row["ColumnSize"]);
                        int numericPrecision = Convert.ToInt32(row["NumericPrecision"]);

                        columnSizes[rowIndex] = Math.Min(Math.Max(columnSize, numericPrecision), 1000);

                        ++rowIndex;
                    }
                }

                // Apply Column Name size
                for (int i = 0; i < dataReader.FieldCount; ++i)
                {
                    while (columnSizes.Count <= i)
                        columnSizes.Add(0);

                    columnSizes[i] = Math.Max(columnSizes[i], dataReader.GetName(i).Length);
                }

                // Build header lines
                headerLine.Append("+");
                headerNames.Append("|");
                for (int i = 0; i < dataReader.FieldCount; ++i)
                {
                    headerLine.Append(new string('-', columnSizes[i]));
                    headerLine.Append("+");

                    headerNames.Append(dataReader.GetName(i).PadRight(columnSizes[i]));
                    headerNames.Append("|");
                }

                // Header
                OutputWriter.WriteLine(headerLine.ToString());
                OutputWriter.WriteLine(headerNames.ToString());
                OutputWriter.WriteLine(headerLine.ToString());

                // Row Data
                rowCount = 0;
                while(dataReader.Read())
                {
                    ++rowCount;
                    OutputWriter.Write("|");
                    for (int i = 0; i < dataReader.FieldCount; ++i)
                    {
                        object o = GetFieldData(dataReader, i);

                        OutputWriter.Write(Convert.ToString(o).PadRight(columnSizes[i]));
                        OutputWriter.Write("|");
                    }
                    OutputWriter.WriteLine();
                }

                OutputWriter.WriteLine(headerLine.ToString());
            }
            catch(Exception ex)
            {
                ConsoleHelper.DisplayError(ex.Message);
            }
        }

        /// <summary>
        /// Gets the field data.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <returns></returns>
        static private object GetFieldData(IDataReader dataReader, int columnIndex)
        {
            object o = null;

            try
            {
                o = dataReader.GetValue(columnIndex);

                if (o == null || o == DBNull.Value)
                    o = "NULL";
            }
            catch
            {
                try
                {
                    o = dataReader.GetString(columnIndex);
                }
                catch
                {
                    o = "NULL";
                }
            }

            if (o is Array)
            {
                Array a = o as Array;

                StringBuilder sb = new StringBuilder();

                if (a != null)
                {
                    sb.AppendFormat("[{0}] {{ ", a.Length);

                    bool sep = false;
                    foreach (object e in a)
                    {
                        if (sep)
                            sb.Append(", ");

                        sb.AppendFormat(Convert.ToString(e));

                        sep = true;
                    }

                    sb.Append(" }}");
                }

                o = sb.ToString();
            }

            return o;
        }

        /// <summary>
        /// Gets the indent string.
        /// </summary>
        /// <param name="indent">The indent.</param>
        /// <returns></returns>
        static private string GetIndentString(int indent)
        {
            return new string(' ', indent * 2);
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
        ///
        /// </summary>
        /// <param name="s"></param>
        static private void Display(string s)
        {
            if (!Arguments.Quiet)
                ConsoleHelper.Display(string.Format("{0}: {1}", DateTime.Now.ToLongTimeString(), s));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="statement"></param>
        /// <returns></returns>
        static private int ExecuteSQL(IDbConnection connection, string statement)
        {
            return ExecuteSQL(connection, statement, GetExecutionType(statement, Arguments.ExecutionType), false);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="statement"></param>
        static private int ExecuteSQL(IDbConnection connection, string statement, ExecutionType execType, bool silent)
        {
            int rowCount = EXECUTESQL_FAILURE;


            string rowFilter = string.Empty;

            try
            {
                if (statement.Contains("~"))
                {
                    string[] bits = statement.Split("~".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                    if (bits.Length > 0)
                        statement = bits[0];
                    if (bits.Length > 1)
                        rowFilter = bits[1];
                }

                using (IDbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = statement;
                    if (Arguments.DebugSQL)
                        Display(string.Format("DEBUG: {0}", cmd.CommandText));

                    switch (execType)
                    {
                        case ExecutionType.Results:
                            IDataReader dataReader = cmd.ExecuteReader();

                            if (Arguments.ShowResults)
                                DisplayDataReader(dataReader, out rowCount);
                            else
                                rowCount = CountRows(dataReader);

                            if (Arguments.ShowResultsRowCount && !silent)
                                Display(string.Format("{0} rows affected", rowCount));

                            break;

                        case ExecutionType.SingleValue:
                            object scalar = cmd.ExecuteScalar();
                            rowCount = 1;

                            if (Arguments.ShowSingleValue && !silent)
                                Display(Convert.ToString(scalar));

                            break;

                        case ExecutionType.NoResults:
                            rowCount = cmd.ExecuteNonQuery();
                            if (Arguments.ShowNoResultsRowCount && !silent)
                                Display(string.Format("{0} rows affected", rowCount));

                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.DisplayError(ex.Message);
                rowCount = EXECUTESQL_FAILURE;
            }

            return rowCount;
        }
    }
}
