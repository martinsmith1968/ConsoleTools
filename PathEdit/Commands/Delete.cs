using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathEdit.Commands
{
    [CommandDefinition(ShortName = "d", Description = "Delete Path at position n", MinParameterCount = 1, Parameters = "n", Order = 102)]
    public class Delete : BaseCommand
    {
        private int _Position;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathCollection"></param>
        public override void Validate(IPathCollection pathCollection)
        {
            base.Validate(pathCollection);

            if (!int.TryParse(GetParameter(0), out _Position))
                throw new ValidationError("Position is not numeric");
            ValidatePosition(_Position, pathCollection);
            _Position -= 1; // Coz our array is 0 based
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathCollection"></param>
        /// <returns></returns>
        public override CommandResult Execute(IPathCollection pathCollection)
        {
            pathCollection.RemovePath(_Position);

            return CommandResult.OK();
        }
    }
}
