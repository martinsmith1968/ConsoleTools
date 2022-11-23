using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Windows.Forms;

namespace ClipCmd
{
    class Program
    {
        /// <summary>
        /// Program entry point
        /// </summary>
        /// <param name="args">The args.</param>
        static void Main(string[] args)
        {
            Thread clipboardThread = new Thread(DisplayClipboard);
            clipboardThread.SetApartmentState(ApartmentState.STA);
            clipboardThread.IsBackground = false;
            clipboardThread.Start();
            clipboardThread.Join();
        }

        /// <summary>
        /// Displays the clipboard.
        /// </summary>
        static public void DisplayClipboard()
        {
            Console.Out.WriteLine(
                Clipboard.GetText()
                );
        }
    }
}
