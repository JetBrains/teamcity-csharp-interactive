namespace TeamCity.CSharpInteractive
{
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    internal readonly record struct CompilationDiagnostics(ICommand SourceCommand, ReadOnlyCollection<Diagnostic> Diagnostics);
}