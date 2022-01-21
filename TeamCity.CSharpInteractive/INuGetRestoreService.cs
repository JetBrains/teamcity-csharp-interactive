namespace TeamCity.CSharpInteractive;

using Script.NuGet;

internal interface INuGetRestoreService
{
    bool TryRestore(RestoreSettings settings, out string projectAssetsJson);
}