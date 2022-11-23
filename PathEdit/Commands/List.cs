using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathEdit.Commands
{
    [CommandDefinition(ShortName = "l", Description = "Display the list of Paths", Order = 10)]
    public class List : BaseCommand
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathCollection"></param>
        /// <returns></returns>
        public override CommandResult Execute(IPathCollection pathCollection)
        {
            return CommandResult.OK(CommandStateType.Continue);
        }
    }
}
