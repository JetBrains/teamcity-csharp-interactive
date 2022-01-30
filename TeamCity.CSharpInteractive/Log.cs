// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using System.Diagnostics.CodeAnalysis;
using HostApi;

[ExcludeFromCodeCoverage]
internal class Log<T> : ILog<T>
{
    private readonly IStatistics _statistics;
    private readonly ISettings _settings;
    private readonly IStdOut _stdOut;

    public Log(
        ISettings settings,
        IStdOut stdOut,
        IStatistics statistics)
    {
        _settings = settings;
        _stdOut = stdOut;
        _statistics = statistics;
    }

    public void Error(ErrorId id, params Text[] error)
    {
        if (!error.Any())
        {
            return;
        }

        _statistics.RegisterError(string.Join("", error.Select(i => i.Value)));
        _stdOut.WriteLine(GetMessage(error, Color.Error));
    }

    public void Warning(params Text[] warning)
    {
        if (!warning.Any())
        {
            return;
        }

        _statistics.RegisterWarning(string.Join("", warning.Select(i => i.Value)));
        _stdOut.WriteLine(GetMessage(warning, Color.Warning));
    }

    public void Info(params Text[] message)
    {
        if (_settings.VerbosityLevel >= VerbosityLevel.Normal)
        {
            _stdOut.WriteLine(GetMessage(message, Color.Default));
        }
    }

    public void Trace(Func<Text[]> traceMessagesFactory, string origin)
    {
        // ReSharper disable once InvertIf
        if (_settings.VerbosityLevel >= VerbosityLevel.Diagnostic)
        {
            origin = string.IsNullOrWhiteSpace(origin) ? typeof(T).Name : origin.Trim();
            _stdOut.WriteLine(GetMessage(new Text($"{origin,-40}") + traceMessagesFactory(), Color.Trace));
        }
    }

    private static Text[] GetMessage(Text[] message, Color defaultColor) => message.WithDefaultColor(defaultColor);
}