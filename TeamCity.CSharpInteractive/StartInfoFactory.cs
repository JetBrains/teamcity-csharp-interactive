// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System.Diagnostics;
    using Cmd;

    internal class StartInfoFactory : IStartInfoFactory
    {
        private readonly ILog<StartInfoFactory> _log;
        private readonly IEnvironment _environment;
        private readonly IWellknownValueResolver _wellknownValueResolver;

        public StartInfoFactory(
            ILog<StartInfoFactory> log,
            IEnvironment environment,
            IWellknownValueResolver wellknownValueResolver)
        {
            _log = log;
            _environment = environment;
            _wellknownValueResolver = wellknownValueResolver;
        }

        public ProcessStartInfo Create(CommandLine commandLine)
        {
            var workingDirectory = _wellknownValueResolver.Resolve(commandLine.WorkingDirectory);
            _log.Trace($"Working directory: \"{workingDirectory}\".");
            if (string.IsNullOrWhiteSpace(workingDirectory))
            {
                workingDirectory = _environment.GetPath(SpecialFolder.Working);
                _log.Trace($"The working directory has been replaced with the directory \"{workingDirectory}\".");
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = _wellknownValueResolver.Resolve(commandLine.ExecutablePath),
                WorkingDirectory = workingDirectory,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            
            _log.Trace($"File name: \"{startInfo.FileName}\".");

            foreach (var arg in commandLine.Args)
            {
                var curArg = _wellknownValueResolver.Resolve(arg);
                startInfo.ArgumentList.Add(curArg);
                _log.Trace($"Add the argument \"{curArg}\".");
            }

            foreach (var (name, value) in commandLine.Vars)
            {
                var curName = _wellknownValueResolver.Resolve(name);
                var curValue = _wellknownValueResolver.Resolve(value);
                startInfo.Environment[curName] = curValue;
                _log.Trace($"Add the environment variable {curName}={curValue}.");
            }
            
            return startInfo;
        }
    }
}