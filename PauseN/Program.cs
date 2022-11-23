using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PauseN
{
    class Program
    {
        /// <summary>
        /// Mains the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        static void Main(string[] args)
        {
			int waitSeconds = 0;
            string waitMessage = "Press any key to continue . . .";

			if (args.Length > 0)
			{
				if (!Int32.TryParse(args[0], out waitSeconds) || waitSeconds < 0)
					waitSeconds = 0;
			}

            if (waitSeconds > 0)
			    waitMessage = "Press any key to continue (or wait {0} seconds) . . .";

			Console.Out.Write(string.Format(waitMessage, waitSeconds));

            DateTime timeout = DateTime.MaxValue;
            if (waitSeconds > 0)
                timeout = DateTime.Now.AddSeconds(waitSeconds);

            while (DateTime.Now < timeout)
            {
                if (Console.KeyAvailable)
                {
                    Console.ReadKey(true);
                    break;
                }
            }
        }
    }
}
