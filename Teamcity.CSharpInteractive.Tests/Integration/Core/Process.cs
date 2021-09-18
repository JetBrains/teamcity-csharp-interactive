// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive.Tests.Integration.Core
{
    using System;
    using System.Diagnostics;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal class Process : IProcess
    {
        private readonly System.Diagnostics.Process _process;
        
        public Process(System.Diagnostics.Process process) =>
            _process = process ?? throw new ArgumentNullException(nameof(process));

        public ExitCode Run(IProcessListener listener)
        {
            if (listener == null) throw new ArgumentNullException(nameof(listener));

            void OnOutputDataReceived(object sender, DataReceivedEventArgs args) =>
                listener.OnStdOut(args.Data);

            void OnErrorDataReceived(object sender, DataReceivedEventArgs args) =>
                listener.OnStdErr(args.Data);

            _process.OutputDataReceived += OnOutputDataReceived;
            _process.ErrorDataReceived += OnErrorDataReceived;
            listener.OnStart(_process.StartInfo);
            _process.Start();
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();

            try
            {
                _process.WaitForExit();
            }
            finally
            {
                _process.OutputDataReceived -= OnOutputDataReceived;
                _process.ErrorDataReceived -= OnErrorDataReceived;
            }

            ExitCode exitCode = _process.ExitCode;
            listener.OnExitCode(exitCode);
            return exitCode;
        }

        public void Dispose() => _process.Dispose();
    }
}