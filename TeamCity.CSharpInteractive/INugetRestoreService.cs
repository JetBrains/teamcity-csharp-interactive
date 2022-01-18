namespace TeamCity.CSharpInteractive
{
    using NuGet;

    internal interface INugetRestoreService
    {
        bool TryRestore(RestoreSettings settings, out string projectAssetsJson);
    }
}