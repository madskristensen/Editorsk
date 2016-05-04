using System;
using System.ComponentModel.Design;
using System.Web;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace Editorsk
{
    internal class EncodingCommand
    {
        private DTE2 _dte;
        private OleMenuCommandService _mcs;
        private delegate string Replacement(string original);

        private EncodingCommand(IServiceProvider serviceProvider)
        {
            _dte = (DTE2)serviceProvider.GetService(typeof(DTE));
            _mcs = serviceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            SetupCommands();
        }

        public static EncodingCommand Instance { get; set; }

        public static void Initialize(IServiceProvider serviceProvider)
        {
            Instance = new EncodingCommand(serviceProvider);
        }

        private void SetupCommands()
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
            _mcs.AddCommand(menuCommand);
        }

        private void Replace(Replacement callback)
        {
            var document = _dte.GetActiveTextDocument();
            string result = callback(document.Selection.Text);

            if (result == document.Selection.Text)
                return;

            using (_dte.Undo(callback.Method.Name))
            {
                document.Selection.Insert(result, 0);
            }
        }
    }
}
