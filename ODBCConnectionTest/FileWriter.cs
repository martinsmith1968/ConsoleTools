// <copyright file="FileWriter.cs" company="Thales e-Security Ltd">
// Copyright (c) 2010 Thales e-Security Ltd
// All rights reserved. Company confidential.
// </copyright>
//
// TODO: [Description of FileWriter]

using System;
using System.Collections.Generic;
using System.IO;

namespace ODBCConnectionTest
{
    /// <summary>
    /// TODO: [Description of FileWriter]
    /// </summary>
    public class FileWriter
    {
        #region Constants

        #endregion

        #region Static Methods

        #endregion

        #region Fields

        private object locker = new object();

        #endregion

        #region Properties

        public string FileName
        {
            get;
            private set;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FileWriter"/> class.
        /// </summary>
        public FileWriter(string fileName)
        {
            this.FileName = fileName;
        }

        #endregion

        #region Overrides

        #endregion

        #region Internal Methods

        private void AppendText(string text)
        {
            lock (this.locker)
            {
                File.AppendAllText(this.FileName, text);
            }
        }

        #endregion

        #region Public Methods

        #endregion

        public void WriteLine(string format, params object[] arg)
        {
            AppendText(string.Format(format, arg));
        }

        public void WriteLine(string value)
        {
            AppendText(value);
        }

        public void WriteLine(object value)
        {
            AppendText(Convert.ToString(value));
        }
    }
}
