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

        public ProcessStartInfo Create(IStartInfo info)
        {
            var workingDirectory = _wellknownValueResolver.Resolve(info.WorkingDirectory);
            var directory = workingDirectory;
            _log.Trace(() => new []{new Text($"Working directory: \"{directory}\".")});
            if (string.IsNullOrWhiteSpace(workingDirectory))
            {
                workingDirectory = _environment.GetPath(SpecialFolder.Working);
                _log.Trace(() => new []{new Text($"The working directory has been replaced with the directory \"{workingDirectory}\".")});
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = _wellknownValueResolver.Resolve(info.ExecutablePath),
                WorkingDirectory = workingDirectory,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            
            _log.Trace(() => new []{new Text($"File name: \"{startInfo.FileName}\".")});

            foreach (var arg in info.Args)
            {
                var curArg = _wellknownValueResolver.Resolve(arg);
                startInfo.ArgumentList.Add(curArg);
                _log.Trace(() => new []{new Text($"Add the argument \"{curArg}\".")});
            }

            foreach (var (name, value) in info.Vars)
            {
                var curName = _wellknownValueResolver.Resolve(name);
                var curValue = _wellknownValueResolver.Resolve(value);
                startInfo.Environment[curName] = curValue;
                _log.Trace(() => new []{new Text($"Add the environment variable {curName}={curValue}.")});
            }
            
            return startInfo;
        }
    }
}