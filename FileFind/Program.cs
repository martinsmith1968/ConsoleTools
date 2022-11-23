using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NRA.Util;
using NRA.Util.CommandLine;

namespace FileFind
{
    /// <summary>
    ///
    /// </summary>
    class Program
    {
        /// <summary>
        /// Program Arguments
        /// </summary>
        static Arguments Arguments = new Arguments();

        static int currentPathPointX = 0;
        static int currentPathPointY = 0;
        static int currentPathLength = 0;

        /// <summary>
        /// A delegate for a event fired when a file is found
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        delegate void FileFound(string fileName);

        /// <summary>
        /// Mains the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        static void Main(string[] args)
        {
            // Process Command Line
            bool argumentsOK = Parser.ParseArguments(args, Arguments);

            // Fixup Arguments (provide a shortcut to specifying certain arguments)
            Arguments.Fixup();

            // Show Help ?
            if (!argumentsOK || Arguments.Help || Arguments.NameCount == 0)
            {
                // Usage
                ConsoleHelper.DisplayHeader();
                ConsoleHelper.Display();
                ConsoleHelper.Display(Parser.ArgumentsUsage(typeof(Arguments), Console.WindowWidth));
                return;
            }

            // Display Header
            if (!Arguments.Quiet)
            {
                // Start display
                ConsoleHelper.DisplayHeader();
                ConsoleHelper.Display();

                // Display what we're going to do
                ConsoleHelper.Display(string.Format("Searching: {0}", Arguments.FullPath));
                for (int i = 0; i < Arguments.NameCount; ++i)
                {
                    ConsoleHelper.Display(string.Format("{0}: {1}", (i == 0 ? "For" : "   ").PadRight(9), Arguments.Name[i]));
                }
                ConsoleHelper.Display();
            }

            if (Arguments.Progress)
            {
                currentPathPointX = ConsoleHelper.CurrentX;
                currentPathPointY = ConsoleHelper.CurrentY;
            }

            // Search
            FindFiles(Arguments.FullPath, Arguments.Name, FileFoundEvent);
        }

        /// <summary>
        /// Files the found.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        private static void FileFoundEvent(string fileName)
        {
            ConsoleHelper.Display(fileName);

            if (Arguments.Progress)
            {
                currentPathPointY = ConsoleHelper.CurrentY;
            }
        }

        /// <summary>
        /// Finds the files.
        /// </summary>
        private static void FindFiles(string path, string[] fileNames, FileFound method)
        {
            // Show current folder ?
            if (Arguments.Progress)
            {
                /*
                string blanker = new string(' ', currentPathLength);

                ConsoleHelper.DisplayAt(currentPathPointY, currentPathPointX, blanker);
                 * */

                string pathDisplay = path.PadRight(currentPathLength, ' ');

                ConsoleHelper.DisplayAt(currentPathPointY, currentPathPointX, pathDisplay);

                currentPathLength = string.IsNullOrEmpty(path) ? 0 : path.Length;
            }

            // Search for files in this path
            foreach (string pattern in fileNames)
            {
                string[] files = Directory.GetFiles(path, pattern, SearchOption.TopDirectoryOnly);

                if (files != null && files.Length > 0)
                {
                    foreach (string file in files)
                    {
                        method(file);
                    }
                }
            }

            // Find all subfolders
            string[] folders = Directory.GetDirectories(path, "*.*", SearchOption.TopDirectoryOnly);

            foreach (string folder in folders)
            {
                FindFiles(folder, fileNames, method);
            }
        }
    }
}
