namespace TeamCity.CSharpInteractive;

using Script.Cmd;

internal interface IProcessMonitor
{
    void Started(IStartInfo startInfo, int processId);
    
    void Finished(IStartInfo startInfo, long elapsedMilliseconds, ProcessState state, int? exitCode = default);
}