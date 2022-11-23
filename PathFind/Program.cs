using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NRA.Util;
using NRA.Util.CommandLine;

namespace PathFind
{
    class Program
    {
        /// <summary>
        /// The Program Arguments
        /// </summary>
        static Arguments Arguments = new Arguments();

        /// <summary>
        /// Mains the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
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

            ConsoleHelper.Display(string.Format("Searching: {0}", Arguments.EnvironmentVariable));
            ConsoleHelper.Display();

            string variableContents = Environment.GetEnvironmentVariable(Arguments.EnvironmentVariable);
            if (!string.IsNullOrEmpty(variableContents))
            {
                string[] paths = variableContents.Split(Arguments.Separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                foreach (string path in paths)
                {
                    FindFile(path, Arguments.FileName);
                }
            }
            else
            {
                ConsoleHelper.DisplayError(string.Format("Invalid or empty {0}", Arguments.EnvironmentVariable));
            }
        }

        /// <summary>
        /// Displays the paths.
        /// </summary>
        /// <param name="paths">The paths.</param>
        static public void FindFile(string path, string fileName)
        {
            ConsoleHelper.Display(path, false);

            if (!Directory.Exists(path))
            {
                ConsoleHelper.DisplayError(" - Folder does not exist : {0}");
            }
            else
            {
                ConsoleHelper.Display(string.Empty);
                string[] files = Directory.GetFiles(path, fileName);

                foreach (string file in files)
                {
                    FileInfo fi = new FileInfo(file);

                    ConsoleHelper.Display(string.Format("  {0}\t{1}\t{2}", fi.Name, fi.Length, fi.LastWriteTime));
                }
            }
        }
    }
}
