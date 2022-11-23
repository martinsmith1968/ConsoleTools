using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NRA.Util;
using NRA.Util.CommandLine;

// TODO: Allow specifying a value separator to break values more sensibly

namespace SETList
{
    #region String Comparer Reverse

    class StringComparerReverse : StringComparer
    {
        private IComparer _Base;

        public StringComparerReverse(IComparer baseComparer)
        {
            _Base = baseComparer;
        }

        public override int Compare(string x, string y)
        {
            return _Base.Compare(x, y) * -1;
        }

        public override bool Equals(string x, string y)
        {
            return x.Equals(y);
        }

        public override int GetHashCode(string obj)
        {
            return obj.GetHashCode();
        }
    }

    #endregion

    /// <summary>
    /// 
    /// </summary>
    class Program
    {
        static Arguments Arguments = new Arguments();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // Start display
            ConsoleHelper.DisplayHeader();
            ConsoleHelper.Display();

            // Process Command Line
            bool argumentsOK = Parser.ParseArguments(args, Arguments);
            if (!argumentsOK || Arguments.Help)
            {
                // Usage
                ConsoleHelper.Display(Parser.ArgumentsUsage(typeof(Arguments), Console.WindowWidth));
                return;
            }

            // Get Environment variables
            IDictionary variableList = Environment.GetEnvironmentVariables();
            
            // Sort ?
            if (Arguments.SortType != SortType.None)
            {
                IComparer<string> comparer = null;
                if (Arguments.SortType == SortType.Asc)
                    comparer = StringComparer.CurrentCultureIgnoreCase;
                else
                    comparer = new StringComparerReverse(StringComparer.CurrentCultureIgnoreCase);

                SortedDictionary<string, object> sortedVariableList = new SortedDictionary<string, object>(comparer);

                foreach (string name in variableList.Keys)
                    sortedVariableList.Add(name, variableList[name]);

                variableList = sortedVariableList;
            }

            // Find largest Variable Name
            int maxlengthName = Convert.ToInt32(Arguments.VariableNameWidth);
            if (maxlengthName < 0)
            {
                foreach (string variableName in variableList.Keys)
                {
                    maxlengthName = Math.Max(variableName.Length, maxlengthName);
                }
            }

            // Determine how much space we have for the value
            int maxlengthValue = Console.WindowWidth - maxlengthName - 3;   // 2 spaces and =

            // Display
            foreach (string variableName in variableList.Keys)
            {
                string name = variableName.PadRight(maxlengthName);
                string nameEmpty = new string(' ', maxlengthName);
                string value = Convert.ToString(variableList[variableName]);
                if (value == null)
                    value = string.Empty;
                string separator = "=";

                // Display Name
                Console.Out.Write(name);
                if (variableName.Length > maxlengthName)
                {
                    Console.Out.WriteLine();
                    Console.Out.Write(string.Empty.PadRight(maxlengthName));
                }

                // Break values into a list
                List<string> valueList = new List<string>();
                if (Arguments.HasValueSeparator)
                {
                    foreach (string valueSplitEntry in value.Split(Arguments.ValueSeparatorChar))
                    {
                        string valueSplit = valueSplitEntry;

                        while (valueSplit.Length > maxlengthValue)
                        {
                            valueList.Add(valueSplit.Substring(0, maxlengthValue));
                            valueSplit = valueSplit.Substring(maxlengthValue);
                        }
                        if (valueSplit.Length > 0)
                            valueList.Add(valueSplit);
                    }
                }
                else
                {
                    while (value.Length > maxlengthValue)
                    {
                        valueList.Add(value.Substring(0, maxlengthValue));
                        value = value.Substring(maxlengthValue);
                    }
                    if (value.Length > 0)
                        valueList.Add(value);
                }

                // Display each
                string namePadder = string.Empty;
                foreach(string valueEntry in valueList)
                {
                    if (valueEntry.Length < maxlengthValue)
                    {
                        Console.Out.WriteLine("{0} {1} {2}", namePadder, separator, valueEntry);
                    }
                    else
                    {
                        Console.Out.Write("{0} {1} {2}", namePadder, separator, valueEntry);
                    }

                    separator = " ";
                    namePadder = nameEmpty;
                }
            }
        }
    }
}
