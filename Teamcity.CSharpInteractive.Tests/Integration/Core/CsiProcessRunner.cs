namespace Teamcity.CSharpInteractive.Tests.Integration.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    internal class CsiProcessRunner : IProcessRunner
    {
        private readonly IProcessFactory _processFactory;

        public CsiProcessRunner(IProcessFactory processFactory) => _processFactory = processFactory;

        public IProcessResult Run(IEnumerable<CommandLineArgument> args, IEnumerable<EnvironmentVariable> vars)
        {
            var csiProcess = new ProcessParameters(
                "dotnet",
                Environment.CurrentDirectory,
                new [] { new CommandLineArgument("dotnet-csi.dll") }.Concat(args),
                vars);

            var process = _processFactory.Create(csiProcess);
            var listener = new Listener();
            process.Run(listener);
            return listener;
        }
        
        private class Listener: IProcessListener, IProcessResult
        {
            private readonly List<string> _stdOut = new();
            private readonly List<string> _stdErr = new();
            
            public ExitCode ExitCode { get; private set; }

            public IReadOnlyCollection<string> StdOut => _stdOut.AsReadOnly();

            public IReadOnlyCollection<string> StdErr => _stdErr.AsReadOnly();

            public void OnStart(ProcessStartInfo startInfo) { }

            public void OnStdOut(string? line)
            {
                if (line != null)
                {
                    _stdOut.Add(line);
                }
            }

            public void OnStdErr(string? line)
            {
                if (line != null)
                {
                    _stdErr.Add(line);
                }
            }

            public void OnExitCode(ExitCode exitCode) => ExitCode = exitCode;
        }
    }
}