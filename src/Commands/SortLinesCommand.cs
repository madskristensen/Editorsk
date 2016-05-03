using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace Editorsk
{
    internal class SortLinesCommand
    {
        private DTE2 _dte;
        private OleMenuCommandService _mcs;
        private delegate void Replacement(Direction direction);

        private SortLinesCommand(IServiceProvider serviceProvider)
        {
            _dte = (DTE2)serviceProvider.GetService(typeof(DTE));
            _mcs = serviceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;

            SetupCommand(PackageIds.cmdSortAsc, Direction.Ascending);
            SetupCommand(PackageIds.cmdSortDesc, Direction.Descending);
        }

        public static SortLinesCommand Instance { get; set; }

        public static void Initialize(IServiceProvider serviceProvider)
        {
            Instance = new SortLinesCommand(serviceProvider);
        }

        private void SetupCommand(int command, Direction direction)
        {
            CommandID commandId = new CommandID(PackageGuids.guidLinesCmdSet, command);
            OleMenuCommand menuCommand = new OleMenuCommand((s, e) => Execute(direction), commandId);

            menuCommand.BeforeQueryStatus += (s, e) =>
            {
                var document = GetTextDocument();

                if (document == null)
                    return;

                string selection = document.Selection.Text;
                menuCommand.Enabled = selection.Length > 0;
            };

            _mcs.AddCommand(menuCommand);
        }

        private TextDocument GetTextDocument()
        {
            if (_dte.ActiveDocument != null)
                return _dte.ActiveDocument.Object("TextDocument") as TextDocument;

            return null;
        }

        private void Execute(Direction direction)
        {
            var document = GetTextDocument();
            var text = document.Selection.Text;
            var lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length == 0)
                return;

            string result = SortLines(direction, lines);

            try
            {
                _dte.UndoContext.Open("Sort Selected Lines");
                document.Selection.Insert(result, 0);
            }
            finally
            {
                _dte.UndoContext.Close();
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
