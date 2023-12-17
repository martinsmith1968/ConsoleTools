using NRA.Util;
using NRA.Util.CommandLine;

namespace PathList
{
    /// <summary>
    ///
    /// </summary>
    public class Program
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

            ConsoleHelper.Display(string.Format("Expanding: {0}", Arguments.EnvironmentVariable));
            ConsoleHelper.Display();

            string variableContents = Environment.GetEnvironmentVariable(Arguments.EnvironmentVariable);
            if (!string.IsNullOrEmpty(variableContents))
            {
                string[] paths = variableContents.Split(Arguments.Separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                DisplayPaths(paths);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="paths"></param>
        static public void DisplayPaths(string[] paths)
        {
            int maxPad = paths.Length.ToString().Length;

            int count = 0;
            foreach (string path in paths)
            {
                string existsChar = " ";
                if (Arguments.CheckFolderExists && !Directory.Exists(path))
                    existsChar = "*";

                ConsoleHelper.Display(string.Format("{0}:{1}{2}", (++count).ToString().PadLeft(maxPad), existsChar, path));
            }
        }
    }
}
