// ReSharper disable NotAccessedPositionalProperty.Global
namespace TeamCity.CSharpInteractive;

internal record ProcessResult(ProcessState State, int? ExitCode = default);