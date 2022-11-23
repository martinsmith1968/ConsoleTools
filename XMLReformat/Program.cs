using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using NRA.Util;
using NRA.Util.CommandLine;

// TODO: Implement all reformat modes

namespace XMLReformat
{
    class Program
    {
        static Arguments Arguments = new Arguments();

        static void Main(string[] args)
        {
            // Start display
            ConsoleHelper.DisplayHeader();
            ConsoleHelper.Display();

            // Process Command Line
            bool argumentsOK = Parser.ParseArguments(args, Arguments);
            if (!argumentsOK || Arguments.Help)
            {
                // Usage
                ConsoleHelper.Display(Parser.ArgumentsUsage(typeof(Arguments), Console.WindowWidth));
                return;
            }

            // Check FileName
            if (!File.Exists(Arguments.FileName))
            {
                ConsoleHelper.Display(string.Format("File does not exist! ({0})", Arguments.FileName));
                return;
            }

            // Might need this
            XmlDocument doc = null;
            XmlTextWriter writer = null;

            // Process according to Mode
            ConsoleHelper.Display(string.Format("Using Mode: {0}", Arguments.FormatType.ToString()));
            switch (Arguments.FormatType)
            {
                case FormatType.Compressed:
#if FORMAT_COMPRESS_SUPPORTED
                    // Read
                    string contents = System.IO.File.ReadAllText(Arguments.FileName);

                    // Replace
                    contents = contents.Replace("<", "\r\n<");
                    contents = contents.Replace("\r\n</", "</");
                    while (contents.StartsWith("\r\n"))
                        contents = contents.Substring(2);

                    // Write
                    System.Console.Out.Write(contents);
                    break;
#else
                    ConsoleHelper.DisplayError("Format not yet supported");
                    return;
#endif

                case FormatType.Indented:
#if FORMAT_INDENTED_SUPPORTED
                    break;
#else
                    ConsoleHelper.DisplayError("Format not yet supported");
                    return;
#endif

                case FormatType.XmlDocumentCompressed:
                    doc = new XmlDocument();

                    doc.Load(Arguments.FileName);

                    if (Arguments.HasOutputFileName)
                        writer = new XmlTextWriter(Arguments.OutputFileName, Encoding.Default);
                    else
                        writer = new XmlTextWriter(Console.Out);

                    using (writer)
                    {
                        writer.Formatting = Formatting.None;

                        doc.WriteContentTo(writer);
                    }

                    break;

                case FormatType.XmlDocumentIndented:
                    doc = new XmlDocument();

                    doc.Load(Arguments.FileName);

                    if (Arguments.HasOutputFileName)
                        writer = new XmlTextWriter(Arguments.OutputFileName, Arguments.GetEncoding());
                    else
                        writer = new XmlTextWriter(Console.Out);

                    using (writer)
                    {
                        writer.Formatting = Formatting.Indented;

                        doc.WriteContentTo(writer);
                    }

                    break;
            }
        }
    }
}
