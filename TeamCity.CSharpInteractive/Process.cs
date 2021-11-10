// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System;
    using System.Diagnostics;
    using Contracts;

    internal class Process: IProcess
    {
        private readonly ILog<Process> _log;
        private readonly IStdOut _stdOut;
        private readonly IStdErr _stdErr;
        private readonly IStartInfoFactory _startInfoFactory;
        private readonly Text _stdOutPrefix;
        private readonly Text _stdErrPrefix;
        private readonly System.Diagnostics.Process _process;
        private CommandLine? _commandLine;
        private Text _processId;
        private int _disposed;

        public Process(
            ILog<Process> log,
            IStdOut stdOut,
            IStdErr stdErr,
            IStringService stringService,
            IStartInfoFactory startInfoFactory)
        {
            _log = log;
            _stdOut = stdOut;
            _stdErr = stdErr;
            _startInfoFactory = startInfoFactory;
            _stdOutPrefix = new Text($"{stringService.Tab}OUT: "); 
            _stdErrPrefix = new Text($"{stringService.Tab}ERR: ", Color.Error);
            _process = new System.Diagnostics.Process{ EnableRaisingEvents = true };
            _process.OutputDataReceived += ProcessOnOutputDataReceived;
            _process.ErrorDataReceived += ProcessOnErrorDataReceived;
            _process.Exited += ProcessOnExited;
        }

        public event Action<CommandLineOutput>? OnOutput;

        public event Action? OnExit;

        public int Id => _process.Id;

        public int ExitCode => _process.ExitCode;

        public bool Start(CommandLine commandLine)
        {
            _commandLine = commandLine;
            _process.StartInfo = _startInfoFactory.Create(commandLine);
            if (!_process.Start())
            {
                return false;
            }

            _processId = new Text(_process.Id.ToString().PadRight(5));
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();
            return true;
        }

        public void WaitForExit() => _process.WaitForExit();

        public bool WaitForExit(TimeSpan timeout) => _process.WaitForExit((int)timeout.TotalMilliseconds);

        public void Kill() => _process.Kill();

        private void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs e) => ProcessOutput(e, _commandLine!, false);

        private void ProcessOnErrorDataReceived(object sender, DataReceivedEventArgs e) => ProcessOutput(e, _commandLine!, true);
            
        private void ProcessOnExited(object? sender, EventArgs e) => OnExit?.Invoke();

        private void ProcessOutput(DataReceivedEventArgs e, CommandLine commandLine, bool isError)
        {
            var line = e.Data;
            if (line == default)
            {
                return;
            }

            var handler = OnOutput;
            if (handler != default)
            {
                _log.Trace(_processId, isError ? _stdErrPrefix : _stdOutPrefix, new Text(line));
                handler(new CommandLineOutput(commandLine, isError, line));
            }
            else
            {
                if (isError)
                {
                    _stdErr.WriteLine(new []{ new Text(line) });
                }
                else
                {
                    _stdOut.WriteLine(new []{ new Text(line) });
                }
            }
        }

        public void Dispose()
        {
            try
            {
                if (System.Threading.Interlocked.Exchange(ref _disposed, 1) != 0)
                {
                    return;
                }
                
                _process.Exited -= ProcessOnExited;
                _process.OutputDataReceived -= ProcessOnOutputDataReceived;
                _process.ErrorDataReceived -= ProcessOnErrorDataReceived;
                _process.Dispose();
            }
            catch (Exception exception)
            {
                _log.Trace($"Exception during disposing: {exception}.");
            }
        }
    }
}