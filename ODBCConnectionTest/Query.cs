// <copyright file="Query.cs" company="Thales e-Security Ltd">
// Copyright (c) 2010 Thales e-Security Ltd
// All rights reserved. Company confidential.
// </copyright>
//
// TODO: [Description of Query]

using System.Data;
using System.Data.Odbc;



namespace ODBCConnectionTest
{
    /// <summary>
    /// TODO: [Description of Query]
    /// </summary>
    public class Query
    {
        #region Constants

        private const int MAX_SLEEP_SECONDS = 10;

        #endregion

        #region Static Methods

        #endregion

        #region Fields

        static Random randomizer = new Random();

        private object locker_quit = new object();

        private bool quit = false;

        private Thread thread = null;

        #endregion

        #region Properties

        public OdbcConnection Connection
        {
            get;
            private set;
        }

        public string ConnectionString
        {
            get;
            private set;
        }

        public string SQL
        {
            get;
            private set;
        }

        public FileWriter Writer
        {
            get;
            private set;
        }

        public int ThreadId
        {
            get { return this.thread == null ? 0 : this.thread.ManagedThreadId; }
        }

        public ThreadState ThreadState
        {
            get { return this.thread.ThreadState; }
        }

        public bool IsStopping
        {
            get
            {
                lock (this.locker_quit)
                {
                    return this.quit;
                }
            }
        }

        public bool IsStopped
        {
            get { return this.ThreadState == ThreadState.Stopped; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Query"/> class.
        /// </summary>
        public Query(string connectionString, string sql, FileWriter writer)
        {
            this.ConnectionString = connectionString;
            this.SQL              = sql;
            this.Writer           = writer;
        }

        #endregion

        #region Overrides

        #endregion

        #region Internal Methods

        /// <summary>
        /// Writes the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        private void Display(string text)
        {
            this.Writer.WriteLine(string.Format("{0}: {1}{2}",
                Thread.CurrentThread.ManagedThreadId,
                text,
                Environment.NewLine
                )
            );
        }

        /// <summary>
        /// Writes the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg">The arg.</param>
        private void Display(string format, params object[] arg)
        {
            this.Display(string.Format(format, arg));
        }

        /// <summary>
        /// Displays the specified ex.
        /// </summary>
        /// <param name="ex">The ex.</param>
        private void Display(Exception ex)
        {
            Display("Exception: {0}", ex.Message);
            Display(ex.StackTrace);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Goes this instance.
        /// </summary>
        public void Go()
        {
            lock (this.locker_quit)
            {
                this.quit = false;
            }

            this.thread = new Thread(new ThreadStart(this.Execute));
            this.thread.Start();
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            lock (this.locker_quit)
            {
                Display("Quiting...");
                this.quit = true;
            }
        }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        public void Execute()
        {
            while (!this.IsStopping)
            {
                if (this.Connection == null)
                {
                    Display("Connecting...");

                    try
                    {
                        this.Connection = new OdbcConnection(this.ConnectionString);
                        this.Connection.Open();
                    }
                    catch (Exception ex)
                    {
                        Display(ex);
                        return;
                    }
                }

                //TimeSpan sleepTime = TimeSpan.FromSeconds(DateTime.Now.Millisecond % 10);
                TimeSpan sleepTime = TimeSpan.FromSeconds(randomizer.Next(MAX_SLEEP_SECONDS));
                Display("Sleeping: {0}", sleepTime.TotalSeconds);
                Thread.Sleep(sleepTime);

                Display("Preparing...");
                using (IDbCommand cmd = this.Connection.CreateCommand())
                {
                    cmd.CommandText = this.SQL;
                    cmd.CommandType = CommandType.Text;

                    object result = null;

                    try
                    {
                        Display("Executing...");
                        result = cmd.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                        Display(ex);
                        return;
                    }

                    this.Display("SQL: {0}, Result: {1}",
                        this.SQL,
                        result
                    );
                }
            }

            Display("Stopping...");
        }

        #endregion
    }
}
