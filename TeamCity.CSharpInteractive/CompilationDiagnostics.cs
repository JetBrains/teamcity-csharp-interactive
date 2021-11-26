namespace TeamCity.CSharpInteractive
{
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    internal readonly struct CompilationDiagnostics
    {
        public readonly ICommand SourceCommand;
        public readonly ReadOnlyCollection<Diagnostic> Diagnostics;

        public CompilationDiagnostics(ICommand sourceCommand, ReadOnlyCollection<Diagnostic> diagnostics)
        {
            SourceCommand = sourceCommand;
            Diagnostics = diagnostics;
        }
    }
}