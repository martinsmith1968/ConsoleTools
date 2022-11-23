using System;
using System.Collections.Generic;
using TimerCmd;
using NUnit.Framework;
using System.Threading;

namespace TimerCmdTest
{
    [TestFixture]
    public class TimerCmdTest
    {
        /// <summary>
        /// Test1s this instance.
        /// </summary>
        [Test]
        public void Test1()
        {
            TimerCmd.Timer timer = new TimerCmd.Timer("Shite", true);

            TimeSpan ts = TimeSpan.FromSeconds(3);

            Thread.Sleep(ts);

            Assert.GreaterOrEqual(timer.TotalElapsedTime, ts);
        }
    }
}
