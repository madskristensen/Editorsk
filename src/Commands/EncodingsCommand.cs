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
            SetupCommand(PackageIds.cmdHtmlDecode, new Replacement(HttpUtility.HtmlDecode));
            SetupCommand(PackageIds.cmdAttrEncode, new Replacement(HttpUtility.HtmlAttributeEncode));
            SetupCommand(PackageIds.cmdUrlEncode, new Replacement(HttpUtility.UrlEncode));
            SetupCommand(PackageIds.cmdUrlDecode, new Replacement(HttpUtility.UrlDecode));
            SetupCommand(PackageIds.cmdJavaScriptEncode, new Replacement(HttpUtility.JavaScriptStringEncode));
        }

        private void SetupCommand(int command, Replacement callback)
        {
            CommandID commandId = new CommandID(PackageGuids.guidEncodingCmdSet, command);
            OleMenuCommand menuCommand = new OleMenuCommand((s, e) => Replace(callback), commandId);

            menuCommand.BeforeQueryStatus += (s, e) =>
            {
                var document = GetTextDocument();

                if (document == null)
                    return;

                string selection = document.Selection.Text;
                menuCommand.Enabled = selection.Length > 0 && callback(selection) != selection;
            };

            _mcs.AddCommand(menuCommand);
        }

        private TextDocument GetTextDocument()
        {
            if (_dte.ActiveDocument != null)
                return _dte.ActiveDocument.Object("TextDocument") as TextDocument;

            return null;
        }

        private void Replace(Replacement callback)
        {
            TextDocument document = GetTextDocument();

            if (document == null)
                return;

            string replacement = callback(document.Selection.Text);

            try
            {
                _dte.UndoContext.Open(callback.Method.Name);
                document.Selection.Insert(replacement, 0);
            }
            finally
            {
                _dte.UndoContext.Close();
            }
        }
    }
}
