// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using System.Diagnostics;
using Script.Cmd;

internal class StartInfoFactory : IStartInfoFactory
{
    private readonly ILog<StartInfoFactory> _log;
    private readonly IEnvironment _environment;

    public StartInfoFactory(
        ILog<StartInfoFactory> log,
        IEnvironment environment)
    {
        _log = log;
        _environment = environment;
    }

    public ProcessStartInfo Create(IStartInfo info)
    {
        var workingDirectory = info.WorkingDirectory;
        var directory = workingDirectory;
        var description = info.GetDescription();
        _log.Trace(() => new []{new Text($"Working directory: \"{directory}\".")}, description);
        if (string.IsNullOrWhiteSpace(workingDirectory))
        {
            workingDirectory = _environment.GetPath(SpecialFolder.Working);
            _log.Trace(() => new []{new Text($"The working directory has been replaced with the directory \"{workingDirectory}\".")}, description);
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = info.ExecutablePath,
            WorkingDirectory = workingDirectory,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
            
        _log.Trace(() => new []{new Text($"File name: \"{startInfo.FileName}\".")}, description);

        foreach (var arg in info.Args)
        {
            startInfo.ArgumentList.Add(arg);
            _log.Trace(() => new []{new Text($"Add the argument \"{arg}\".")}, description);
        }

        foreach (var (name, value) in info.Vars)
        {
            startInfo.Environment[name] = value;
            _log.Trace(() => new []{new Text($"Add the environment variable {name}={value}.")}, description);
        }
            
        return startInfo;
    }
}