using System.ComponentModel.Design;
using System.Web;
using Microsoft.VisualStudio.Shell;

namespace Editorsk
{
    internal class EncodingCommand : BaseCommand<EncodingCommand>
    {
        private delegate string Replacement(string original);

        protected override void SetupCommands()
        {
            SetupCommand(PackageIds.cmdHtmlEncode, new Replacement(HttpUtility.HtmlEncode));
            SetupCommand(PackageIds.cmdAttrEncode, new Replacement(HttpUtility.HtmlAttributeEncode));
            SetupCommand(PackageIds.cmdHtmlDecode, new Replacement(HttpUtility.HtmlDecode));
            SetupCommand(PackageIds.cmdUrlEncode, new Replacement(HttpUtility.UrlEncode));
            SetupCommand(PackageIds.cmdUrlDecode, new Replacement(HttpUtility.UrlDecode));
            SetupCommand(PackageIds.cmdJavaScriptEncode, new Replacement(HttpUtility.JavaScriptStringEncode));
        }

        private void SetupCommand(int command, Replacement callback)
        {
            CommandID commandId = new CommandID(PackageGuids.guidEncodingCmdSet, command);
            OleMenuCommand menuCommand = new OleMenuCommand((s, e) => Replace(callback), commandId);
            CommandService.AddCommand(menuCommand);
        }

        private void Replace(Replacement callback)
        {
            var document = GetTextDocument();
            string result = callback(document.Selection.Text);

            if (result == document.Selection.Text)
                return;

            using (UndoContext(callback.Method.Name))
            {
                document.Selection.Insert(result, 0);
            }
        }
    }
}
