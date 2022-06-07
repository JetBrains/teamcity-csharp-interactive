// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InvertIf
namespace TeamCity.CSharpInteractive;

using System.Collections.ObjectModel;
using System.Diagnostics;

internal class Statistics : IStatistics
{
    private readonly Stopwatch _stopwatch = new();
    private readonly List<string> _errors = new();
    private readonly List<string> _warnings = new();

    public IReadOnlyCollection<string> Errors
    {
        get
        {
            lock (_errors)
            {
                return new ReadOnlyCollection<string>(_errors);   
            }
        }
    }

    public IReadOnlyCollection<string> Warnings
    {
        get
        {
            lock (_warnings)
            {
                return new ReadOnlyCollection<string>(_warnings);   
            }
        }
    }

    public TimeSpan TimeElapsed => _stopwatch.Elapsed;

    public IDisposable Start()
    {
        _stopwatch.Start();
        return Disposable.Create(() => _stopwatch.Stop());
    }

    public void RegisterError(string error)
    {
        error = error.Trim();
        if (!string.IsNullOrWhiteSpace(error))
        {
            lock (_errors)
            {
                _errors.Add(error);   
            }
        }
    }

    public void RegisterWarning(string warning)
    {
        warning = warning.Trim();
        if (!string.IsNullOrWhiteSpace(warning))
        {
            lock (_warnings)
            {
                _warnings.Add(warning);   
            }
        }
    }
}