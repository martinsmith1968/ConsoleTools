using NUnit.Framework;
using NUnit.Framework.Legacy;

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
            var timer = new TimerCmd.Timer("Shite", true);

            var ts = TimeSpan.FromSeconds(3);

            Thread.Sleep(ts);

            ClassicAssert.GreaterOrEqual(timer.TotalElapsedTime, ts);
        }
    }
}
