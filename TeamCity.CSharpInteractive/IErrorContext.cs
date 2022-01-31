namespace TeamCity.CSharpInteractive;

using System.Diagnostics.CodeAnalysis;

internal interface IErrorContext
{
    bool TryGetSourceName([NotNullWhen(true)] out string? name);
}