// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using HostApi;

internal class ProcessMonitor : IProcessMonitor
{
    private readonly ILog<ProcessMonitor> _log;
    private readonly IEnvironment _environment;
    private int? _processId;

    public ProcessMonitor(
        ILog<ProcessMonitor> log,
        IEnvironment environment)
    {
        _log = log;
        _environment = environment;
    }

    public void Started(IStartInfo startInfo, int processId)
    {
        _processId = processId;
        var executable = new List<Text>
        {
            new($"{startInfo.GetDescription(processId)} process started ", Color.Highlighted),
            new(startInfo.ExecutablePath.EscapeArg())
        };

        foreach (var arg in startInfo.Args)
        {
            executable.Add(Text.Space);
            executable.Add(new Text(arg.EscapeArg()));
        }

        _log.Info(executable.ToArray());

        var workingDirectory = startInfo.WorkingDirectory;
        if (string.IsNullOrWhiteSpace(workingDirectory))
        {
            workingDirectory = _environment.GetPath(SpecialFolder.Working);
        }

        if (!string.IsNullOrWhiteSpace(workingDirectory))
        {
            _log.Info(new Text("in directory: "), new Text(workingDirectory.EscapeArg()));
        }
    }

    public void Finished(IStartInfo startInfo, long elapsedMilliseconds, ProcessState state, int? exitCode = default)
    {
        var footer = GetFooter(startInfo, exitCode, elapsedMilliseconds, state).ToArray();
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (state)
        {
            case ProcessState.Failed:
                // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                _log.Error(ErrorId.Process, footer);
                break;

            case ProcessState.Canceled:
                _log.Warning(footer);
                break;

            default:
                _log.Info(footer);
                break;
        }
    }

    private IEnumerable<Text> GetFooter(IStartInfo startInfo, int? exitCode, long elapsedMilliseconds, ProcessState? state)
    {
        string? stateText;
        var color = Color.Default;
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (state)
        {
            case ProcessState.Failed:
                stateText = "failed";
                break;

            case ProcessState.Canceled:
                stateText = "canceled";
                break;

            default:
                stateText = "finished";
                color = Color.Highlighted;
                break;
        }

        yield return new Text($"{startInfo.GetDescription(_processId)} process ", color);
        yield return new Text(stateText, color);
        yield return new Text($" (in {elapsedMilliseconds} ms)");
        if (exitCode.HasValue)
        {
            yield return new Text($" with exit code {exitCode}");
        }

        yield return new Text(".");
    }
}