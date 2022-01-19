// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using System;
using System.Threading;
using System.Threading.Tasks;
using Cmd;
using Contracts;

internal class CommandLineService: ICommandLine
{
    private readonly IHost _host;
    private readonly IProcessRunner _processRunner;
    private readonly Func<IProcessMonitor> _monitorFactory;

    public CommandLineService(
        IHost host,
        IProcessRunner processRunner,
        Func<IProcessMonitor> monitorFactory)
    {
        _host = host;
        _processRunner = processRunner;
        _monitorFactory = monitorFactory;
    }

    public int? Run(IProcess process, Action<Output>? handler = default, TimeSpan timeout = default) =>
        _processRunner.Run(new ProcessRun(process.GetStartInfo(_host.Host), _monitorFactory(), handler), timeout).ExitCode;

    public async Task<int?> RunAsync(IProcess process, Action<Output>? handler = default, CancellationToken cancellationToken = default)
    {
        var result = await _processRunner.RunAsync(new ProcessRun(process.GetStartInfo(_host.Host), _monitorFactory(), handler), cancellationToken);
        return result.ExitCode;
    }
}