using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NRA.Util;
using NRA.Util.CommandLine;

namespace TextViewer
{
    class Program
    {
        static Arguments Arguments = new Arguments();

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // Process Command Line
            bool argumentsOK = Parser.ParseArguments(args, Arguments);
            if (!argumentsOK || Arguments.Help)
            {
                // Usage
                ConsoleHelper.DisplayHeader();
                ConsoleHelper.Display();
                ConsoleHelper.Display(Parser.ArgumentsUsage(typeof(Arguments), Console.WindowWidth));
                return;
            }

            // Check File Exists
            if (!File.Exists(Arguments.FileName))
            {
                ConsoleHelper.DisplayHeader();
                ConsoleHelper.Display();
                ConsoleHelper.DisplayError(string.Format("File: {0} does not exist or cannot be opened !", Arguments.FileName));
                return;
            }

            // Setup screen
            using (Viewer viewer = new Viewer())
            {
                viewer.CursorVisible = false;
                viewer.LoadFile(Arguments.FileName);
                viewer.CurrentLine = Arguments.Line;
                viewer.CurrentCol = Arguments.Column;
                viewer.Display();
            }
            Console.Clear();
        }
    }
}
