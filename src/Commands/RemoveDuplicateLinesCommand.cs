using System;
using System.ComponentModel.Design;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace Editorsk
{
    internal class RemoveDuplicateLinesCommand
    {
        private DTE2 _dte;
        private OleMenuCommandService _mcs;
        private delegate void Replacement(Direction direction);

        private RemoveDuplicateLinesCommand(IServiceProvider serviceProvider)
        {
            _dte = (DTE2)serviceProvider.GetService(typeof(DTE));
            _mcs = serviceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;

            SetupCommand(PackageIds.cmdRemoveDuplicateLines);
        }

        public static RemoveDuplicateLinesCommand Instance { get; set; }

        public static void Initialize(IServiceProvider serviceProvider)
        {
            Instance = new RemoveDuplicateLinesCommand(serviceProvider);
        }

        private void SetupCommand(int command)
        {
            CommandID commandId = new CommandID(PackageGuids.guidLinesCmdSet, command);
            OleMenuCommand menuCommand = new OleMenuCommand((s, e) => Execute(), commandId);
            _mcs.AddCommand(menuCommand);
        }

        private void Execute()
        {
            var document = _dte.GetActiveTextDocument();
            var lines = document.GetSelectedLines();

            string result = string.Join(Environment.NewLine, lines.Distinct(new LineComparer()));

            if (result == document.Selection.Text)
                return;

            using (_dte.Undo("Remove Duplicate Lines"))
            {
                document.Selection.Insert(result, 0);
            }
        }
    }
}
