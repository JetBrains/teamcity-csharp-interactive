// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable ForCanBeConvertedToForeach
namespace TeamCity.CSharpInteractive;

using System.Diagnostics;

internal class ProcessRunner : IProcessRunner
{
    private readonly Func<IProcessManager> _processManagerFactory;
    private readonly CancellationTokenSource _cancellationTokenSource;

    public ProcessRunner(
        Func<IProcessManager> processManagerFactory,
        CancellationTokenSource cancellationTokenSource)
    {
        _processManagerFactory = processManagerFactory;
        _cancellationTokenSource = cancellationTokenSource;
    }

    public ProcessResult Run(ProcessInfo processInfo, TimeSpan timeout)
    {
        var (startInfo, monitor, handler) = processInfo;
        using var processManager = _processManagerFactory();
        if (handler != default)
        {
            processManager.OnOutput += handler;
        }

        var stopwatch = new Stopwatch();
        stopwatch.Start();
        if (!processManager.Start(startInfo, out var error))
        {
            stopwatch.Stop();
            return monitor.Finished(startInfo, stopwatch.ElapsedMilliseconds, ProcessState.Failed, default, error);
        }

        monitor.Started(startInfo, processManager.Id);
        var finished = true;
        if (timeout == TimeSpan.Zero)
        {
            processManager.WaitForExit();
        }
        else
        {
            finished = processManager.WaitForExit(timeout);
        }

        if (finished)
        {
            stopwatch.Stop();
            return monitor.Finished(startInfo, stopwatch.ElapsedMilliseconds, ProcessState.Finished, processManager.ExitCode);
        }

        processManager.Kill();
        stopwatch.Stop();
        return monitor.Finished(startInfo, stopwatch.ElapsedMilliseconds, ProcessState.Canceled);
    }

    public async Task<ProcessResult> RunAsync(ProcessInfo processInfo, CancellationToken cancellationToken)
    {
        var (startInfo, monitor, handler) = processInfo;
        if (cancellationToken == default || cancellationToken == CancellationToken.None)
        {
            cancellationToken = _cancellationTokenSource.Token;
        }

        var processManager = _processManagerFactory();
        if (handler != default)
        {
            processManager.OnOutput += handler;
        }

        var completionSource = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
        // ReSharper disable once AccessToDisposedClosure
        processManager.OnExit += () => completionSource.TrySetResult(processManager.ExitCode);
        var stopwatch = new Stopwatch();
        if (!processManager.Start(startInfo, out var error))
        {
            stopwatch.Stop();
            var result = monitor.Finished(startInfo, stopwatch.ElapsedMilliseconds, ProcessState.Failed, default, error);
            processManager.Dispose();
            return result;
        }

        monitor.Started(startInfo, processManager.Id);
        void Cancel()
        {
            if (processManager.Kill())
            {
                completionSource.TrySetCanceled(cancellationToken);
            }

            processManager.Dispose();
        }

        await using (cancellationToken.Register(Cancel, false))
        {
            using (processManager)
            {
                var exitCode = await completionSource.Task.ConfigureAwait(false);
                stopwatch.Stop();
                return monitor.Finished(startInfo, stopwatch.ElapsedMilliseconds, cancellationToken.IsCancellationRequested ? ProcessState.Canceled : ProcessState.Finished, exitCode);
            }
        }
    }
}