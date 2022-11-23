using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathEdit.Commands
{
    // TODO: Allow specifying a command name which then displays detailed help (+ examples?) about the command
    [CommandDefinition(ShortName = "h", Description = "Display a list of commands", MinParameterCount = 0, MaxParameterCount = 1, Order = 0)]
    public class Help : BaseCommand
    {
        private bool _Debug = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathCollection"></param>
        public override void Validate(IPathCollection pathCollection)
        {
            base.Validate(pathCollection);

            if (Parameters.Length > 0)
            {
                string debug = GetParameter(0);

                if (debug.Trim().ToLower() == "debug")
                    _Debug = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathCollection"></param>
        /// <returns></returns>
        public override CommandResult Execute(IPathCollection pathCollection)
        {
            List<CommandDefinition> definitions = new List<CommandDefinition>();
            definitions.AddRange(GetCommands().OrderBy(d => d.Order).ThenBy(d => d.Name));

            if (definitions.Count > 0)
            {
                int maxNameLength = definitions.Max(
                    d => d.GetDescription(CommandDescriptionFlags.Name
                                        | CommandDescriptionFlags.ShortName
                                        | CommandDescriptionFlags.Parameters
                                        ).Length
                    );

                foreach (CommandDefinition def in definitions)
                {
                    Display(string.Format("{0}{1}  {2}",
                                        _Debug ? string.Format("{0} ", def.Order) : string.Empty,
                                        def.GetDescription().PadRight(maxNameLength),
                                        def.Description
                                        )
                           );
                }
            }

            return CommandResult.OK(CommandStateType.Continue, CommandControlType.SuppressList);
        }
    }
}
