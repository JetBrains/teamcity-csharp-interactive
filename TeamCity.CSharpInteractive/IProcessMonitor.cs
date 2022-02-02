namespace TeamCity.CSharpInteractive;

using HostApi;

internal interface IProcessMonitor
{
    void Started(IStartInfo startInfo, int processId);

    ProcessResult Finished(IStartInfo startInfo, long elapsedMilliseconds, ProcessState state, int? exitCode = default, Exception? error = default);
}