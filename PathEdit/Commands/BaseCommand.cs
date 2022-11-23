using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using NRA.Util;

namespace PathEdit.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class BaseCommand : ICommand
    {
        #region Static Configuration Properties

        static public bool BackupPath
        {
            get;
            set;
        }
        static public bool ValidateNewPaths
        {
            get;
            set;
        }

        #endregion

        private List<string> _Parameters = new List<string>();
        private TextWriter _Writer = null;

        protected string[] Parameters
        {
            get { return _Parameters.ToArray(); }
        }

        public BaseCommand()
        {
        }

        static public void Display()
        {
            ConsoleHelper.Display();
        }
        static public void Display(string s)
        {
            ConsoleHelper.Display(s);
        }

        protected void ValidatePath(string path)
        {
            if (!Directory.Exists(path))
                throw new ValidationError(string.Format("Path {0} does not exist", path));
        }

        protected void ValidatePosition(int position, IPathCollection pathCollection)
        {
            if (position < 1 || position > pathCollection.Count)
                throw new ValidationError(string.Format("Invalid position (must be 1 - {0})", pathCollection.Count));
        }

        static public CommandDefinition[] GetCommands()
        {
            List<CommandDefinition> definitions = new List<CommandDefinition>();

            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (t.IsAbstract)
                    continue;

                if (typeof(ICommand).IsAssignableFrom(t))
                {
                    definitions.Add(new CommandDefinition(t));
                }
            }

            return definitions.ToArray();
        }

        #region ICommand Members

        public TextWriter Writer
        {
            get { return _Writer; }
            set { _Writer = value; }
        }

        protected string GetParameter(int index)
        {
            if (index < 0 || index > _Parameters.Count)
                return null;
            return _Parameters[index];
        }

        public void SetParameters(string[] parameters)
        {
            _Parameters.Clear();
            _Parameters.AddRange(parameters);
        }

        public void AddParameter(string parameter)
        {
            _Parameters.Add(parameter);
        }

        static public CommandDefinitionAttribute GetCommandDefinition(Type CommandType)
        {
            CommandDefinitionAttribute[] atts = (CommandDefinitionAttribute[])CommandType.GetCustomAttributes(typeof(CommandDefinitionAttribute), true);
            return (atts.Length > 0) ? atts[0] : CommandDefinitionAttribute.Empty;
        }

        public virtual void Validate(IPathCollection pathCollection)
        {
            CommandDefinitionAttribute def = GetCommandDefinition(GetType());

            if (def.MinParameterCount > 0)
            {
                if (_Parameters.Count < def.MinParameterCount)
                    throw new ValidationError("Not enough Parameters !");
            }
            if (def.MaxParameterCount > 0)
            {
                if (_Parameters.Count > def.MaxParameterCount)
                    throw new ValidationError("Too many Parameters !");
            }
        }

        public abstract CommandResult Execute(IPathCollection pathCollection);

        #endregion

        #region IDisposable Members

        public virtual void Dispose()
        {
            if (_Parameters != null)
                _Parameters.Clear();
        }

        #endregion
    }
}
