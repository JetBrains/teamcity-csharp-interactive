namespace TeamCity.CSharpInteractive;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
[Immutype.Target]
internal record CommandResult(ICommand Command, bool? Success, int? ExitCode = default);