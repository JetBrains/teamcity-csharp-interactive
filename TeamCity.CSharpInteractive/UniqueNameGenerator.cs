// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
internal class UniqueNameGenerator : IUniqueNameGenerator
{
    public string Generate() => Guid.NewGuid().ToString().Replace("-", string.Empty)[..8];
}