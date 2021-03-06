namespace TeamCity.CSharpInteractive;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
internal class Debugger : IActive
{
    private readonly ILog<Debugger> _log;
    private readonly IEnvironmentVariables _environmentVariables;

    public Debugger(
        ILog<Debugger> log,
        IEnvironmentVariables environmentVariables)
    {
        _log = log;
        _environmentVariables = environmentVariables;
    }

    public IDisposable Activate()
    {
        if (_environmentVariables.GetEnvironmentVariable("DEBUG_CSI") == null)
        {
            return Disposable.Empty;
        }

        _log.Warning($"\nWaiting for debugger in process [{System.Environment.ProcessId}] \"{Process.GetCurrentProcess().ProcessName}\".");
        while (!System.Diagnostics.Debugger.IsAttached)
        {
            Thread.Sleep(100);
        }

        System.Diagnostics.Debugger.Break();

        return Disposable.Empty;
    }
}