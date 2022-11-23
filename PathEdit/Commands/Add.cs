using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PathEdit.Commands
{
    [CommandDefinition(ShortName = "a", Description="Add a new path", MinParameterCount = 1, Parameters = "{path}", Order = 100)]
    class Add : BaseCommand
    {
        private string _Path;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathCollection"></param>
        public override void Validate(IPathCollection pathCollection)
        {
            base.Validate(pathCollection);

            // Save
            _Path = GetParameter(0);
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
            Display(string.Format("Adding: {0}", _Path));
            pathCollection.AddPath(_Path);

            return CommandResult.OK();
        }
    }
}
