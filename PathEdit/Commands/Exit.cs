using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NRA.Util;

namespace PathEdit.Commands
{
    [CommandDefinition(ShortName = "x", Description = "Exit saving all changes")]
    public class Exit : BaseCommand
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathCollection"></param>
        /// <returns></returns>
        public override CommandResult Execute(IPathCollection pathCollection)
        {
            using (ICommand saveCommand = new Save())
            {
                CommandResult result = saveCommand.Execute(pathCollection);

                if (result != null && result.Result == CommandResultType.OK)
                {
                    Display("Exiting");

                    return CommandResult.OK(CommandStateType.Exit);
                }
                else
                    return result;
            }
        }
    }
}
