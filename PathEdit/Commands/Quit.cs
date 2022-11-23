using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathEdit.Commands
{
    [CommandDefinition(ShortName = "q", Description = "Exit without saving any changes")]
    public class Quit : BaseCommand
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathCollection"></param>
        /// <returns></returns>
        public override CommandResult Execute(IPathCollection pathCollection)
        {
            Display("Exiting");

            return CommandResult.OK(CommandStateType.Exit);
        }
    }
}
