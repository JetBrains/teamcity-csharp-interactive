// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable SuggestBaseTypeForParameterInConstructor
// ReSharper disable NotAccessedPositionalProperty.Global
namespace HostApi;

using Immutype;
using NuGet.Versioning;

[Target]
public readonly record struct NuGetPackage(
    string Name,
    Version Version,
    NuGetVersion NuGetVersion,
    string Type,
    string Path,
    string Sha512,
    IReadOnlyList<string> Files,
    bool HasTools,
    bool IsServiceable);