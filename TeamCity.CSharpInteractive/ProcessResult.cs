namespace TeamCity.CSharpInteractive;

using Cmd;

internal record ProcessResult(ProcessState State, int? ExitCode = default);