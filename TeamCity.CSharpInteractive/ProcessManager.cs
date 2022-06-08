// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using HostApi;

[ExcludeFromCodeCoverage]
internal class ProcessManager : IProcessManager
{
    private static readonly Text StdOutPrefix = new("OUT: ");
    private static readonly Text StdErrPrefix = new("ERR: ", Color.Error);
    private readonly ILog<ProcessManager> _log;
    private readonly IProcessOutputWriter _processOutputWriter;
    private readonly IStartInfoFactory _startInfoFactory;
    private readonly IExitTracker _exitTracker;
    private readonly Process _process;
    private int _disposed;
    private IStartInfo? _startInfo;
    private string _description = "The";

    public ProcessManager(
        ILog<ProcessManager> log,
        IProcessOutputWriter processOutputWriter,
        IStartInfoFactory startInfoFactory,
        IExitTracker exitTracker)
    {
        _log = log;
        _processOutputWriter = processOutputWriter;
        _startInfoFactory = startInfoFactory;
        _exitTracker = exitTracker;
        _process = new Process {EnableRaisingEvents = true};
        _process.OutputDataReceived += ProcessOnOutputDataReceived;
        _process.ErrorDataReceived += ProcessOnErrorDataReceived;
        _process.Exited += ProcessOnExited;
    }

    public event Action<Output>? OnOutput;

    public event Action? OnExit;

    public int Id { get; private set; }

    public int ExitCode => _process.ExitCode;

    public bool Start(IStartInfo info, out Exception? error)
    {
        _startInfo = info;
        _process.StartInfo = _startInfoFactory.Create(info);
        try
        {
            if (!_process.Start())
            {
                error = default;
                return false;
            }
        }
        catch (Exception e)
        {
            error = e;
            return false;
        }

        try
        {
            Id = _process.Id;
        }
        catch
        {
            // ignored
        }

        _description = _startInfo.GetDescription(Id) + " process";
        _process.BeginOutputReadLine();
        _process.BeginErrorReadLine();
        error = default;
        return true;
    }

    public Task WaitForExitAsync(CancellationToken cancellationToken) =>
        _process.WaitForExitAsync(cancellationToken);
    
    public bool WaitForExit(TimeSpan timeout)
    {
        if (timeout != TimeSpan.Zero)
        {
            return _process.WaitForExit((int)timeout.TotalMilliseconds);
        }

        _process.WaitForExit();
        return true;
    }

    public bool Kill()
    {
        try
        {
            _log.Trace(() => new[] {new Text($"{_description} is terminating.")}, _description);
            _process.Kill();
            _log.Trace(() => new[] {new Text($"{_description} was terminated.")}, _description);
            if (_exitTracker.IsTerminating)
            {
                _log.Warning($"{_description} was forcibly stopped. Try to wait for completion or cancel this process on one's own to avoid this warning.");
            }
        }
        catch (Exception ex)
        {
            _log.Warning(new Text($"{_description} was not terminated properly with error \"{ex.Message}\"."));
            return false;
        }

        return true;
    }

    private void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs e) => ProcessOutput(e, false);

    private void ProcessOnErrorDataReceived(object sender, DataReceivedEventArgs e) => ProcessOutput(e, true);

    private void ProcessOnExited(object? sender, EventArgs e)
    {
        _process.WaitForExit();
        OnExit?.Invoke();
    }

    private void ProcessOutput(DataReceivedEventArgs e, bool isError)
    {
        var line = e.Data;
        if (line == default)
        {
            return;
        }

        var handler = OnOutput;
        var output = new Output(_startInfo!, isError, line, Id);
        if (handler != default)
        {
            _log.Trace(() => new[] {isError ? StdErrPrefix : StdOutPrefix, new Text(line)}, _description);
            handler(output);
        }
        else
        {
            _processOutputWriter.Write(output);
        }
    }

    public void Dispose()
    {
        try
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0)
            {
                return;
            }

            OnOutput = default;
            OnExit = default;
            _process.Exited -= ProcessOnExited;
            _process.OutputDataReceived -= ProcessOnOutputDataReceived;
            _process.ErrorDataReceived -= ProcessOnErrorDataReceived;
            _process.Dispose();
        }
        catch (Exception exception)
        {
            _log.Trace(() => new[] {new Text($"Exception during disposing: {exception}.")});
        }
    }
}