// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using Cmd;
    using Contracts;

    [ExcludeFromCodeCoverage]
    internal class ProcessManager: IProcessManager
    {
        private static readonly Text StdOutPrefix = new("OUT: ");
        private static readonly Text StdErrPrefix = new("ERR: ", Color.Error);
        private readonly ILog<ProcessManager> _log;
        private readonly IProcessOutputWriter _processOutputWriter;
        private readonly IStartInfoFactory _startInfoFactory;
        private readonly Process _process;
        private int _disposed;
        private IStartInfo? _startInfo;
        private string _description = "The";

        public ProcessManager(
            ILog<ProcessManager> log,
            IProcessOutputWriter processOutputWriter,
            IStartInfoFactory startInfoFactory)
        {
            _log = log;
            _processOutputWriter = processOutputWriter;
            _startInfoFactory = startInfoFactory;
            _process = new Process{ EnableRaisingEvents = true };
            _process.OutputDataReceived += ProcessOnOutputDataReceived;
            _process.ErrorDataReceived += ProcessOnErrorDataReceived;
            _process.Exited += ProcessOnExited;
        }

        public event Action<Output>? OnOutput;

        public event Action? OnExit;

        public int Id { get; private set; }

        public int ExitCode => _process.ExitCode;

        public bool Start(IStartInfo info)
        {
            _startInfo = info;
            _process.StartInfo = _startInfoFactory.Create(info);
            try
            {
                if (!_process.Start())
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                _log.Error(ErrorId.Process, e.Message);
                return false;
            }

            try
            {
                 Id = _process.Id;
            }
            catch
            {
                // ignored
            }

            _description = _startInfo.GetDescription(Id) + " process";
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();
            return true;
        }

        public void WaitForExit() => _process.WaitForExit();

        public bool WaitForExit(TimeSpan timeout) => _process.WaitForExit((int)timeout.TotalMilliseconds);

        public bool Kill()
        {
            try
            {
                _log.Trace(() => new []{new Text($"{_description} is terminating.")}, _description);
                _process.Kill();
                _log.Trace(() => new []{new Text($"{_description} was terminated.")}, _description);
            }
            catch (Exception ex)
            {
                _log.Warning(new Text($"{_description} was not terminated properly with error \"{ex.Message}\"."));
                return false;
            }

            return true;
        }

        private void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs e) => ProcessOutput(e, false);

        private void ProcessOnErrorDataReceived(object sender, DataReceivedEventArgs e) => ProcessOutput(e, true);
            
        private void ProcessOnExited(object? sender, EventArgs e) => OnExit?.Invoke();

        private void ProcessOutput(DataReceivedEventArgs e, bool isError)
        {
            var line = e.Data;
            if (line == default)
            {
                return;
            }

            var handler = OnOutput;
            var output = new Output(_startInfo!, isError, line, Id);
            if (handler != default)
            {
                _log.Trace(() => new []{ isError ? StdErrPrefix : StdOutPrefix, new Text(line) }, _description);
                handler(output);
            }
            else
            {
                _processOutputWriter.Write(output);
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
                _log.Trace(() => new []{new Text($"Exception during disposing: {exception}.")});
            }
        }
    }
}