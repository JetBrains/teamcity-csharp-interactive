// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Cmd;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;
    using Pure.DI;

    internal class ProcessInBlockRunner: IProcessRunner
    {
        private readonly ILog<ProcessInBlockRunner> _log;
        private readonly IProcessRunner _baseProcessRunner;
        
        public ProcessInBlockRunner(
            ILog<ProcessInBlockRunner> log,
            [Tag("base")] IProcessRunner baseProcessRunner)
        {
            _log = log;
            _baseProcessRunner = baseProcessRunner;
        }
        
        public int? Run(IStartInfo startInfo, Action<Output>? handler, IProcessStateProvider? stateProvider, IProcessMonitor monitor, TimeSpan timeout)
        {
            using var block = CreateBlock(startInfo);
            return _baseProcessRunner.Run(startInfo, handler, stateProvider, monitor, timeout);
        }
        
        public Task<int?> RunAsync(IStartInfo startInfo, Action<Output>? handler, IProcessStateProvider? stateProvider, IProcessMonitor monitor, CancellationToken cancellationToken)
        {
            var block = CreateBlock(startInfo);
            return _baseProcessRunner.RunAsync(startInfo, handler, stateProvider, monitor, cancellationToken)
                .ContinueWith(
                    task =>
                    {
                        block.Dispose();
                        return task.Result;
                    }, cancellationToken);
        }

        private IDisposable CreateBlock(IStartInfo startInfo) =>
            _log.Block(new []{new Text(startInfo.ShortName)});
    }
}