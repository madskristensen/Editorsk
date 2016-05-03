using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace Editorsk
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", Vsix.Version, IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.guidPackageString)]
    public sealed class VSPackage : Package
    {
        protected override void Initialize()
        {
            EncodingCommand.Initialize(this);
            TransformCommand.Initialize(this);
            SortLinesCommand.Initialize(this);
            RemoveEmptyLinesCommand.Initialize(this);
            RemoveDuplicateLinesCommand.Initialize(this);

            base.Initialize();
        }
    }
}
