namespace TeamCity.CSharpInteractive;

using HostApi;

internal interface IProcessMonitor
{
    void Started(IStartInfo startInfo, int processId);
}
