using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathEdit.Commands
{
    /// <summary>
    /// 
    /// </summary>
    [CommandDefinition(ShortName = "v", Description = "View the Full Path", Order = 15)]
    public class View : BaseCommand
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathCollection"></param>
        /// <returns></returns>
        public override CommandResult Execute(IPathCollection pathCollection)
        {
            Display(string.Format("Full Path: {0}", pathCollection.FullPath));

            return CommandResult.OK(CommandStateType.Continue, CommandControlType.SuppressList);
        }
    }
}
