// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System.Diagnostics;

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

        public ProcessStartInfo Create(Contracts.CommandLine commandLine)
        {
            var workingDirectory = commandLine.WorkingDirectory;
            _log.Trace($"Working directory: \"{workingDirectory}\".");
            if (string.IsNullOrWhiteSpace(workingDirectory))
            {
                workingDirectory = _environment.GetPath(SpecialFolder.Working);
                _log.Trace($"The working directory has been replaced with the directory \"{workingDirectory}\".");
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = commandLine.ExecutablePath,
                WorkingDirectory = workingDirectory,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            
            _log.Trace($"File name: \"{startInfo.FileName}\".");

            foreach (var arg in commandLine.Args)
            {
                startInfo.ArgumentList.Add(arg);
                _log.Trace($"Add the argument \"{arg}\".");
            }

            foreach (var (name, value) in commandLine.Vars)
            {
                startInfo.Environment[name] = value;
                _log.Trace($"Add the environment variable {name}={value}.");
            }
            
            return startInfo;
        }
    }
}