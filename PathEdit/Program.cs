using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NRA.Util;
using NRA.Util.CommandLine;
using PathEdit.Commands;

namespace PathEdit
{
    /// <summary>
    /// 
    /// </summary>
    class Program
    {
        static Arguments Arguments = new Arguments();

        static IPathCollection _Paths = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        [STAThread]
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

            // Save Parameters
            BaseCommand.ValidateNewPaths = Arguments.EnsureFolderExists;
            BaseCommand.BackupPath = Arguments.Backup;

            // Get Paths
            _Paths = new PathCollection(Arguments.EnvironmentVariable, Arguments.Separator);

            // Process all commands
            CommandResult result = CommandResult.OK();
            do
            {
                bool showList = Arguments.ShowList;
                if ((result.Control & CommandControlType.ShowList) > 0)
                    showList = true;
                if ((result.Control & CommandControlType.SuppressList) > 0)
                    showList = false;

                if (showList)
                    DisplayPaths();

                string cmdName = GetCommand();

                ICommand command = CommandFactory.Create(cmdName);
                if (command == null)
                    result = CommandResult.Warning(string.Format("Command \"{0}\" not known", cmdName), CommandControlType.SuppressList);
                else
                {
                    command.Writer = Console.Out;

                    try
                    {
                        command.Validate(_Paths);

                        result = command.Execute(_Paths);
                    }
                    catch (ValidationError ex)
                    {
                        result = CommandResult.Warning(ex.Message, CommandControlType.SuppressList);
                    }
                    catch (Exception ex)
                    {
                        result = CommandResult.Critical(ex.Message);
                    }
                    finally
                    {
                        if (result == null)
                            result = CommandResult.Failure("No Result !");
                    }
                }

                if (result.HasMessage)
                    BaseCommand.Display(result.Message);

            } while (result.CanContinue);
        }

        /// <summary>
        /// 
        /// </summary>
        static void DisplayPaths()
        {
            PathList.Program.DisplayPaths(_Paths.ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        static string GetCommand()
        {
            ConsoleHelper.Display();
            ConsoleHelper.Display(string.Format("{0}:{1}", AssemblyHelper.GetProgramNameWithoutExtension(), _Paths.IsDirty ? "*" : " "), false);

            return Console.ReadLine();
        }
    }
}
