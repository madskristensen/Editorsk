using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
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
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                TextDocument document = GetTextDocument();
                IEnumerable<string> lines = GetSelectedLines(document);

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
