// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable ForCanBeConvertedToForeach
namespace TeamCity.CSharpInteractive;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using HostApi;

internal class ProcessRunner : IProcessRunner
{
    private readonly Func<IProcessManager> _processManagerFactory;

    public ProcessRunner(Func<IProcessManager> processManagerFactory) =>
        _processManagerFactory = processManagerFactory;

    public ProcessResult Run(ProcessInfo processInfo, TimeSpan timeout)
    {
        var processManager = _processManagerFactory();
        var process = new Process(processInfo, processManager);
        if (!process.TryStart(out var processResult))
        {
            return processResult;
        }

        var finished = processManager.WaitForExit(timeout);

        return process.Finish(finished);
    }

    public async Task<ProcessResult> RunAsync(ProcessInfo processInfo, CancellationToken cancellationToken)
    {
        var processManager =  _processManagerFactory();
        var process = new Process(processInfo, processManager);
        if (!process.TryStart(out var processResult))
        {
            return processResult;
        }
        
        await processManager.WaitForExitAsync(cancellationToken);
        var finished = !cancellationToken.IsCancellationRequested;

        return process.Finish(finished);
    }

    private class Process
    {
        public Process(ProcessInfo processInfo, IProcessManager processManager)
        {
            ProcessInfo = processInfo;
            ProcessManager = processManager;
            Stopwatch = new Stopwatch();
        }
        
        private IProcessManager ProcessManager { get; }
        
        private ProcessInfo ProcessInfo { get; }
        
        private Stopwatch Stopwatch { get; }

        private IStartInfo StartInfo => ProcessInfo.StartInfo;

        private IProcessMonitor Monitor => ProcessInfo.Monitor;

        private Action<Output>? Handler => ProcessInfo.Handler;
        
        public bool TryStart([MaybeNullWhen(true)] out ProcessResult processResult)
        {
            if (Handler != default)
            {
                ProcessManager.OnOutput += Handler;
            }

            Stopwatch.Start();
            if (!ProcessManager.Start(StartInfo, out var error))
            {
                Stopwatch.Stop();
                {
                    processResult = ProcessResult.FailedToStart(StartInfo, Stopwatch.ElapsedMilliseconds, error);
                    if (Handler != default)
                    {
                        ProcessManager.OnOutput -= Handler;
                    }
                    return false;
                }
            }

            Monitor.Started(StartInfo, ProcessManager.Id);
            processResult = default;
            return true;
        }
    
        public ProcessResult Finish(bool finished)
        {
            if (Handler != default)
            {
                ProcessManager.OnOutput -= Handler;
            }

            if (finished)
            {
                Stopwatch.Stop();
                return ProcessResult.RanToCompletion(StartInfo, ProcessManager.Id, Stopwatch.ElapsedMilliseconds, ProcessManager.ExitCode);
            }

            ProcessManager.Kill();
            Stopwatch.Stop();
            return ProcessResult.CancelledByTimeout(StartInfo, ProcessManager.Id, Stopwatch.ElapsedMilliseconds);
        }
    }
}
