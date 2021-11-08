// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable ForCanBeConvertedToForeach
namespace TeamCity.CSharpInteractive
{
    using System;
    using System.Linq;
    using System.Text;
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
            var info = new Text(GetInfo(commandLine), Color.Header);
            if (!process.Start(commandLine))
            {
                _log.Trace(info, new Text(" - cannot start."));
                return default;
            }

            var finished = true;
            if (timeout == TimeSpan.Zero)
            {
                _log.Trace(info, new Text($" - started with process id {process.Id}."));
                process.WaitForExit();
            }
            else
            {
                _log.Trace(info, new Text($" - started with id {process.Id} and with timeout {timeout}."));
                finished = process.WaitForExit(timeout);
            }

            if (finished)
            {
                _log.Trace(info, new Text($" - finished with exit code {process.ExitCode}."));
                return process.ExitCode;
            }

            _log.Trace(info, new Text(" - timeout is expired."));
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
            var info = new Text(GetInfo(commandLine), Color.Header);
            if (!process.Start(commandLine))
            {
                _log.Trace(info, new Text(" - cannot start process."));
                process.Dispose();
                return default;
            }
            
            _log.Trace(info, new Text($" - started with process id {process.Id}."));
            
            void Cancel()
            {
                if (Kill(process, info))
                {
                    completionSource.TrySetCanceled(cancellationToken);
                }
                
                process.Dispose();
                _log.Trace(info, new Text(" - canceled."));
            }

            await using (cancellationToken.Register(Cancel, false))
            {
                using (process)
                {
                    var exitCode = await completionSource.Task.ConfigureAwait(false);
                    _log.Trace(info, new Text($" - finished with exit code {exitCode}."));
                    return exitCode;
                }
            }
        }
        
        private bool Kill(IProcess process, Text info)
        {
            try
            {
                _log.Trace(info, new Text(" - try to kill process."));
                process.Kill();
                _log.Trace(info, new Text(" - killed."));
            }
            catch (Exception ex)
            {
                _log.Trace(info, new Text($" - failed to kill: {ex.Message}."));
                return false;
            }

            return true;
        }
        
        private static string GetInfo(CommandLine commandLine)
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(commandLine.WorkingDirectory))
            {
                sb.Append(commandLine.WorkingDirectory);
                sb.Append(" => ");
            }

            sb.Append('[');
            sb.Append(Escape(commandLine.ExecutablePath));
            foreach (var arg in commandLine.Args)
            {
                sb.Append(' ');
                sb.Append(Escape(arg));
            }
            sb.Append(']');

            // ReSharper disable once InvertIf
            if (commandLine.Vars.Any())
            {
                sb.Append(" with environment variables ");
                foreach (var (name, value) in commandLine.Vars)
                {
                    sb.Append('[');
                    sb.Append(name);
                    sb.Append('=');
                    sb.Append(value);
                    sb.Append(']');
                }
            }

            return sb.ToString();
        }

        private static string Escape(string text) => text.Contains(' ') ? $"\"{text}\"" : text;
    }
}