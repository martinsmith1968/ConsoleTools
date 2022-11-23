using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathEdit.Commands
{
    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum CommandDescriptionFlags
    {
        None = 0x0,
        Name = 0x1,
        ShortName = 0x2,
        Parameters = 0x4,
    }

    /// <summary>
    /// 
    /// </summary>
    public class CommandDefinitionAttribute : Attribute
    {
        public int Order
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
        public string ShortName
        {
            get;
            set;
        }
        public int MinParameterCount
        {
            get;
            set;
        }
        public int MaxParameterCount
        {
            get;
            set;
        }
        public string Parameters
        {
            get;
            set;
        }
        public string Description
        {
            get;
            set;
        }

        public CommandDefinitionAttribute()
            : base()
        {
            Order = int.MaxValue;
        }

        static public CommandDefinitionAttribute Empty
        {
            get { return new CommandDefinitionAttribute(); }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CommandDefinition
    {
        private CommandDefinitionAttribute _DefinitionAttribute = null;

        public Type CommandType
        {
            get;
            private set;
        }
        public CommandDefinitionAttribute DefinitionAttribute
        {
            get { return _DefinitionAttribute ?? CommandDefinitionAttribute.Empty; }
        }
        public int Order
        {
            get { return DefinitionAttribute.Order; }
        }
        public string Name
        {
            get { return string.IsNullOrEmpty(DefinitionAttribute.Name) ? CommandType.Name : DefinitionAttribute.Name; }
        }
        public string ShortName
        {
            get { return DefinitionAttribute.ShortName; }
        }
        public string Description
        {
            get{ return DefinitionAttribute.Description; }
        }
        public string Parameters
        {
            get { return DefinitionAttribute.Parameters; }
        }

        public CommandDefinition(Type type)
        {
            if (typeof(ICommand).IsAssignableFrom(type))
            {
                CommandType = type;

                _DefinitionAttribute = BaseCommand.GetCommandDefinition(CommandType);
            }
        }

        public string GetDescription()
        {
            return GetDescription(CommandDescriptionFlags.Name | CommandDescriptionFlags.ShortName | CommandDescriptionFlags.Parameters);
        }
        public string GetDescription(CommandDescriptionFlags flags)
        {
            StringBuilder sb = new StringBuilder();

            if ((flags & CommandDescriptionFlags.Name) > 0)
                sb.Append(Name);

            if ((flags & CommandDescriptionFlags.ShortName) > 0 && !string.IsNullOrEmpty(ShortName))
            {
                if (sb.Length > 0)
                    sb.Append(" ");
                sb.AppendFormat("[{0}]", ShortName);
            }

            if ((flags & CommandDescriptionFlags.Parameters) > 0 && !string.IsNullOrEmpty(Parameters))
            {
                if (sb.Length > 0)
                    sb.Append(" ");
                sb.AppendFormat("{0}", Parameters);
            }

            return sb.ToString();
        }

        public bool IsCommandName(string name)
        {
            if (string.Compare(name, this.Name, true) == 0)
                return true;
            if (!string.IsNullOrEmpty(name) && string.Compare(name, this.ShortName, true) == 0)
                return true;

            return false;
        }
    }
}
