namespace Teamcity.CSharpInteractive.Tests.Integration.Core
{
    using System;
    using System.Diagnostics;
    using Pure.DI;
    using static Tags;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class ProcessFactory : IProcessFactory
    {
        private readonly Func<System.Diagnostics.Process, IProcess> _processFactory;
        private readonly string _workingDirectory;
        private readonly string _separator;

        public ProcessFactory(
            Func<System.Diagnostics.Process, IProcess> processFactory,
            [Tag(WorkingDirectory)] string workingDirectory,
            [Tag(ArgumentsSeparatorChar)] char separator)
        {
            _processFactory = processFactory ?? throw new ArgumentNullException(nameof(processFactory));
            _workingDirectory = workingDirectory;
            _separator = new string(separator, 1);
        }

        public IProcess Create(ProcessParameters parameters)
        {
            var workingDirectory = !string.IsNullOrWhiteSpace(parameters.WorkingDirectory) ? parameters.WorkingDirectory : _workingDirectory;
            var startInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                FileName = parameters.Executable,
                WorkingDirectory = workingDirectory,
                Arguments = string.Join(_separator, parameters.Arguments)
            };

            foreach (var variable in parameters.Variables)
            {
                startInfo.Environment[variable.Name] = variable.Value;
            }

            var process = new System.Diagnostics.Process
            {
                StartInfo = startInfo,
                EnableRaisingEvents = true
            };
            
            return _processFactory(process);
        }
    }
}