namespace TeamCity.CSharpInteractive;

using NuGet;

internal interface INuGetRestoreService
{
    bool TryRestore(RestoreSettings settings, out string projectAssetsJson);
}