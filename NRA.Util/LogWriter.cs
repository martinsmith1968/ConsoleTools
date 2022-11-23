// <copyright file="LogWriter.cs" company="Thales e-Security Ltd">
// Copyright (c) 2010 Thales e-Security Ltd
// All rights reserved. Company confidential.
// </copyright>
//
// LogWriting class and supporting listeners

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NRA.Util
{
    #region Supporting Enumerations

    /// <summary>
    /// The Severity of the Log Message
    /// </summary>
    public enum Severity
    {
        /// <summary>
        /// Debugging information
        /// </summary>
        Debug,

        /// <summary>
        /// For Information only
        /// </summary>
        Information,

        /// <summary>
        /// A Warning
        /// </summary>
        Warning,

        /// <summary>
        /// An Error
        /// </summary>
        Error,

        /// <summary>
        /// A Critical message
        /// </summary>
        Critical,

        /// <summary>
        /// An Exception
        /// </summary>
        Exception
    }

    #endregion

    #region ILogListener Interface

    /// <summary>
    ///
    /// </summary>
    public interface ILogListener
    {
        /// <summary>
        /// Called when the log is started
        /// </summary>
        void OnStart();

        /// <summary>
        /// Called when the log is finished
        /// </summary>
        void OnFinish();

        /// <summary>
        /// Called when a log message is received.
        /// </summary>
        /// <param name="timestamp">The timestamp.</param>
        /// <param name="severity">The severity.</param>
        /// <param name="text">The text of the message.</param>
        void OnMessage(DateTime timestamp, Severity severity, string text);
    }

    #endregion

    #region LogWriter controller class

    /// <summary>
    /// The LogWriter static class
    /// </summary>
    public class LogWriter
    {
        #region Constants

        /// <summary>
        /// Time
        /// </summary>
        public const string FORMAT_TIME = "hh:mm.ss";

        /// <summary>
        /// Time including Milliseconds
        /// </summary>
        public const string FORMAT_TIME_WITH_MILLISECOND = "hh:mm.ss.fff";

        #endregion

        #region Singleton Pattern

        /// <summary>
        /// Initializes a new instance of the <see cref="Cache"/> class.
        /// </summary>
        private LogWriter()
        {
            this.Initialise();
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static LogWriter Instance
        {
            get
            {
                return Nested.Instance;
            }
        }

        /// <summary>
        /// Nested container class
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1409:RemoveUnnecessaryCode", Justification = "Required for generating correct IL code")]
        private class Nested
        {
            /// <summary>
            /// Initializes the <see cref="Nested"/> class.
            /// </summary>
            /// <remarks>
            /// Explicit static constructor to tell C# compiler
            /// not to mark type as beforefieldinit
            /// NOTE: Ignore erroneous StyleCop Warning
            /// </remarks>
            static Nested()
            {
            }

            /// <summary>
            /// The instance. Created statically.
            /// </summary>
            internal static readonly LogWriter Instance = new LogWriter();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The internal collection of log listeners
        /// </summary>
        private List<ILogListener> listeners = new List<ILogListener>();

        #endregion

        #region Constructors

        /// <summary>
        /// Initialises this instance.
        /// </summary>
        private void Initialise()
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds the listener.
        /// </summary>
        /// <param name="listener">The listener.</param>
        public void AddListener(ILogListener listener)
        {
            if (!this.listeners.Contains(listener))
            {
                this.listeners.Add(listener);
            }
        }

        /// <summary>
        /// Removes the listener.
        /// </summary>
        /// <param name="listener">The listener.</param>
        public void RemoveListener(ILogListener listener)
        {
            if (this.listeners.Contains(listener))
            {
                this.listeners.Remove(listener);
            }
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="text">The text.</param>
        public void WriteLine(string text)
        {
            this.WriteLine(Severity.Information, text);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="severity">The severity.</param>
        /// <param name="text">The text to write.</param>
        public void WriteLine(Severity severity, string text)
        {
            DateTime now = DateTime.Now;

            foreach (var listener in this.listeners)
            {
                listener.OnMessage(now, severity, text);
            }
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void WriteLine(string format, params object[] args)
        {
            this.WriteLine(Severity.Information, format, args);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="severity">The severity.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void WriteLine(Severity severity, string format, params object[] args)
        {
            this.WriteLine(severity, string.Format(format, args));
        }

        #endregion
    }

    #endregion

    #region FileLogListener class

    /// <summary>
    /// A LogListener to write to a File.
    /// </summary>
    public class FileLogListener : ILogListener
    {
        #region Constants

        /// <summary>
        /// The Default message format
        /// </summary>
        public const string MESSAGEFORMAT_DEFAULT = "{0:" + LogWriter.FORMAT_TIME + "}: {1,-11} {2}";

        /// <summary>
        /// The Default message format, including milliseconds on the timestamp
        /// </summary>
        public const string MESSAGEFORMAT_DEFAULT_WITH_MILLISECOND = "{0:" + LogWriter.FORMAT_TIME_WITH_MILLISECOND + "}: {1,-11} {2}";

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        public string FileName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the encoding.
        /// </summary>
        public Encoding Encoding
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the message format.
        /// </summary>
        /// <value>
        /// The message format.
        /// </value>
        public string MessageFormat
        {
            get;
            set;
        }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="FileLogListener"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public FileLogListener(string fileName)
            : this(fileName, Encoding.ASCII)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileLogListener"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="encoding">The encoding.</param>
        public FileLogListener(string fileName, Encoding encoding)
            : this(fileName, encoding, MESSAGEFORMAT_DEFAULT_WITH_MILLISECOND)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileLogListener"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="messageFormat">The message format.</param>
        public FileLogListener(string fileName, Encoding encoding, string messageFormat)
        {
            this.FileName      = fileName;
            this.Encoding      = encoding;
            this.MessageFormat = messageFormat;
        }

        #endregion

        #region ILogListener Members

        /// <summary>
        /// Called when the log is started
        /// </summary>
        public void OnStart()
        {
            File.WriteAllText(this.FileName, string.Empty, this.Encoding);
        }

        /// <summary>
        /// Called when the log is finished
        /// </summary>
        public void OnFinish()
        {
        }

        /// <summary>
        /// Called when a log message is received.
        /// </summary>
        /// <param name="timestamp">The timestamp.</param>
        /// <param name="severity">The severity.</param>
        /// <param name="text">The text of the message.</param>
        public void OnMessage(DateTime timestamp, Severity severity, string text)
        {
            string message = string.Format(this.MessageFormat,
                timestamp,
                severity,
                text);

            File.AppendAllText(this.FileName, message, this.Encoding);
        }

        #endregion
    }

    #endregion

    #region CustomLogListener

    /// <summary>
    /// Event Args for a Custom Log Event
    /// </summary>
    public class CustomLogEventArgs
    {
        /// <summary>
        /// Gets the empty.
        /// </summary>
        static public CustomLogEventArgs Empty
        {
            get
            {
                return new CustomLogEventArgs(DateTime.MinValue, Severity.Debug, string.Empty);
            }
        }

        /// <summary>
        /// Gets the date time.
        /// </summary>
        public DateTime Timestamp
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the severity.
        /// </summary>
        public Severity Severity
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        public string Text
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomLogEventArgs"/> class.
        /// </summary>
        /// <param name="timestamp">The timestamp.</param>
        /// <param name="severity">The severity.</param>
        /// <param name="text">The text.</param>
        public CustomLogEventArgs(DateTime timestamp, Severity severity, string text)
        {
            this.Timestamp = timestamp;
            this.Severity  = severity;
            this.Text      = text;
        }
    }

    /// <summary>
    /// A LogListener that raises .NET events
    /// </summary>
    public class CustomLogListener : ILogListener
    {
        #region Events and Delegates

        public delegate void LogStartDelegate();
        public delegate void LogFinishDelegate();
        public delegate void LogMessageDelegate(CustomLogEventArgs args);

        public event LogStartDelegate OnLogStart;
        public event LogFinishDelegate OnLogFinish;
        public event LogMessageDelegate OnLogMessage;

        #endregion

        #region ILogListener Members

        /// <summary>
        /// Called when the log is started
        /// </summary>
        public void OnStart()
        {
            if (this.OnLogStart != null)
            {
                this.OnLogStart();
            }
        }

        /// <summary>
        /// Called when the log is finished
        /// </summary>
        public void OnFinish()
        {
            if (this.OnLogFinish != null)
            {
                this.OnLogFinish();
            }
        }

        /// <summary>
        /// Called when a log message is received.
        /// </summary>
        /// <param name="timestamp">The timestamp.</param>
        /// <param name="severity">The severity.</param>
        /// <param name="text">The text of the message.</param>
        public void OnMessage(DateTime timestamp, Severity severity, string text)
        {
            if (this.OnLogMessage != null)
            {
                CustomLogEventArgs args = new CustomLogEventArgs(timestamp, severity, text);

                this.OnLogMessage(args);
            }
        }

        #endregion
    }

    #endregion
}
