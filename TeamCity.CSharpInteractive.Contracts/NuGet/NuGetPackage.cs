// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable SuggestBaseTypeForParameterInConstructor
// ReSharper disable CheckNamespace
// ReSharper disable NotAccessedPositionalProperty.Global
namespace NuGet
{
    using System;
    using System.Collections.Generic;

    [Immutype.Target]
    public readonly record struct NuGetPackage(
        string Name,
        Version Version,
        Versioning.NuGetVersion NuGetVersion,
        string Type,
        string Path,
        string Sha512,
        IReadOnlyList<string> Files,
        bool HasTools,
        bool IsServiceable);
}