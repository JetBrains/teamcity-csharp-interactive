// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable SuggestBaseTypeForParameterInConstructor
// ReSharper disable CheckNamespace
namespace NuGet
{
    using System;

    public record NuGetPackage(
        string Name,
        Version Version,
        string Type,
        string Path,
        string Sha512);
}