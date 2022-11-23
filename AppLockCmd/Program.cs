using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NRA.Util;
using NRA.Util.CommandLine;

namespace AppLockCmd
{
    /// <summary>
    /// Program class
    /// </summary>
    class Program
    {
        /// <summary>
        /// Internal arguments field
        /// </summary>
        static Arguments Arguments = new Arguments();

        /// <summary>
        /// Program entry point
        /// </summary>
        /// <param name="args">The args.</param>
        static void Main(string[] args)
        {
            // Process Command Line
            bool argumentsOK = Parser.ParseArguments(args, Arguments, false);
            if (!argumentsOK)
            {
                // Start display
                ConsoleHelper.DisplayHeader();
                ConsoleHelper.Display();

                // Usage
                Parser.ParseArguments(args, Arguments, true);
                ConsoleHelper.Display(Parser.ArgumentsUsage(typeof(Arguments), Console.WindowWidth, Console.WindowWidth / 2));

                return;
            }

            // Start display ?
            if (!Arguments.Quiet)
            {
                ConsoleHelper.DisplayHeader();
                ConsoleHelper.Display();
            }


            ConsoleHelper.Display(string.Format("{0} - {1}", Arguments.LockType, Arguments.Name));

        }
    }
}
