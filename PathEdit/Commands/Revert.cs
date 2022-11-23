using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathEdit.Commands
{
    /// <summary>
    /// 
    /// </summary>
    [CommandDefinition(ShortName = "rv", Description = "Discard all changes and Revert to last saved value", Order = 1010)]
    public class Revert : BaseCommand
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathCollection"></param>
        /// <returns></returns>
        public override CommandResult Execute(IPathCollection pathCollection)
        {
            Display("Reverting to original paths");

            pathCollection.Refresh();

            return CommandResult.OK();
        }
    }
}
