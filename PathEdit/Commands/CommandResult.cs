using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathEdit.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public enum CommandResultType
    {
        OK,
        Warning,
        Failure,
        Critical
    }

    /// <summary>
    /// 
    /// </summary>
    public enum CommandStateType
    {
        Continue,
        Exit
    }

    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum CommandControlType
    {
        None,
        ShowList,
        SuppressList
    }

    /// <summary>
    /// 
    /// </summary>
    public class CommandResult
    {
        public readonly CommandResultType Result;
        public readonly CommandStateType State;
        public readonly CommandControlType Control;
        public readonly string Message;

        public bool CanContinue
        {
            get { return State == CommandStateType.Continue; }
        }
        public bool HasMessage
        {
            get { return !string.IsNullOrEmpty(Message); }
        }

        private CommandResult(CommandResultType resultType, CommandStateType stateType)
            : this(resultType, stateType, CommandControlType.None)
        {
        }
        private CommandResult(CommandResultType resultType, CommandStateType stateType, string message)
            : this(resultType, stateType, CommandControlType.None, message)
        {
        }
        private CommandResult(CommandResultType resultType, CommandStateType stateType, CommandControlType controlType)
            : this(resultType, stateType, controlType, string.Empty)
        {
        }
        private CommandResult(CommandResultType resultType, CommandStateType stateType, CommandControlType controlType, string message)
        {
            this.Result = resultType;
            this.State = stateType;
            this.Control = controlType;
            this.Message = message;
        }

        static public CommandResult OK()
        {
            return OK(CommandStateType.Continue);
        }

        static public CommandResult OK(CommandStateType stateType)
        {
            return new CommandResult(CommandResultType.OK, stateType);
        }

        static public CommandResult OK(CommandStateType stateType, CommandControlType controlType)
        {
            return new CommandResult(CommandResultType.OK, stateType, controlType);
        }

        static public CommandResult Warning(string message)
        {
            return new CommandResult(CommandResultType.Warning, CommandStateType.Continue, message);
        }

        static public CommandResult Warning(string message, CommandControlType controlType)
        {
            return new CommandResult(CommandResultType.Warning, CommandStateType.Continue, controlType, message);
        }

        static public CommandResult Failure(string message)
        {
            return new CommandResult(CommandResultType.Failure, CommandStateType.Continue, message);
        }

        static public CommandResult Critical(string message)
        {
            return new CommandResult(CommandResultType.Failure, CommandStateType.Exit, message);
        }
    }
}
