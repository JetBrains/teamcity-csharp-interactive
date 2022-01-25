namespace TeamCity.CSharpInteractive;

using HostApi;

internal interface INuGetRestoreService
{
    bool TryRestore(NuGetRestore settings, out string projectAssetsJson);
}