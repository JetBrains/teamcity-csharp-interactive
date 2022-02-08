// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace HostApi;

public interface INuGet
{
    IEnumerable<NuGetPackage> Restore(NuGetRestoreSettings settings);
}