// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

[Immutype.Target]
internal record Summary(bool? Success = default)
{
    public static readonly Summary Empty = new();
}