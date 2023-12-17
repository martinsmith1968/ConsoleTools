using System.Windows.Forms;

namespace ClipCmd
{
    internal class Program
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
        public static void DisplayClipboard()
        {
            Console.Out.WriteLine(
                Clipboard.GetText()
                );
        }
    }
}
