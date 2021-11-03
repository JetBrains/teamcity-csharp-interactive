// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable ForCanBeConvertedToForeach
namespace TeamCity.CSharpInteractive
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Contracts;

    internal class CommandLineService: ICommandLine
    {
        private readonly ILog<CommandLineService> _log;
        private readonly Func<IProcess> _processFactory;
        
        public CommandLineService(
            ILog<CommandLineService> log,
            Func<IProcess> processFactory)
        {
            _log = log;
            _processFactory = processFactory;
        }

        public int? Run(CommandLine commandLine, Action<CommandLineOutput>? handler = default, TimeSpan timeout = default)
        {
            using var process = _processFactory();
            process.OnOutput += handler;
            var info = commandLine.ToString();
            if (!process.Start(commandLine))
            {
                _log.Trace($"{info} - cannot start.");
                return default;
            }

            var finished = true;
            if (timeout == TimeSpan.Zero)
            {
                _log.Trace($"{info} - started with process id {process.Id}.");
                process.WaitForExit();
            }
            else
            {
                _log.Trace($"{info} - started with id {process.Id} and with timeout {timeout}.");
                finished = process.WaitForExit(timeout);
            }

            if (finished)
            {
                _log.Trace($"{info} - finished with exit code {process.ExitCode}.");
                return process.ExitCode;
            }

            _log.Trace($"{info} - timeout is expired.");
            Kill(process, info);

            return default;
        }

        public async Task<int?> RunAsync(CommandLine commandLine, Action<CommandLineOutput>? handler = default, CancellationToken cancellationToken = default)
        {
            var process = _processFactory();
            process.OnOutput += handler;
            var completionSource = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
            // ReSharper disable once AccessToDisposedClosure
            process.OnExit += () => completionSource.TrySetResult(process.ExitCode);
            var info = commandLine.ToString();
            if (!process.Start(commandLine))
            {
                _log.Trace($"{info} - cannot start process.");
                process.Dispose();
                return default;
            }
            
            _log.Trace($"{info} - started with process id {process.Id}.");
            
            void Cancel()
            {
                if (Kill(process, info))
                {
                    completionSource.TrySetCanceled(cancellationToken);
                }
                
                process.Dispose();
                _log.Trace($"{info} - canceled.");
            }

            await using (cancellationToken.Register(Cancel, false))
            {
                using (process)
                {
                    var exitCode = await completionSource.Task.ConfigureAwait(false);
                    _log.Trace($"{info} - finished with exit code {exitCode}.");
                    return exitCode;
                }
            }
        }
        
        private bool Kill(IProcess process, string info)
        {
            try
            {
                _log.Trace($"{info} - try to kill process.");
                process.Kill();
                _log.Trace($"{info} - killed.");
            }
            catch (Exception ex)
            {
                _log.Trace($"{info} - failed to kill: {ex.Message}.");
                return false;
            }

            return true;
        }
    }
}