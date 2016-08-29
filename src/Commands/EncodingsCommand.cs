using System;
using System.Web;

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

        private void SetupCommand(int commandId, Replacement callback)
        {
            RegisterCommand(PackageGuids.guidEncodingCmdSet, commandId, () => Execute(callback));
        }

        private void Execute(Replacement callback)
        {
            try
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
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }
    }
}
