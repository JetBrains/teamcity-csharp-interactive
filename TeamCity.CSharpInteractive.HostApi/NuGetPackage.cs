// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable SuggestBaseTypeForParameterInConstructor
// ReSharper disable NotAccessedPositionalProperty.Global
namespace HostApi;

[Immutype.Target]
public readonly record struct NuGetPackage(
    string Name,
    Version Version,
    NuGet.Versioning.NuGetVersion NuGetVersion,
    string Type,
    string Path,
    string Sha512,
    IReadOnlyList<string> Files,
    bool HasTools,
    bool IsServiceable);