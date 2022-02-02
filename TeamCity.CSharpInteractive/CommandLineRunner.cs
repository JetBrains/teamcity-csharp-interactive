// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using HostApi;

internal class CommandLineRunner : ICommandLineRunner
{
    private readonly IHost _host;
    private readonly IProcessRunner _processRunner;
    private readonly Func<IProcessMonitor> _monitorFactory;
    private readonly IProcessResultHandler _processResultHandler;

    public CommandLineRunner(
        IHost host,
        IProcessRunner processRunner,
        Func<IProcessMonitor> monitorFactory,
        IProcessResultHandler processResultHandler)
    {
        _host = host;
        _processRunner = processRunner;
        _monitorFactory = monitorFactory;
        _processResultHandler = processResultHandler;
    }

    public int? Run(ICommandLine commandLine, Action<Output>? handler = default, TimeSpan timeout = default)
    {
        var result = _processRunner.Run(new ProcessInfo(commandLine.GetStartInfo(_host), _monitorFactory(), handler), timeout);
        _processResultHandler.Handle(result, handler);
        return result.ExitCode;
    }

    public async Task<int?> RunAsync(ICommandLine commandLine, Action<Output>? handler = default, CancellationToken cancellationToken = default)
    {
        var result = await _processRunner.RunAsync(new ProcessInfo(commandLine.GetStartInfo(_host), _monitorFactory(), handler), cancellationToken);
        _processResultHandler.Handle(result, handler);
        return result.ExitCode;
    }
}