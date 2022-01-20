// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using System.Collections;
using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
internal class HostIntegrationCodeSource: ICodeSource
{
    private static readonly string[] ImplicitUsing = Array.Empty<string>();

    // ReSharper disable once IdentifierTypo
    private static readonly string ImplicitUsings = string.Join(System.Environment.NewLine, ImplicitUsing);
        
    public string Name => string.Empty;

    public bool Internal => true;

    public IEnumerator<string?> GetEnumerator() => Enumerable.Repeat(ImplicitUsings, 1).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}