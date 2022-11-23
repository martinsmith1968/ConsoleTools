using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathEdit.Commands
{
    [CommandDefinition(ShortName = "s", Description = "Save all changes", Order = 1000)]
    public class Save : BaseCommand
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathCollection"></param>
        /// <returns></returns>
        public override CommandResult Execute(IPathCollection pathCollection)
        {
            Display("Saving");

            // TODO: Save to CurrentControlSet

            // SET for current Console
            Environment.SetEnvironmentVariable(pathCollection.EnvironmentVariable, pathCollection.FullPath);

            Display("TODO: NOT saved to CurrentControlSet");

            return CommandResult.OK(CommandStateType.Continue, CommandControlType.SuppressList);
        }
    }
}
