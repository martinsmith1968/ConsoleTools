using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NRA.Util
{
    /// <summary>
    ///
    /// </summary>
    public enum DisplayAtAlignment
    {
        Left,
        Right,
        Centre
    }

    /// <summary>
    ///
    /// </summary>
    public abstract class ConsoleHelper
    {
        /// <summary>
        ///
        /// </summary>
        static public void DisplayHeader()
        {
            DisplayHeader(Console.Out);
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="writer"></param>
        static public void DisplayHeader(TextWriter writer)
        {
            string s;

            writer.Write("{0} v{1}", AssemblyHelper.GetTitle(), AssemblyHelper.FormatVersionText(AssemblyHelper.GetVersionText()));

            s = AssemblyHelper.GetDescription();
            if (!string.IsNullOrEmpty(s))
                writer.Write(" - {0}", s);
            writer.WriteLine();

            s = AssemblyHelper.GetCopyright();
            if (!string.IsNullOrEmpty(s))
                writer.WriteLine(s);
        }

        /// <summary>
        ///
        /// </summary>
        static public void Display()
        {
            Display(null);
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="s"></param>
        static public void Display(string s)
        {
            Display(s, true);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="s"></param>
        /// <param name="newline"></param>
        static public void Display(string s, bool newline)
        {
            Display(s, Console.Out, newline);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="s"></param>
        /// <param name="wtr"></param>
        static public void Display(string s, TextWriter wtr)
        {
            Display(s, wtr, true);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="s"></param>
        /// <param name="wtr"></param>
        static public void Display(string s, TextWriter wtr, bool newline)
        {
            if (newline)
                wtr.WriteLine(s);
            else
                wtr.Write(s);
        }

        /// <summary>
        /// Displays at.
        /// </summary>
        /// <param name="y">The y.</param>
        /// <param name="x">The x.</param>
        /// <param name="text">The text.</param>
        static public void DisplayAt(int y, int x, string text)
        {
            DisplayAt(y, x, text, DisplayAtAlignment.Left);
        }

        /// <summary>
        /// Displays at.
        /// </summary>
        /// <param name="y">The y.</param>
        /// <param name="x">The x.</param>
        /// <param name="text">The text.</param>
        static public void DisplayAt(int y, int x, string text, DisplayAtAlignment alignment)
        {
            if (!string.IsNullOrEmpty(text))
            {
                switch (alignment)
                {
                    case DisplayAtAlignment.Right:
                        x -= text.Length;
                        break;

                    case DisplayAtAlignment.Centre:
                        x -= (text.Length / 2);
                        break;
                }
            }

            MoveTo(y, x);
            Console.Out.Write(text);
        }

        /// <summary>
        /// Gets or sets the current X.
        /// </summary>
        /// <value>
        /// The current X.
        /// </value>
        static public int CurrentX
        {
            get { return Console.CursorLeft; }
            set { Console.CursorLeft = value; }
        }

        /// <summary>
        /// Gets or sets the current Y.
        /// </summary>
        /// <value>
        /// The current Y.
        /// </value>
        static public int CurrentY
        {
            get { return Console.CursorTop; }
            set { Console.CursorTop = value; }
        }

        /// <summary>
        /// Moves to.
        /// </summary>
        /// <param name="y">The y.</param>
        /// <param name="x">The x.</param>
        static public void MoveTo(int y, int x)
        {
            Console.CursorLeft = x;
            Console.CursorTop = y;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="s"></param>
        static public void DisplayError(string s)
        {
            DisplayError(s, true);
        }

        /// <summary>
        /// Displays the error.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="changeColour">if set to <c>true</c> [change colour].</param>
        static public void DisplayError(string s, bool changeColour)
        {
            if (changeColour)
            {
                using (new ConsoleColorChanger(ConsoleColor.Red, ColorType.Foreground))
                {
                    Display(s, Console.Error);
                }
            }
            else
            {
                Display(s, Console.Error);
            }
        }
    }

    #region Console Color Changer

    /// <summary>
    ///
    /// </summary>
    public enum ColorType
    {
        Foreground,
        Background
    }

    /// <summary>
    ///
    /// </summary>
    public class ConsoleColorChanger : IDisposable
    {
        private ConsoleColor oldColor;
        private ColorType oldColorType;

        public ConsoleColorChanger(ConsoleColor newColor)
            : this(newColor, ColorType.Foreground)
        {
        }

        public ConsoleColorChanger(ConsoleColor newColor, ColorType type)
        {
            oldColorType = type;
            oldColor = GetColor(oldColorType);

            SetColor(newColor, oldColorType);
        }

        #region Static Methods

        static public ConsoleColor GetColor()
        {
            return GetColor(ColorType.Foreground);
        }
        static public ConsoleColor GetColor(ColorType type)
        {
            switch (type)
            {
                case ColorType.Foreground:
                    return Console.ForegroundColor;
                case ColorType.Background:
                    return Console.BackgroundColor;
                default:
                    return Console.ForegroundColor;
            }
        }

        static public void SetColor(ConsoleColor color)
        {
            SetColor(color, ColorType.Foreground);
        }
        static public void SetColor(ConsoleColor color, ColorType type)
        {
            switch (type)
            {
                case ColorType.Foreground:
                    Console.ForegroundColor = color;
                    break;

                case ColorType.Background:
                    Console.BackgroundColor = color;
                    break;
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            SetColor(oldColor, oldColorType);
        }

        #endregion
    }

    #endregion
}
