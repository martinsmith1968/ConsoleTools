// <copyright file="Timer.cs" company="Thales e-Security Ltd">
// Copyright (c) 2010 Thales e-Security Ltd
// All rights reserved. Company confidential.
// </copyright>
//
// TODO: [Description of Timer]

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace TimerCmd
{
    /// <summary>
    ///
    /// </summary>
    public enum TimerState
    {
        Running,

        Paused
    }

    /// <summary>
    /// TODO: [Description of Timer]
    /// </summary>
    [Serializable]
    public class Timer
    {
        #region Fields

        /// <summary>
        ///
        /// </summary>
        protected DateTime _InitialStartTime = DateTime.MinValue;

        /// <summary>
        ///
        /// </summary>
        protected TimeSpan _AccumulatedElapsedTime = TimeSpan.FromTicks(0);

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the status.
        /// </summary>
        public TimerState Status
        {
            get
            {
                if (this.StartTime > DateTime.MinValue)
                    return TimerState.Running;

                return TimerState.Paused;
            }
        }

        /// <summary>
        /// Gets the initial start time.
        /// </summary>
        public DateTime InitialStartTime
        {
            get { return _InitialStartTime; }
        }

        /// <summary>
        /// Gets the start time.
        /// </summary>
        public DateTime StartTime
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the total elapsed time.
        /// </summary>
        public TimeSpan TotalElapsedTime
        {
            get
            {
                TimeSpan elapsed = this.ElapsedTime;

                if (this._AccumulatedElapsedTime > TimeSpan.FromTicks(0))
                    elapsed += this._AccumulatedElapsedTime;

                return elapsed;
            }
        }

        /// <summary>
        /// Gets the elapsed time.
        /// </summary>
        public TimeSpan ElapsedTime
        {
            get
            {
                TimeSpan elapsed = TimeSpan.FromTicks(0);

                Trace.WriteLine("ElapsedTime: StartTime: " + this.StartTime.ToString(), "DEBUG");

                if (this.StartTime > DateTime.MinValue)
                    elapsed += DateTime.Now - this.StartTime;

                return elapsed;
            }
        }

        /// <summary>
        /// Gets the elapsed time text.
        /// </summary>
        public string ElapsedTimeText
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                if (ElapsedTime == TimeSpan.FromTicks(0))
                {
                    sb.Append("None");
                }
                else
                {
                    if (ElapsedTime.Days > 0)
                    {
                        if (sb.Length > 0)
                            sb.Append(", ");
                        sb.AppendFormat("{0} days", ElapsedTime.Days);
                    }
                    if (ElapsedTime.Hours > 0)
                    {
                        if (sb.Length > 0)
                            sb.Append(", ");
                        sb.AppendFormat("{0} hours", ElapsedTime.Hours);
                    }
                    if (ElapsedTime.Minutes > 0)
                    {
                        if (sb.Length > 0)
                            sb.Append(", ");
                        sb.AppendFormat("{0} minutes", ElapsedTime.Minutes);
                    }
                    if (ElapsedTime.Seconds > 0)
                    {
                        if (sb.Length > 0)
                            sb.Append(", ");
                        sb.AppendFormat("{0} seconds", ElapsedTime.Seconds);
                    }
                    if (sb.Length == 0)
                    {
                        sb.AppendFormat("{0} ms", ElapsedTime.Milliseconds);
                    }
                }

                return sb.ToString();
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Timer"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public Timer(string name)
            : this(name, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Timer"/> class.
        /// </summary>
        public Timer(string name, bool autoStart)
        {
            this.Name      = name;
            this.StartTime = DateTime.MinValue;

            if (autoStart)
                this.Start();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines whether the specified name is named.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        ///   <c>true</c> if the specified name is named; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNamed(string name)
        {
            return string.Compare(this.Name, name, true) == 0;
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            this.StartTime = DateTime.Now;

            if (_InitialStartTime == DateTime.MinValue)
                _InitialStartTime = this.StartTime;
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            Pause();
            Delete();
        }

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        public void Delete()
        {
            List<Timer> timers = new List<Timer>();

            timers.AddRange(GetAll());

            for (int i = 0; i < timers.Count; ++i)
            {
                if (timers[i].IsNamed(this.Name))
                {
                    timers.RemoveAt(i);
                    break;
                }
            }

            Settings.Default.TimerList.Clear();
            foreach(Timer t in timers)
                Settings.Default.TimerList.Add(t.ToText());
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            Stop();
            this._AccumulatedElapsedTime = TimeSpan.FromTicks(0);
        }

        /// <summary>
        /// Pauses this instance.
        /// </summary>
        public void Pause()
        {
            this._AccumulatedElapsedTime += this.ElapsedTime;
            this.StartTime               = DateTime.MinValue;
        }

        /// <summary>
        /// Toes the text.
        /// </summary>
        /// <returns></returns>
        public string ToText()
        {
            return string.Format("{0}:{1}:{2}:{3}:{4}",
                this.Name,
                this.Status.ToString(),
                this.InitialStartTime.Ticks,
                this.StartTime.Ticks,
                this._AccumulatedElapsedTime.Ticks
                );
        }

        /// <summary>
        /// Froms the text.
        /// </summary>
        public bool FromText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return false;

            string[] bits = text.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            string name = bits.Length > 0 ? bits[0]: string.Empty;
            if (string.IsNullOrEmpty(name))
                return false;

            string status = bits.Length > 1 ? bits[1] : string.Empty;

            long initialStartTime       = DateTime.MinValue.Ticks;
            long startTime              = DateTime.MinValue.Ticks;
            long accumulatedElapsedTime = 0;

            if (bits.Length > 2)
            {
                if (!long.TryParse(bits[2], out initialStartTime))
                    return false;
            }
            if (bits.Length > 3)
            {
                if (!long.TryParse(bits[3], out startTime))
                    return false;
            }
            if (bits.Length > 4)
            {
                if (!long.TryParse(bits[4], out accumulatedElapsedTime))
                    return false;
            }

            this.Name                    = name;
            this._InitialStartTime       = new DateTime(initialStartTime);
            this.StartTime               = new DateTime(startTime);
            this._AccumulatedElapsedTime = new TimeSpan(accumulatedElapsedTime);

            return true;
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Existses the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        static public bool Exists(string name)
        {
            return Get(name) != null;
        }

        /// <summary>
        /// Gets the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        static public Timer Get(string name)
        {
            foreach (Timer timer in GetAll())
            {
                if (timer.IsNamed(name))
                    return timer;
            }

            return null;
        }

        /// <summary>
        /// Parses the specified definition.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <returns></returns>
        static public Timer Parse(string definition)
        {
            Timer timer = null;

            try
            {
                Trace.WriteLine(string.Format("Definition: {0}", definition), "DEBUG");
                timer = new Timer("new");
                if (!timer.FromText(definition))
                    timer = null;
            }
            catch
            {
                timer = null;
            }

            return timer;
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns></returns>
        static public Timer[] GetAll()
        {
            List<Timer> timers = new List<Timer>();

            if (Settings.Default.TimerList != null)
            {
                foreach (string def in Settings.Default.TimerList)
                {
                    Timer timer = Timer.Parse(def);
                    if (timer != null)
                        timers.Add(timer);
                }
            }

            return timers.ToArray();
        }

        /// <summary>
        /// Counts this instance.
        /// </summary>
        /// <returns></returns>
        static public int Count
        {
            get
            {
                Timer[] timers = GetAll();

                return (timers == null) ? 0 : timers.Length;
            }
        }

        #endregion
    }
}
