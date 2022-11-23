using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathEdit.Commands
{
    /// <summary>
    /// 
    /// </summary>
    [CommandDefinition(ShortName = "m", Description = "Move a path to a new position", MinParameterCount = 2, MaxParameterCount = 2, Parameters = "n m", Order = 110)]
    public class Move : BaseCommand
    {
        private int _Position1;
        private int _Position2;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathCollection"></param>
        public override void Validate(IPathCollection pathCollection)
        {
            base.Validate(pathCollection);

            if (!int.TryParse(GetParameter(0), out _Position1))
                throw new ValidationError("Position 1 is not numeric");
            ValidatePosition(_Position1, pathCollection);
            _Position1 -= 1; // Coz our array is 0 based

            if (!int.TryParse(GetParameter(1), out _Position2))
                throw new ValidationError("Position 2 is not numeric");
            ValidatePosition(_Position2, pathCollection);
            _Position2 -= 1; // Coz our array is 0 based
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathCollection"></param>
        /// <returns></returns>
        public override CommandResult Execute(IPathCollection pathCollection)
        {
            Display(string.Format("Moving path at {0} to {1}", _Position1, _Position2));

            string path = pathCollection.GetPath(_Position1);

            pathCollection.RemovePath(_Position1);
            pathCollection.InsertPath(_Position2, path);

            return CommandResult.OK();
        }
    }
}
