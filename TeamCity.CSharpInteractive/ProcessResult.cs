// ReSharper disable NotAccessedPositionalProperty.Global
namespace TeamCity.CSharpInteractive;

using HostApi;

internal record ProcessResult(
    IStartInfo StartInfo,
    int? ProcessId,
    ProcessState State,
    long ElapsedMilliseconds,
    Text[] Description,
    int? ExitCode = default,
    Exception? Error = default);
