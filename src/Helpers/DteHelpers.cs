using System;
using EnvDTE;
using EnvDTE80;

namespace Editorsk
{
    public static class DteHelpers
    {
        public static TextDocument GetActiveTextDocument(this DTE2 dte)
        {
            return dte.ActiveDocument?.Object("TextDocument") as TextDocument;
        }

        public static IDisposable Undo(this DTE2 dte, string name)
        {
            dte.UndoContext.Open(name);
            return new Disposable(dte.UndoContext.Close);
        }
    }
}
