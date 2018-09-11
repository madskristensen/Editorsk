using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace Editorsk
{
    abstract class BaseCommand<T> where T : BaseCommand<T>, new()
    {
        public static T Instance { get; private set; }
        public DTE2 DTE { get; private set; }

        private OleMenuCommandService CommandService { get; set; }

        public static void Initialize(DTE2 dte, OleMenuCommandService commandService)
        {
            Instance = new T
            {
                DTE = dte,
                CommandService = commandService
            };

            Instance.SetupCommands();
        }

        protected abstract void SetupCommands();

        protected void RegisterCommand(CommandID commandId, Action action)
        {
            var menuCommand = new OleMenuCommand((s, e) => action(), commandId);
            CommandService.AddCommand(menuCommand);
        }

        protected void RegisterCommand(Guid commandGuid, int commandId, Action action)
        {
            var cmd = new CommandID(commandGuid, commandId);
            RegisterCommand(cmd, action);
        }

        public TextDocument GetTextDocument()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return DTE.ActiveDocument?.Object("TextDocument") as TextDocument;
        }

        public IDisposable UndoContext(string name)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            DTE.UndoContext.Open(name);
            return new Disposable(DTE.UndoContext.Close);
        }

        public IEnumerable<string> GetSelectedLines(TextDocument document)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            int firstLine = Math.Min(document.Selection.TopLine, document.Selection.BottomLine);
            int lineCount = document.Selection.TextRanges.Count;

            document.Selection.MoveToLineAndOffset(firstLine, 1);
            document.Selection.LineDown(true, lineCount);
            document.Selection.CharRight(true, -1);

            for (int i = 1; i <= document.Selection.TextRanges.Count; i++)
            {
                TextRange range = document.Selection.TextRanges.Item(i);
                yield return range.StartPoint.GetText(range.EndPoint).TrimEnd();
            }
        }
    }
}
