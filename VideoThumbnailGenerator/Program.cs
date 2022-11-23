using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NRA.Util;
using NRA.Util.CommandLine;

namespace VideoThumbnailGenerator
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



        }
    }
}
