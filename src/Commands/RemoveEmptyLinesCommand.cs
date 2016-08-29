using System;
using System.Linq;

namespace Editorsk
{
    internal class RemoveEmptyLinesCommand : BaseCommand<RemoveEmptyLinesCommand>
    {
        private delegate void Replacement(Direction direction);

        protected override void SetupCommands()
        {
            RegisterCommand(PackageGuids.guidLinesCmdSet, PackageIds.cmdRemoveEmptyLines, Execute);
        }

        private void Execute()
        {
            try
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
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }
    }
}
