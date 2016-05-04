using System;
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
        public OleMenuCommandService CommandService { get; private set; }

        public static void Initialize(IServiceProvider serviceProvider)
        {
            Instance = new T
            {
                DTE = (DTE2)serviceProvider.GetService(typeof(DTE)),
                CommandService = (OleMenuCommandService)serviceProvider.GetService(typeof(IMenuCommandService))
            };

            Instance.SetupCommands();
        }

        protected abstract void SetupCommands();

        public TextDocument GetTextDocument()
        {
            return DTE.ActiveDocument?.Object("TextDocument") as TextDocument;
        }

        public IDisposable UndoContext(string name)
        {
            DTE.UndoContext.Open(name);
            return new Disposable(DTE.UndoContext.Close);
        }

        public string[] GetSelectedLines(TextDocument document)
        {
            return document.Selection.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
