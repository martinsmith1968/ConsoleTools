using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using NRA.Util;
using NRA.Util.CommandLine;

namespace TimerCmd
{
    /// <summary>
    ///
    /// </summary>
    class Program
    {
        /// <summary>
        ///
        /// </summary>
        static Arguments Arguments = new Arguments();

        #region Settings

        /// <summary>
        /// Loads the settings.
        /// </summary>
        static void LoadSettings()
        {
            if (Settings.Default.IsUpgraded)
            {
                Settings.Default.Upgrade();

                Settings.Default.IsUpgraded = false;
                Settings.Default.Save();
            }

            if (Settings.Default.TimerList == null)
                Settings.Default.TimerList = new System.Collections.Specialized.StringCollection();
        }

        /// <summary>
        /// Saves the settings.
        /// </summary>
        static void SaveSettings()
        {
            Settings.Default.Save();
        }

        #endregion

        /// <summary>
        /// Mains the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        static void Main(string[] args)
        {
            TextWriterTraceListener tr1 = new TextWriterTraceListener(Console.Out);
#if HELPME
            Debug.Listeners.Add(tr1);
#endif

            // Process Command Line
            bool argumentsOK = Parser.ParseArguments(args, Arguments, false);
#if !HELPME
            if (Arguments.Debug)
            {
                Debug.Listeners.Add(tr1);
            }
#endif
            if (!argumentsOK)
            {
                // Start display
                ConsoleHelper.DisplayHeader();
                ConsoleHelper.Display();

                int timerCount = Timer.Count;

                TimerOperation operation = TimerOperation.Help;

                if (Arguments.Help)
                {
                    operation = TimerOperation.Help;
                }
                else if (Arguments.ClearAll)
                {
                    operation = TimerOperation.ClearAll;
                }
                else if (Arguments.List)
                {
                    operation = TimerOperation.List;
                }
                else if (timerCount > 0)
                {
                    operation = TimerOperation.List;
                }
                else
                {
                    operation = TimerOperation.Help;
                }

                // Process standalone arguments
                switch (operation)
                {
                    case TimerOperation.ClearAll:
                    case TimerOperation.List:
                        PerformOperation(operation);
                        break;

                    default:
                        // Usage
                        Parser.ParseArguments(args, Arguments, true);
                        ConsoleHelper.Display(Parser.ArgumentsUsage(typeof(Arguments), Console.WindowWidth, Console.WindowWidth / 2));
                        break;
                }

                return;
            }

            // Start display ?
            if (!Arguments.Quiet)
            {
                ConsoleHelper.DisplayHeader();
                ConsoleHelper.Display();
            }

            // Get User Config
            LoadSettings();

            // Load Timer
            Timer timer = Timer.Get(Arguments.Name);


            // Is Timer required ?
            bool timerRequired = true;
            switch (Arguments.Command)
            {
                case TimerCommand.Start:
                    timerRequired = false;
                    break;
            }

            if (timerRequired && timer == null)
            {
                ConsoleHelper.DisplayError(string.Format("Timer not found: {0}", Arguments.Name));
                return;
            }
            else if (!timerRequired && timer != null && !Arguments.Reuse)
            {
                ConsoleHelper.DisplayError(string.Format("Timer already exists: {0}", Arguments.Name));
                return;
            }

            // Act on timer
            try
            {
                switch (Arguments.Command)
                {
                    case TimerCommand.Start:
                        if (timer != null)
                        {
                            timer.Delete();
                        }

                        timer = new Timer(Arguments.Name, false);
                        timer.Start();

                        Settings.Default.TimerList.Add(timer.ToText());

                        break;

                    case TimerCommand.Stop:
                        ConsoleHelper.Display(string.Format("{0} finished: {1}", timer.Name, timer.ElapsedTimeText));
                        timer.Stop();
                        break;

                    case TimerCommand.Reset:
                        timer.Reset();
                        break;

                    case TimerCommand.Pause:
                        timer.Pause();
                        break;

                    case TimerCommand.Delete:
                        timer.Delete();
                        break;

                    case TimerCommand.Elapsed:
                        ConsoleHelper.Display(string.Format("{0} elapsed: {1}", timer.Name, timer.ElapsedTimeText));
                        break;
                }

                SaveSettings();
            }
            catch (Exception ex)
            {
                ConsoleHelper.DisplayError(string.Format("Error: {0}", ex.Message));
            }
        }

        /// <summary>
        /// Performs the operation.
        /// </summary>
        /// <param name="operation">The operation.</param>
        static void PerformOperation(TimerOperation operation)
        {
            Trace.WriteLine(string.Format("Performing operation: {0}", operation));
            switch (operation)
            {
                case TimerOperation.List:
                    {
                        int count = Timer.Count;

                        if (count > 0)
                        {
                            ConsoleHelper.Display("Timers");
                            ConsoleHelper.Display("------");

                            Timer[] timers = Timer.GetAll();
                            if (timers != null)
                            {
                                int posLength = timers.Length.ToString().Length;
                                int nameLength = timers.Max(t => t.Name.Length);
                                int statusLength = timers.Max(t => t.Status.ToString().Length);

                                int pos = 0;
                                foreach (Timer timer in timers)
                                {
                                    ++pos;
                                    ConsoleHelper.Display(string.Format("{0}: {1}\t{2} {3}\t{4}\t{5}",
                                        pos.ToString().PadLeft(posLength),
                                        timer.Name.PadRight(nameLength),
                                        timer.StartTime.ToShortDateString(),
                                        timer.StartTime.ToShortTimeString(),
                                        timer.Status.ToString().PadRight(statusLength),
                                        timer.ElapsedTimeText
                                        ));
                                }
                            }
                        }

                        Console.Out.WriteLine("{0} timers found", count);
                    }
                    break;

                case TimerOperation.ClearAll:
                    Settings.Default.TimerList = new System.Collections.Specialized.StringCollection();
                    SaveSettings();

                    PerformOperation(TimerOperation.List);
                    break;
            }
        }
    }
}
