// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable SuggestBaseTypeForParameterInConstructor
// ReSharper disable NotAccessedPositionalProperty.Global
namespace Script.NuGet;

[Immutype.Target]
public readonly record struct NuGetPackage(
    string Name,
    Version Version,
    global::NuGet.Versioning.NuGetVersion NuGetVersion,
    string Type,
    string Path,
    string Sha512,
    IReadOnlyList<string> Files,
    bool HasTools,
    bool IsServiceable);