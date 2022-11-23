using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PathEdit.Commands
{
    [CommandDefinition(ShortName = "e", Description = "Edit path at position n", MinParameterCount = 2, Parameters = "n {path}", Order = 101)]
    public class Edit : BaseCommand
    {
        private int _Position;
        private string _Path;

        /// <summary>
        /// 
        /// </summary>
        public override void Validate(IPathCollection pathCollection)
        {
            base.Validate(pathCollection);

            if (!int.TryParse(GetParameter(0), out _Position))
                throw new ValidationError("Position is not numeric");
            ValidatePosition(_Position, pathCollection);
            _Position -= 1; // Coz our array is 0 based

            _Path = GetParameter(1);
            if (ValidateNewPaths)
            {
                ValidatePath(_Path);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathCollection"></param>
        /// <returns></returns>
        public override CommandResult Execute(IPathCollection pathCollection)
        {
            pathCollection.SetPath(_Position, _Path);

            return CommandResult.OK();
        }
    }
}
