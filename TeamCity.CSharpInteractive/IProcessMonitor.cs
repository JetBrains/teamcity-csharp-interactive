namespace TeamCity.CSharpInteractive;

using Cmd;

internal interface IProcessMonitor
{
    void Starting(IStartInfo startInfo, int processId);
    
    void Started();

    void Finished(long elapsedMilliseconds, ProcessState state, int? exitCode = default);
}