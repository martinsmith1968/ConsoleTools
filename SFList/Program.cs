using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NRA.Util;
using NRA.Util.CommandLine;

namespace SFList
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

            // Get list of Paths
            IDictionary<string, string> specialFolders = GetSpecialFolders();

            // Sort ?
            if (Arguments.SortType != SortType.None)
            {
                IComparer<string> comparer = null;
                if (Arguments.SortType == SortType.Asc)
                    comparer = StringComparer.CurrentCultureIgnoreCase;
                else
                    comparer = new StringComparerReverse(StringComparer.CurrentCultureIgnoreCase);

                SortedDictionary<string, string> sortedSpecialFolders = new SortedDictionary<string ,string>(comparer);

                foreach (string folder in specialFolders.Keys)
                    sortedSpecialFolders.Add(folder, specialFolders[folder]);

                specialFolders = sortedSpecialFolders;
            }

            // Get Metrics
            int maxLengthName = specialFolders.Keys.Max(k => k.ToString().Length);
            int maxLengthPath = specialFolders.Values.Max(v => v.Length);

            string separator = " = ";

            // Display
            foreach (string specFolder in specialFolders.Keys)
            {
                ConsoleHelper.Display(
                    string.Format("{0}{1}{2}",
                        specFolder.PadRight(maxLengthName),
                        separator,
                        specialFolders[specFolder])
                    );
            }

            


        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        static IDictionary<string, string> GetSpecialFolders()
        {
            Dictionary<string, string> specialFolders = new Dictionary<string, string>();

            foreach (Environment.SpecialFolder specialFolderType in Enum.GetValues(typeof(Environment.SpecialFolder)))
            {
                string folderKey = specialFolderType.ToString();

                if (!specialFolders.ContainsKey(folderKey))
                    specialFolders.Add(folderKey, Environment.GetFolderPath(specialFolderType));
            }

            return specialFolders;
        }
    }
}
