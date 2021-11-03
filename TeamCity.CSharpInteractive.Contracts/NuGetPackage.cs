// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable SuggestBaseTypeForParameterInConstructor
namespace TeamCity.CSharpInteractive.Contracts
{
    using System;

    public record NuGetPackage(
        string Name,
        Version Version,
        string Type,
        string Path,
        string Sha512);
}