// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable ForCanBeConvertedToForeach
namespace TeamCity.CSharpInteractive
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Cmd;
    using Contracts;

    internal class CommandLineService: ICommandLine
    {
        private readonly ILog<CommandLineService> _log;
        private readonly IHost _host;
        private readonly Func<IProcessManager> _processManagerFactory;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public CommandLineService(
            ILog<CommandLineService> log,
            IHost host,
            Func<IProcessManager> processManagerFactory,
            CancellationTokenSource cancellationTokenSource)
        {
            _log = log;
            _host = host;
            _processManagerFactory = processManagerFactory;
            _cancellationTokenSource = cancellationTokenSource;
        }

        public int? Run(IProcess process, Action<Output>? handler = default, TimeSpan timeout = default)
        {
            using var processManager = _processManagerFactory();
            if (handler != default)
            {
                processManager.OnOutput += handler;
            }

            var processInfo = process.GetStartInfo(_host.Host);
            var info = new Text(GetInfo(processInfo, processManager), Color.Header);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            if (!processManager.Start(processInfo, out var startInfo))
            {
                _log.Trace(() => new []{info, new Text(" - cannot start.")});
                return default;
            }

            _log.Info(GetHeader(startInfo, processManager).ToArray());
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
                stopwatch.Start();
                ShowFooter(process, processManager, stopwatch);
                return processManager.ExitCode;
            }

            _log.Warning(info, new Text(" - timeout is expired."));
            Kill(processManager, info);

            return default;
        }

        public async Task<int?> RunAsync(IProcess process, Action<Output>? handler = default, CancellationToken cancellationToken = default)
        {
            if (cancellationToken == default)
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
            var processInfo = process.GetStartInfo(_host.Host);
            var info = new Text(GetInfo(processInfo, processManager), Color.Header);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            if (!processManager.Start(processInfo, out var startInfo))
            {
                _log.Warning(info, new Text(" - cannot start process."));
                processManager.Dispose();
                return default;
            }
            
            _log.Info(GetHeader(startInfo, processManager).ToArray());
            void Cancel()
            {
                if (Kill(processManager, info))
                {
                    completionSource.TrySetCanceled(cancellationToken);
                }
                
                processManager.Dispose();
                _log.Trace(() => new []{info, new Text(" - canceled.")});
            }

            await using (cancellationToken.Register(Cancel, false))
            {
                using (processManager)
                {
                    var exitCode = await completionSource.Task.ConfigureAwait(false);
                    stopwatch.Start();
                    ShowFooter(process, processManager, stopwatch);
                    return exitCode;
                }
            }
        }
        
        private bool Kill(IProcessManager processManager, Text info)
        {
            try
            {
                _log.Trace(() => new []{info, new Text(" - try to kill process.")});
                processManager.Kill();
                _log.Trace(() => new []{info, new Text(" - killed.")});
            }
            catch (Exception ex)
            {
                _log.Warning(info, new Text($" - failed to kill: {ex.Message}."));
                return false;
            }

            return true;
        }
        
        private static string GetInfo(IStartInfo info, IProcessManager processManager)
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(info.WorkingDirectory))
            {
                sb.Append(info.WorkingDirectory);
                sb.Append(" => ");
            }

            sb.Append($"process {processManager.Id} [");
            sb.Append(Escape(info.ExecutablePath));
            foreach (var arg in info.Args)
            {
                sb.Append(' ');
                sb.Append(Escape(arg));
            }
            sb.Append(']');

            // ReSharper disable once InvertIf
            if (info.Vars.Any())
            {
                sb.Append(" with environment variables ");
                foreach (var (name, value) in info.Vars)
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

        private static string Escape(string text) => !text.TrimStart().StartsWith("\"") && text.Contains(' ') ? $"\"{text}\"" : text;
        
        private static IEnumerable<Text> GetHeader(ProcessStartInfo startInfo, IProcessManager processManager)
        {
            yield return new Text($"Starting process {processManager.Id}: ", Color.Header);
            yield return new Text(Escape(startInfo.FileName), Color.Header);
            foreach (var arg in startInfo.ArgumentList)
            {
                yield return new Text(" ");
                yield return new Text(Escape(arg));
            }
            
            yield return Text.NewLine;
            yield return new Text("in directory: ");
            yield return new Text(Escape(startInfo.WorkingDirectory));
        }

        private void ShowFooter(IProcess process, IProcessManager processManager, Stopwatch stopwatch)
        {
            var state = process.GetState(processManager.ExitCode);
            var footer = GetFooter(processManager, stopwatch, state).ToArray();
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (state)
            {
                case ProcessState.Success:
                    _log.Info(footer.WithDefaultColor(Color.Success));
                    break;

                case ProcessState.Fail:
                    _log.Error(ErrorId.Process, footer);
                    break;

                default:
                    _log.Info(footer.WithDefaultColor(Color.Highlighted));
                    break;
            }
        }

        private static IEnumerable<Text> GetFooter(IProcessManager processManager, Stopwatch stopwatch, ProcessState state)
        {
            var stateText = state switch
            {
                ProcessState.Success => "finished successfully",
                ProcessState.Fail => "failed",
                _ => "finished"
            };

            yield return new Text($"Process {processManager.Id} ");
            yield return new Text(stateText);
            yield return new Text($" (in {stopwatch.ElapsedMilliseconds} ms) with exit code {processManager.ExitCode}.");
        }
    }
}