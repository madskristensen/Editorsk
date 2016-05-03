using System;
using System.ComponentModel.Design;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace Editorsk
{
    internal class RemoveEmptyLinesCommand
    {
        private DTE2 _dte;
        private OleMenuCommandService _mcs;
        private delegate void Replacement(Direction direction);

        private RemoveEmptyLinesCommand(IServiceProvider serviceProvider)
        {
            _dte = (DTE2)serviceProvider.GetService(typeof(DTE));
            _mcs = serviceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;

            SetupCommand(PackageIds.cmdRemoveEmptyLines);
        }

        public static RemoveEmptyLinesCommand Instance { get; set; }

        public static void Initialize(IServiceProvider serviceProvider)
        {
            Instance = new RemoveEmptyLinesCommand(serviceProvider);
        }

        private void SetupCommand(int command)
        {
            CommandID commandId = new CommandID(PackageGuids.guidLinesCmdSet, command);
            OleMenuCommand menuCommand = new OleMenuCommand((s, e) => Execute(), commandId);

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

        private void Execute()
        {
            var document = GetTextDocument();
            var text = document.Selection.Text;
            var lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length == 0)
                return;

            string result = string.Join(Environment.NewLine, lines.Where(s => !string.IsNullOrWhiteSpace(s)));

            try
            {
                _dte.UndoContext.Open("Remove Empty Lines");
                document.Selection.Insert(result, 0);
            }
            finally
            {
                _dte.UndoContext.Close();
            }
        }
    }
}
