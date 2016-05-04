using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using Microsoft.VisualStudio.Shell;

namespace Editorsk
{
    internal class SortLinesCommand : BaseCommand<SortLinesCommand>
    {
        private delegate void Replacement(Direction direction);

        protected override void SetupCommands()
        {
            SetupCommand(PackageIds.cmdSortAsc, Direction.Ascending);
            SetupCommand(PackageIds.cmdSortDesc, Direction.Descending);
        }

        private void SetupCommand(int command, Direction direction)
        {
            var commandId = new CommandID(PackageGuids.guidLinesCmdSet, command);
            var menuCommand = new OleMenuCommand((s, e) => Execute(direction), commandId);
            CommandService.AddCommand(menuCommand);
        }

        private void Execute(Direction direction)
        {
            var document = GetTextDocument();
            var lines = GetSelectedLines(document);

            string result = SortLines(direction, lines);

            if (result == document.Selection.Text)
                return;

            using (UndoContext("Sort Selected Lines"))
            {
                document.Selection.Insert(result, 0);
            }
        }

        private string SortLines(Direction direction, IEnumerable<string> lines)
        {
            if (direction == Direction.Ascending)
                lines = lines.OrderBy(t => t);
            else
                lines = lines.OrderByDescending(t => t);

            return string.Join(Environment.NewLine, lines);
        }
    }

    public enum Direction
    {
        Ascending,
        Descending
    }
}
