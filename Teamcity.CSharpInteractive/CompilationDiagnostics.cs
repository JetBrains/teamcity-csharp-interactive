namespace Teamcity.CSharpInteractive
{
    using System.Collections.ObjectModel;
    using Microsoft.CodeAnalysis;

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