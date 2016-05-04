using System;
using System.ComponentModel.Design;
using System.Linq;
using Microsoft.VisualStudio.Shell;

namespace Editorsk
{
    internal class RemoveEmptyLinesCommand : BaseCommand<RemoveEmptyLinesCommand>
    {
        private delegate void Replacement(Direction direction);

        protected override void SetupCommands()
        {
            CommandID commandId = new CommandID(PackageGuids.guidLinesCmdSet, PackageIds.cmdRemoveEmptyLines);
            OleMenuCommand menuCommand = new OleMenuCommand((s, e) => Execute(), commandId);
            CommandService.AddCommand(menuCommand);
        }

        private void Execute()
        {
            var document = GetTextDocument();
            var lines = GetSelectedLines(document);

            string result = string.Join(Environment.NewLine, lines.Where(s => !string.IsNullOrWhiteSpace(s)));

            if (result == document.Selection.Text)
                return;

            using (UndoContext("Remove Empty Lines"))
            {
                document.Selection.Insert(result, 0);
            }
        }
    }
}
