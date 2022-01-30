// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using Immutype;

[Target]
internal record Summary(bool? Success = default)
{
    public static readonly Summary Empty = new();
}