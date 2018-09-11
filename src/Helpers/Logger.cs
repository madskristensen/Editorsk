using System;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using task = System.Threading.Tasks.Task;

internal static class Logger
{
    private static string _name;
    private static IVsOutputWindowPane _pane;
    private static IVsOutputWindow _output;

    public static async task InitializeAsync(AsyncPackage package, string name)
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

        _output = await package.GetServiceAsync(typeof(SVsOutputWindow)) as IVsOutputWindow;
        _name = name;
    }

    public static void Log(object message)
    {
        ThreadHelper.ThrowIfNotOnUIThread();
        try
        {
            if (EnsurePane())
            {
                _pane.OutputString(DateTime.Now.ToString() + ": " + message + Environment.NewLine);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.Write(ex);
        }
    }

    private static bool EnsurePane()
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        if (_pane == null && _output != null)
        {
            var guid = Guid.NewGuid();
            _output.CreatePane(ref guid, _name, 1, 1);
            _output.GetPane(ref guid, out _pane);
        }

        return _pane != null;
    }
}