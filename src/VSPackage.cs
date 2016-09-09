using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Threading;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace Editorsk
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", Vsix.Version, IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.guidPackageString)]
    public sealed class VSPackage : AsyncPackage
    {
        protected override async System.Threading.Tasks.Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await Logger.InitializeAsync(this, Vsix.Name);

            var dte = await GetServiceAsync(typeof(DTE)) as DTE2;
            var commandService = await GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;

            EncodingCommand.Initialize(dte, commandService);
            TransformCommand.Initialize(dte, commandService);
            SortLinesCommand.Initialize(dte, commandService);
            RemoveEmptyLinesCommand.Initialize(dte, commandService);
            RemoveDuplicateLinesCommand.Initialize(dte, commandService);
        }
    }
}
