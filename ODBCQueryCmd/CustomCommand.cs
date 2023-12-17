using System.Reflection;

namespace ODBCQueryCmd
{
    /// <summary>
    ///
    /// </summary>
    public interface ICustomCommand
    {
        void SetParameters(string parameters);
        bool Execute();
    }

    /// <summary>
    ///
    /// </summary>
    public abstract class BaseCommand : ICustomCommand
    {
        protected List<string> _Parameters = new List<string>();

        /// <summary>
        /// Creates the command.
        /// </summary>
        /// <param name="statement">The statement.</param>
        /// <returns></returns>
        static public ICustomCommand CreateCommand(string statement)
        {
            ICustomCommand command = null;

            if (string.IsNullOrEmpty(statement))
                return null;

            if (statement.StartsWith(ODBCQueryCmd.Program.CUSTOMCOMMAND_PREFIX))
                statement = statement.Substring(ODBCQueryCmd.Program.CUSTOMCOMMAND_PREFIX.Length);

            string[] bits = statement.Split(" ".ToCharArray());

            switch (bits[0].ToUpper())
            {
                case "WAIT":
                    command = new WaitCommand();
                    break;

                default:
                    command = null;
                    break;
            }

            if (command != null)
            {
                string parameters = statement.Substring(bits[0].Length).Trim();

                command.SetParameters(parameters);
            }

            return command;
        }

        #region ICustomCommand Members

        public virtual void SetParameters(string parameters)
        {
            _Parameters.Clear();

            if (!string.IsNullOrEmpty(parameters))
                _Parameters.AddRange(parameters.Split(" ".ToCharArray()));
        }

        public abstract bool Execute();

        #endregion

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return GetCommandName(GetType());
        }

        /// <summary>
        /// Gets the name of the command.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        static public string GetCommandName(Type t)
        {
            string commandName = t.Name;

            if (commandName.EndsWith("Command"))
                commandName = commandName.Substring(0, commandName.Length - 7).ToUpper();

            return commandName;
        }

        /// <summary>
        /// Gets the custom commands.
        /// </summary>
        /// <returns></returns>
        static public Type[] GetCustomCommands()
        {
            List<Type> types = new List<Type>();

            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (t.IsAbstract)
                    continue;

                if (typeof(ICustomCommand).IsAssignableFrom(t))
                    types.Add(t);
            }

            return types.ToArray();
        }
    }

    /// <summary>
    /// Command to Wait for a period of time
    /// </summary>
    public class WaitCommand : BaseCommand
    {
        /// <summary>
        /// Gets the wait seconds.
        /// </summary>
        public int WaitSeconds
        {
            get
            {
                if (_Parameters.Count < 1)
                    return 0;

                int seconds = 0;
                if (!Int32.TryParse(_Parameters[0], out seconds))
                    return 0;

                return seconds;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitCommand"/> class.
        /// </summary>
        public WaitCommand()
        {
        }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            if (WaitSeconds <= 0)
                return false;

            Thread.Sleep(new TimeSpan(0, 0, WaitSeconds));

            return true;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0} {1}", base.ToString(), WaitSeconds);
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class RowHashCommand : BaseCommand
    {
        /// <summary>
        /// Gets or sets the SQL.
        /// </summary>
        /// <value>
        /// The SQL.
        /// </value>
        public string SQL
        {
            get
            {
                if (_Parameters.Count < 1)
                    return null;

                return _Parameters[0];
            }
        }

        public RowHashCommand()
        {
        }

        public override bool Execute()
        {
            throw new NotImplementedException();
        }
    }
}
