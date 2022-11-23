using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NRA.Util;

namespace PathEdit.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICommand : IDisposable
    {
        TextWriter Writer { get; set; }
        void SetParameters(string[] parameters);
        void AddParameter(string parameter);
        void Validate(IPathCollection pathCollection);
        CommandResult Execute(IPathCollection pathCollection);
    }

    /// <summary>
    /// 
    /// </summary>
    public class ValidationError : Exception
    {
        public ValidationError()
            : base("Validation Error")
        {
        }

        public ValidationError(string message)
            : base(message)
        {
        }

        public ValidationError(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
