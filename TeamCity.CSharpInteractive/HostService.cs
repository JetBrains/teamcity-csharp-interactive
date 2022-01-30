// ReSharper disable UnusedMember.Global
// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using HostApi;

internal class HostService : IHost
{
    private readonly ILog<HostService> _log;
    private readonly ISettings _settings;
    private readonly IStdOut _stdOut;

    public HostService(
        ILog<HostService> log,
        ISettings settings,
        IStdOut stdOut,
        IProperties properties)
    {
        _log = log;
        _settings = settings;
        _stdOut = stdOut;
        Props = properties;
    }

    public IHost Host => this;

    public IReadOnlyList<string> Args => _settings.ScriptArguments;

    public IProperties Props { get; }

    public void WriteLine() => _stdOut.WriteLine();

    public void WriteLine<T>(T line, Color color = Color.Default) => _stdOut.WriteLine(new Text(line?.ToString() ?? string.Empty, color));

    public void Error(string? error, string? errorId = default)
    {
        if (error != default)
        {
            _log.Error(new ErrorId(errorId ?? "Unknown"), error);
        }
    }

    public void Warning(string? warning)
    {
        if (warning != default)
        {
            _log.Warning(warning);
        }
    }

    public void Info(string? text)
    {
        if (text != default)
        {
            _log.Info(text);
        }
    }

    public void Trace(string? trace, string? origin = default)
    {
        if (trace != default)
        {
            _log.Trace(() => new[] {new Text(trace)}, origin ?? string.Empty);
        }
    }

    public T GetService<T>() => Composer.Resolve<T>();
}