using System.Windows.Forms;

namespace PathEdit.Commands
{
    [CommandDefinition(ShortName = "cc", Description = "Copy new path to Clipboard", MinParameterCount = 0, Parameters = "", Order = 300)]
    class ClipboardCopy : BaseCommand
    {
        /// <summary>Executes the specified path collection.</summary>
        /// <param name="pathCollection">The path collection.</param>
        /// <returns></returns>
        public override CommandResult Execute(IPathCollection pathCollection)
        {
            Clipboard.SetText(pathCollection.FullPath, TextDataFormat.Text);
            Display("Copied new PATH to clipboard");

            return CommandResult.OK(CommandStateType.Continue, CommandControlType.SuppressList);
        }
    }
}
