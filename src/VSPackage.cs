using System;
using System.Runtime.InteropServices;
using System.Threading;
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

            await EncodingCommand.Initialize(this);
            await TransformCommand.Initialize(this);
            await SortLinesCommand.Initialize(this);
            await RemoveEmptyLinesCommand.Initialize(this);
            await RemoveDuplicateLinesCommand.Initialize(this);
        }
    }
}
