using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NRA.Util.CommandLine;

namespace XMLReformat
{
    /// <summary>
    /// 
    /// </summary>
    public enum FormatType
    {
        Indented,
        Compressed,
        XmlDocumentIndented,
        XmlDocumentCompressed
    }

    public enum EncodingType
    {
        Default,
        ASCII,
        BigEndianUnicode,
        Unicode,
        UTF7,
        UTF8,
        UTF32
    }

    /// <summary>
    /// 
    /// </summary>
    public class Arguments
    {
        #region Main Parameters

        [DefaultArgument(ArgumentType.Required, GroupName = "Main", HelpText = "The XML File to Reformat")]
        public string FileName;

        [Argument(ArgumentType.AtMostOnce, DefaultValue = FormatType.XmlDocumentIndented, GroupName = "Main", ShortName = "t", HelpText = "The method of formatting to use")]
        public FormatType FormatType;

        #endregion

        #region Output

        [Argument(ArgumentType.AtMostOnce, GroupName = "Output", ShortName = "o", HelpText = "The output file name (Default: StdOut)")]
        public string OutputFileName;

        [Argument(ArgumentType.AtMostOnce, DefaultValue = EncodingType.Default, GroupName = "Output", ShortName = "e", HelpText = "The encoding to use if specified OutputFileName")]
        public EncodingType OutputFileEncodingType;

        #endregion

        #region Optional

        #endregion

        #region Standalone

        [Argument(ArgumentType.BypassMandatory, DefaultValue = false, ShortName = "?", GroupName = "Standalone", HelpText = "Show the Help page")]
        public bool Help;

        #endregion

        #region Properties

        public bool HasFileName
        {
            get { return !string.IsNullOrEmpty(FileName); }
        }

        public bool HasOutputFileName
        {
            get { return !string.IsNullOrEmpty(OutputFileName); }
        }

        public Encoding GetEncoding()
        {
            return GetEncodingForEncodingType(OutputFileEncodingType);
        }

        #endregion

        #region Static Helpers

        static public Encoding GetEncodingForEncodingType(EncodingType encodingType)
        {
            switch (encodingType)
            {
                case EncodingType.Default:
                    return Encoding.Default;

                case EncodingType.ASCII:
                    return Encoding.ASCII;

                case EncodingType.BigEndianUnicode:
                    return Encoding.BigEndianUnicode;

                case EncodingType.Unicode:
                    return Encoding.Unicode;

                case EncodingType.UTF7:
                    return Encoding.UTF7;

                case EncodingType.UTF8:
                    return Encoding.UTF8;

                case EncodingType.UTF32:
                    return Encoding.UTF32;

                default:
                    return null;
            }
        }

        #endregion
    }
}
