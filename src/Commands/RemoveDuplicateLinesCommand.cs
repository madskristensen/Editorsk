using System;
using System.ComponentModel.Design;
using System.Linq;
using Microsoft.VisualStudio.Shell;

namespace Editorsk
{
    internal class RemoveDuplicateLinesCommand : BaseCommand<RemoveDuplicateLinesCommand>
    {
        protected override void SetupCommands()
        {
            RegisterCommand(PackageGuids.guidLinesCmdSet, PackageIds.cmdRemoveDuplicateLines, Execute);
        }

        private void Execute()
        {
            try
            {
                var document = GetTextDocument();
                var lines = GetSelectedLines(document);

                string result = string.Join(Environment.NewLine, lines.Distinct(new LineComparer()));

                if (result == document.Selection.Text)
                    return;

                using (UndoContext("Remove Duplicate Lines"))
                {
                    document.Selection.Insert(result, 0);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }
    }
}
