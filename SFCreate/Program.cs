using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NRA.Util;
using NRA.Util.CommandLine;

namespace PFCreate
{
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

            // Create Folder
            try
            {
                string baseFolder = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

                string folderName = Path.Combine(baseFolder, Arguments.FolderName);
                ConsoleHelper.Display(string.Format("Creating: {0}", folderName));
                ConsoleHelper.Display();

                if (Arguments.Confirm)
                {
                    Console.Out.Write("Are you sure you want to continue ? (Y/n)");
                    ConsoleKeyInfo keyInfo = Console.ReadKey(false);
                    ConsoleHelper.Display();

                    bool ok = (keyInfo.Key == ConsoleKey.Y || keyInfo.Key == ConsoleKey.Enter);
                    if (!ok)
                        return;

                    ConsoleHelper.Display();
                }

                // Create
                Directory.CreateDirectory(folderName);
            }
            catch (Exception ex)
            {
                ConsoleHelper.DisplayError(ex.Message);
            }
        }
    }
}
