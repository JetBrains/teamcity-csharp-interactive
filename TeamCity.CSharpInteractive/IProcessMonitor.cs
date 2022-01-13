namespace TeamCity.CSharpInteractive;

using Cmd;

internal interface IProcessMonitor
{
    void Started(IStartInfo startInfo, int processId);
    
    void Finished(long elapsedMilliseconds, ProcessState state, int? exitCode = default);
}