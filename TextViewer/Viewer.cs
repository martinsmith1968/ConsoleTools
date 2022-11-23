// <copyright file="Viewer.cs" company="Thales e-Security Ltd">
// Copyright (c) 2010 Thales e-Security Ltd
// All rights reserved. Company confidential.
// </copyright>
//
// TODO: [Description of Viewer]

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NRA.Util;

namespace TextViewer
{
    /// <summary>
    ///
    /// </summary>
    public enum MessageType
    {
        Info,
        Error
    }

    /// <summary>
    ///
    /// </summary>
    public struct SearchResult
    {
        public int Line;
        public int Column;
        public bool Found;

        public SearchResult(int line, int column)
        {
            Line = line;
            Column = column;
            Found = true;
        }

        static public SearchResult Empty
        {
            get
            {
                return new SearchResult();
            }
        }
    }

    /// <summary>
    /// TODO: [Description of Viewer]
    /// </summary>
    public class Viewer : IDisposable
    {
        #region Constants

        /// <summary>
        /// Gets the help text.
        /// </summary>
        private static string[] HelpText
        {
            get
            {
                List<string> lines = new List<string>();

                string pad = new string(' ', 5);

                lines.Add("");
                lines.Add(string.Format("{0}", AssemblyHelper.GetTitle()));
                lines.Add("");
                lines.Add(pad + "Help Text");
                lines.Add(pad + "=========");
                lines.Add(pad + "?     : This Help Text");
                lines.Add(pad + "q     : Exit");
                lines.Add(pad + "x     : Exit");
                lines.Add(pad + "Up    : Scroll Up 1 line");
                lines.Add(pad + "Down  : Scroll Down 1 line");
                lines.Add(pad + "Left  : Scroll Left 5 columns");
                lines.Add(pad + "Right : Scroll Right 5 columns");

                return lines.ToArray();
            }
        }

        #endregion

        #region Fields

        private bool _PreviousCursorVisible = false;

        private int _PreviousCursorCol = 0;
        private int _PreviousCursorLine = 0;
        private List<string> _PreviousCursorOutput = new List<string>();

        List<string> _Lines = null;

        private int _CurrentLine = 0;
        private int _CurrentCol = 0;

        private string _StatusText;
        private MessageType _StatusType = MessageType.Info;

        private string _SearchText;
        private SearchResult _SearchResult;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        public string FileName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [cursor visible].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [cursor visible]; otherwise, <c>false</c>.
        /// </value>
        public bool CursorVisible
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [lock scroll].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [lock scroll]; otherwise, <c>false</c>.
        /// </value>
        public bool LockScroll
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [allow help].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allow help]; otherwise, <c>false</c>.
        /// </value>
        public bool AllowHelp
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the current line.
        /// </summary>
        public int CurrentLine
        {
            get { return _CurrentLine; }
            set { _CurrentLine = Math.Max(value, 0); }
        }

        /// <summary>
        /// Gets the current col.
        /// </summary>
        public int CurrentCol
        {
            get { return _CurrentCol; }
            set { _CurrentCol = Math.Max(value, 0); }
        }

        /// <summary>
        /// Gets the max line.
        /// </summary>
        public int MaxLine
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the max col.
        /// </summary>
        public int MaxCol
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the display cols.
        /// </summary>
        public int DisplayCols
        {
            get { return Console.WindowWidth; }
        }

        /// <summary>
        /// Gets the display lines.
        /// </summary>
        public int DisplayLines
        {
            get { return Console.WindowHeight - 2; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Viewer"/> class.
        /// </summary>
        public Viewer()
        {
            this.LockScroll = false;
            this.AllowHelp  = true;
        }

        #endregion

        #region Overrides

        #endregion

        #region Internal Methods

        /// <summary>
        /// Analyses the lines.
        /// </summary>
        private void AnalyseLines()
        {
            this.MaxLine = 0;
            this.MaxCol = 0;

            if (_Lines != null)
            {
                this.MaxLine = Math.Max(_Lines.Count, 0);
                this.MaxCol  = Math.Max(_Lines.Max(l => l.Length), 0);
            }
        }

        /// <summary>
        /// Setups the console.
        /// </summary>
        private void SetConsoleSettings()
        {
            _PreviousCursorVisible = Console.CursorVisible;

            Console.CursorVisible = this.CursorVisible;
        }

        /// <summary>
        /// Restores the console.
        /// </summary>
        private void RestoreConsoleSettings()
        {
            Console.CursorVisible = _PreviousCursorVisible;
        }

        /// <summary>
        /// Saves the console output.
        /// </summary>
        private void SaveConsoleOutput()
        {
            _PreviousCursorCol = Console.CursorLeft;
            _PreviousCursorLine = Console.CursorTop;
        }

        /// <summary>
        /// Restores the console output.
        /// </summary>
        private void RestoreConsoleOutput()
        {
            Console.CursorLeft = _PreviousCursorCol;
            Console.CursorTop = _PreviousCursorLine;
        }

        /// <summary>
        /// Determines whether [is any modifier present] [the specified key info].
        /// </summary>
        /// <param name="keyInfo">The key info.</param>
        /// <returns>
        ///   <c>true</c> if [is any modifier present] [the specified key info]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsAnyModifierPresent(ConsoleKeyInfo keyInfo)
        {
            return !(keyInfo.Modifiers == 0);
        }

        /// <summary>
        /// Determines whether [is shift present] [the specified key info].
        /// </summary>
        /// <param name="keyInfo">The key info.</param>
        /// <returns>
        ///   <c>true</c> if [is shift present] [the specified key info]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsShiftPresent(ConsoleKeyInfo keyInfo)
        {
            return (keyInfo.Modifiers & ConsoleModifiers.Shift) > 0;
        }

        /// <summary>
        /// Determines whether [is control present] [the specified key info].
        /// </summary>
        /// <param name="keyInfo">The key info.</param>
        /// <returns>
        ///   <c>true</c> if [is control present] [the specified key info]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsControlPresent(ConsoleKeyInfo keyInfo)
        {
            return (keyInfo.Modifiers & ConsoleModifiers.Control) > 0;
        }

        /// <summary>
        /// Determines whether [is alt present] [the specified key info].
        /// </summary>
        /// <param name="keyInfo">The key info.</param>
        /// <returns>
        ///   <c>true</c> if [is alt present] [the specified key info]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsAltPresent(ConsoleKeyInfo keyInfo)
        {
            return (keyInfo.Modifiers & ConsoleModifiers.Alt) > 0;
        }

        /// <summary>
        /// Draws the chrome.
        /// </summary>
        private void DrawChrome()
        {
            using (new ConsoleColorChanger(ConsoleColor.White, ColorType.Background))
            {
                using (new ConsoleColorChanger(ConsoleColor.DarkBlue, ColorType.Foreground))
                {
                    ConsoleHelper.DisplayAt(0, 0, new string(' ', Console.WindowWidth));
                    ConsoleHelper.DisplayAt(0, 0, Path.GetFileName(this.FileName));

                    DateTime modified = File.GetLastWriteTime(FileName);

                    string text = string.Format("{0} {1}", modified.ToShortDateString(), modified.ToLongTimeString());

                    ConsoleHelper.DisplayAt(0, Console.WindowWidth, text, DisplayAtAlignment.Right);



                    ConsoleHelper.DisplayAt(Console.WindowHeight - 1, 0, new string(' ', Console.WindowWidth - 1));

                    using (new ConsoleColorChanger(ConsoleColor.DarkRed, ColorType.Foreground))
                    {
                        ConsoleHelper.DisplayAt(Console.WindowHeight - 1, Console.WindowWidth - 1, AssemblyHelper.GetTitle(), DisplayAtAlignment.Right);
                    }
                }
            }

            DrawStats();
        }

        /// <summary>
        /// Draws the stats.
        /// </summary>
        private void DrawStats()
        {
            using (new ConsoleColorChanger(ConsoleColor.White, ColorType.Background))
            {
                using (new ConsoleColorChanger(ConsoleColor.DarkBlue, ColorType.Foreground))
                {
                    ConsoleHelper.DisplayAt(Console.WindowHeight - 1, 0, new string(' ', Console.WindowWidth - 1));

                    ConsoleHelper.DisplayAt(Console.WindowHeight - 1, 0, string.Format("Line: {0} / {2}   Col: {1} / {3}", CurrentLine+1, CurrentCol+1, MaxLine, MaxCol));

                    if (!string.IsNullOrEmpty(_StatusText))
                    {
                        ConsoleColor fg = ConsoleColor.Blue;
                        if (_StatusType == MessageType.Error)
                            fg = ConsoleColor.Red;

                        using (new ConsoleColorChanger(fg, ColorType.Foreground))
                        {
                            ConsoleHelper.DisplayAt(Console.WindowHeight - 1, Console.WindowWidth / 2, _StatusText, DisplayAtAlignment.Centre);
                        }
                    }

                    using (new ConsoleColorChanger(ConsoleColor.DarkRed, ColorType.Foreground))
                    {
                        ConsoleHelper.DisplayAt(Console.WindowHeight - 1, Console.WindowWidth - 1, AssemblyHelper.GetTitle(), DisplayAtAlignment.Right);
                    }
                }
            }
        }

        /// <summary>
        /// Draws the lines.
        /// </summary>
        private void DrawLines()
        {
            int index = CurrentLine;
            for (int i = 1; i <= DisplayLines; ++i)
            {
                string text = string.Empty;
                if (index < _Lines.Count)
                {
                    text = _Lines[index];

                    if (text.Length >= CurrentCol)
                        text = text.Substring(CurrentCol);
                    else
                        text = string.Empty;

                    if (text.Length > Console.WindowWidth)
                        text = text.Substring(0, Console.WindowWidth);
                }

                ++index;

                text = text.PadRight(Console.WindowWidth, ' ');

                ConsoleHelper.DisplayAt(i, 0, text);

                if (_SearchResult.Found)
                {
                    if (_SearchResult.Line + 1 == index)
                    {
                        // Get the current console colors
                        ConsoleColor fg = ConsoleColorChanger.GetColor(ColorType.Foreground);
                        ConsoleColor bg = ConsoleColorChanger.GetColor(ColorType.Background);

                        // Invert them to display found text
                        using (new ConsoleColorChanger(bg, ColorType.Foreground))
                        {
                            using (new ConsoleColorChanger(fg, ColorType.Background))
                            {
                                ConsoleHelper.DisplayAt(i, _SearchResult.Column, _SearchText);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads the text.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="lines">The lines.</param>
        /// <returns></returns>
        public bool LoadText(string title, string[] lines)
        {
            this.FileName = title;

            try
            {
                _Lines = new List<string>();
                _Lines.AddRange(lines);

                AnalyseLines();

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Loads the file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public bool LoadFile(string fileName)
        {
            this.FileName = fileName;

            try
            {
                string[] lines = File.ReadAllLines(fileName);

                _Lines = new List<string>();
                _Lines.AddRange(lines);

                AnalyseLines();

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Displays the specified file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public void Display()
        {
            SaveConsoleOutput();
            SetConsoleSettings();
            DrawChrome();

            DrawLines();

            ConsoleKeyInfo keyinfo;
            ConsoleKeyInfo nextkeyinfo = new ConsoleKeyInfo();
            do
            {
                if (nextkeyinfo.Key == 0)
                    keyinfo = Console.ReadKey(true);
                else
                    keyinfo = nextkeyinfo;
                nextkeyinfo = new ConsoleKeyInfo();

                ClearStatusMessage();

                switch (keyinfo.Key)
                {
                    case ConsoleKey.Help:
                    case ConsoleKey.F1:
                        {
                            if (this.AllowHelp)
                            {
                                using (Viewer helpViewer = new Viewer())
                                {
                                    helpViewer.LoadText("Help Text", HelpText);
                                    helpViewer.LockScroll = true;
                                    helpViewer.AllowHelp = false;
                                    helpViewer.Display();
                                }
                                DrawChrome();
                                DrawLines();
                                DrawStats();
                            }
                        }
                        break;
                    case ConsoleKey.UpArrow:
                        if (!LockScroll) CurrentLine = Math.Max(CurrentLine - 1, 0);
                        break;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.Spacebar:
                        if (!LockScroll) CurrentLine = Math.Min(CurrentLine + 1, MaxLine);
                        break;
                    case ConsoleKey.RightArrow:
                        if (!LockScroll) CurrentCol = Math.Min(CurrentCol + 5, MaxCol);
                        break;
                    case ConsoleKey.LeftArrow:
                        if (!LockScroll) CurrentCol = Math.Max(CurrentCol - 5, 0);
                        break;
                    case ConsoleKey.PageUp:
                        if (!LockScroll) CurrentLine = Math.Max(CurrentLine - Console.WindowHeight, 0);
                        break;
                    case ConsoleKey.PageDown:
                        if (!LockScroll) CurrentLine = Math.Min(CurrentLine + Console.WindowHeight, MaxLine);
                        break;
                    case ConsoleKey.Home:
                        if (!LockScroll)
                        {
                            if (!IsAnyModifierPresent(keyinfo))
                            {
                                CurrentCol = 0;
                            }
                            else if (IsControlPresent(keyinfo))
                            {
                                CurrentLine = 0;
                            }
                        }
                        break;
                    case ConsoleKey.End:
                        if (!LockScroll)
                        {
                            if (!IsAnyModifierPresent(keyinfo))
                            {
                                CurrentCol = MaxCol - DisplayCols + 1;
                            }
                            else if (IsControlPresent(keyinfo))
                            {
                                CurrentLine = MaxLine - DisplayLines + 1;
                            }
                        }
                        break;
                    case ConsoleKey.Q:
                    case ConsoleKey.X:
                        keyinfo = new ConsoleKeyInfo('q', ConsoleKey.Escape, false, false, false);
                        break;
                    case ConsoleKey.F:
                        _SearchText   = string.Empty;
                        _SearchResult = SearchResult.Empty;

                        string searchtext = GetInputText("Find");
                        if (!string.IsNullOrEmpty(searchtext))
                        {
                            _SearchText = searchtext;
                            nextkeyinfo = new ConsoleKeyInfo('f', ConsoleKey.F3, false, false, false);
                        }
                        break;
                    case ConsoleKey.Oem2:   // /?
                        if (IsShiftPresent(keyinfo))    // ?
                            nextkeyinfo = new ConsoleKeyInfo('?', ConsoleKey.F1, false, false, false);
                        else
                            nextkeyinfo = new ConsoleKeyInfo('f', ConsoleKey.F, false, false, false);
                        break;
                    case ConsoleKey.Oem5:   // \|
                        if (!IsShiftPresent(keyinfo))
                            nextkeyinfo = new ConsoleKeyInfo('\\', ConsoleKey.F, false, false, false);
                        break;
                    case ConsoleKey.F3:
                        if (string.IsNullOrEmpty(_SearchText))
                        {
                            nextkeyinfo = new ConsoleKeyInfo('f', ConsoleKey.F, false, false, false);
                        }
                        else
                        {
                            if (FindNextOccurrence(_SearchText))
                                CurrentLine = _SearchResult.Line;
                            else
                                SetStatusMessage(string.Format("* * {0} not found * *", _SearchText));
                        }
                        break;
                    case ConsoleKey.R:
                        Console.Clear();
                        DrawChrome();
                        break;
                    case ConsoleKey.F4:
                        SetStatusMessage("* * Really Not Found * *", MessageType.Error);
                        break;
                    case ConsoleKey.Enter:
                        if (_SearchResult.Found)
                        {
                            if (FindNextOccurrence(_SearchText))
                                CurrentLine = _SearchResult.Line;
                            else
                                SetStatusMessage(string.Format("* * {0} not found * *", _SearchText));
                        }
                        break;
                    case ConsoleKey.Escape:
                        if (!string.IsNullOrEmpty(_StatusText))
                            keyinfo = new ConsoleKeyInfo('q', ConsoleKey.NoName, false, false, false);
                        break;
                    default:
                        SetStatusMessage(keyinfo.Key.ToString());
                        break;
                }

                DrawLines();
                DrawStats();
            }
            while (keyinfo.Key != ConsoleKey.Escape);
        }

        /// <summary>
        /// Gets the input text.
        /// </summary>
        /// <param name="prompt">The prompt.</param>
        /// <returns></returns>
        private string GetInputText(string prompt)
        {
            return GetInputText(prompt, -1);
        }

        /// <summary>
        /// Gets the input text.
        /// </summary>
        /// <param name="prompt">The prompt.</param>
        /// <returns></returns>
        private string GetInputText(string prompt, int maxLength)
        {
            if (string.IsNullOrEmpty(prompt))
                return null;

            bool cursorVisible = Console.CursorVisible;
            int cursorSize = Console.CursorSize;

            Console.CursorSize = 3;
            Console.CursorVisible = true;

            string text = string.Empty;

            using (new ConsoleColorChanger(ConsoleColor.White, ColorType.Background))
            {
                using (new ConsoleColorChanger(ConsoleColor.DarkGreen, ColorType.Foreground))
                {
                    int inputLine = Console.WindowHeight - 1;
                    int startColumn = prompt.Length + 2;
                    int absMaxLength = Console.WindowWidth - (prompt.Length + 1) - 13;

                    if (maxLength < 1)
                        maxLength = absMaxLength;

                    ConsoleHelper.DisplayAt(inputLine, 0, string.Format("{0}: ", prompt));
                    ConsoleHelper.DisplayAt(inputLine, startColumn, new string(' ', absMaxLength));
                    ConsoleHelper.MoveTo(inputLine, startColumn);

                    ConsoleKeyInfo keyinfo;
                    do
                    {
                        keyinfo = Console.ReadKey(true);

                        switch (keyinfo.Key)
                        {
                            case ConsoleKey.Escape:
                                if (text.Length > 0)
                                {
                                    ConsoleHelper.DisplayAt(inputLine, startColumn, new string(' ', absMaxLength));
                                    ConsoleHelper.DisplayAt(inputLine, startColumn, string.Empty);
                                    text = string.Empty;
                                }
                                else
                                {
                                    text = null;
                                    keyinfo = new ConsoleKeyInfo('a', ConsoleKey.Enter, false, false, false);
                                }
                                break;
                            case ConsoleKey.A:
                            case ConsoleKey.B:
                            case ConsoleKey.C:
                            case ConsoleKey.D:
                            case ConsoleKey.E:
                            case ConsoleKey.F:
                            case ConsoleKey.G:
                            case ConsoleKey.H:
                            case ConsoleKey.I:
                            case ConsoleKey.J:
                            case ConsoleKey.K:
                            case ConsoleKey.L:
                            case ConsoleKey.M:
                            case ConsoleKey.N:
                            case ConsoleKey.O:
                            case ConsoleKey.P:
                            case ConsoleKey.Q:
                            case ConsoleKey.R:
                            case ConsoleKey.S:
                            case ConsoleKey.T:
                            case ConsoleKey.U:
                            case ConsoleKey.V:
                            case ConsoleKey.W:
                            case ConsoleKey.X:
                            case ConsoleKey.Y:
                            case ConsoleKey.Z:
                            case ConsoleKey.D0:
                            case ConsoleKey.D1:
                            case ConsoleKey.D2:
                            case ConsoleKey.D3:
                            case ConsoleKey.D4:
                            case ConsoleKey.D5:
                            case ConsoleKey.D6:
                            case ConsoleKey.D7:
                            case ConsoleKey.D8:
                            case ConsoleKey.D9:
                            case ConsoleKey.Decimal:
                            case ConsoleKey.Separator:
                                //Console.Write(keyinfo.KeyChar);
                                if (text.Length < maxLength)
                                {
                                    text += keyinfo.KeyChar;
                                    Console.Write(keyinfo.KeyChar);
                                }
                                break;

                            case ConsoleKey.Backspace:
                                if (text.Length > 0)
                                {
                                    text = text.Substring(0, text.Length - 1);
                                    Console.Write(keyinfo.KeyChar);
                                    ConsoleHelper.DisplayAt(inputLine, startColumn + text.Length, " ");
                                    ConsoleHelper.MoveTo(inputLine, startColumn + text.Length);
                                }
                                break;
                        }
                    }
                    while (keyinfo.Key != ConsoleKey.Enter);
                }
            }

            Console.CursorVisible = cursorVisible;
            Console.CursorSize = cursorSize;

            return text;
        }

        /// <summary>
        /// Finds the next occurrence.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        private bool FindNextOccurrence(string text)
        {
            return FindNextOccurrence(text, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Finds the next occurrence.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        private bool FindNextOccurrence(string text, StringComparison comparison)
        {
            int line = Math.Max(_SearchResult.Line, 0);
            int col = Math.Max(_SearchResult.Column, 0);

            if (_SearchResult.Found)
                ++col;

            while (line < _Lines.Count)
            {
                string textline = _Lines[line];

                if (!string.IsNullOrEmpty(textline))
                {
                    int pos = textline.IndexOf(text, col, comparison);
                    if (pos >= 0)
                    {
                        _SearchResult = new SearchResult(line, pos);
                        return true;
                    }
                }

                ++line;
                col = 0;
            }

            return false;
        }

        /// <summary>
        /// Clears the status message.
        /// </summary>
        private void ClearStatusMessage()
        {
            SetStatusMessage(string.Empty);
        }

        /// <summary>
        /// Displays the status message.
        /// </summary>
        /// <param name="text">The text.</param>
        private void SetStatusMessage(string text)
        {
            SetStatusMessage(text, MessageType.Info);
        }

        /// <summary>
        /// Displays the message.
        /// </summary>
        /// <param name="text">The text.</param>
        private void SetStatusMessage(string text, MessageType type)
        {
            _StatusText = text;
            _StatusType = type;
        }

        #endregion

        #region Static Methods

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            RestoreConsoleOutput();
            RestoreConsoleSettings();
        }

        #endregion
    }
}
