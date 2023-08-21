// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using HostApi;

internal class ProcessMonitor : IProcessMonitor
{
    private readonly ILog<ProcessMonitor> _log;
    private readonly IEnvironment _environment;

    public ProcessMonitor(
        ILog<ProcessMonitor> log,
        IEnvironment environment)
    {
        _log = log;
        _environment = environment;
    }

    public void Started(IStartInfo startInfo, int processId)
    {
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
}
