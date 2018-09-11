using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace Editorsk
{
    internal class SortLinesCommand : BaseCommand<SortLinesCommand>
    {
        private delegate void Replacement(Direction direction);

        protected override void SetupCommands()
        {
            var cmdAsc = new CommandID(PackageGuids.guidLinesCmdSet, PackageIds.cmdSortAsc);
            RegisterCommand(cmdAsc, () => Execute(Direction.Ascending));

            var cmdDesc = new CommandID(PackageGuids.guidLinesCmdSet, PackageIds.cmdSortDesc);
            RegisterCommand(cmdDesc, () => Execute(Direction.Descending));
        }

        private void Execute(Direction direction)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                TextDocument document = GetTextDocument();
                IEnumerable<string> lines = GetSelectedLines(document);

                string result = SortLines(direction, lines);

                if (result == document.Selection.Text)
                    return;

                using (UndoContext("Sort Selected Lines"))
                {
                    document.Selection.Insert(result, 0);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
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
