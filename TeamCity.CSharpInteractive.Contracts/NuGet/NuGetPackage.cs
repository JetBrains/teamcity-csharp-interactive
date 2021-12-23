// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable SuggestBaseTypeForParameterInConstructor
// ReSharper disable CheckNamespace
// ReSharper disable NotAccessedPositionalProperty.Global
namespace NuGet
{
    using System;
    using System.Collections.Generic;

    public readonly record struct NuGetPackage(
        string Name,
        Version Version,
        string Type,
        string Path,
        string Sha512,
        IReadOnlyList<string> Files);
}