// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Cmd;
    using Contracts;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;
    using Pure.DI;

    internal class CommandLineInFlowService: IProcessRunner, ICommandLine
    {
        private readonly ILog<CommandLineInFlowService> _log;
        private readonly IHost _host;
        private readonly IProcessRunner _baseProcessRunner;
        private readonly ITeamCitySettings _teamCitySettings;
        private readonly ITeamCityWriter _teamCityWriter;
        private readonly IFlowContext _flowContext;

        public CommandLineInFlowService(
            ILog<CommandLineInFlowService> log,
            IHost host,
            [Tag("base")] IProcessRunner baseProcessRunner,
            ITeamCitySettings teamCitySettings,
            ITeamCityWriter teamCityWriter,
            IFlowContext flowContext)
        {
            _log = log;
            _host = host;
            _baseProcessRunner = baseProcessRunner;
            _teamCitySettings = teamCitySettings;
            _teamCityWriter = teamCityWriter;
            _flowContext = flowContext;
        }
        
        public int? Run(IProcess process, Action<Output>? handler = default, TimeSpan timeout = default) =>
            Run(process.GetStartInfo(_host.Host), process, handler, timeout);

        public int? Run(IStartInfo startInfo, IProcessState state, Action<Output>? handler = default, TimeSpan timeout = default)
        {
            using var flow = CreateFlow();
            using var block = CreateBlock(startInfo);
            return _baseProcessRunner.Run(WrapInFlow(startInfo), state, handler, timeout);
        }
        
        public Task<int?> RunAsync(IProcess process, Action<Output>? handler = default, CancellationToken cancellationToken = default) =>
            RunAsync(process.GetStartInfo(_host.Host), process, handler, cancellationToken);
        
        public Task<int?> RunAsync(IStartInfo startInfo, IProcessState state, Action<Output>? handler = default, CancellationToken cancellationToken = default)
        {
            var flow = CreateFlow();
            var block = CreateBlock(startInfo);
            return _baseProcessRunner.RunAsync(WrapInFlow(startInfo), state, handler, cancellationToken)
                .ContinueWith(
                    task =>
                    {
                        block.Dispose();
                        flow.Dispose();
                        return task.Result;
                    }, cancellationToken);
        }

        private IStartInfo WrapInFlow(IStartInfo startInfo) =>
            _teamCitySettings.IsUnderTeamCity
                ? new StartInfoInFlow(startInfo, _flowContext.CurrentFlowId)
                : startInfo;
        
        private IDisposable CreateBlock(IStartInfo startInfo) =>
            _log.Block(new []{new Text(startInfo.ShortName)});

        private IDisposable CreateFlow() =>
            _teamCitySettings.IsUnderTeamCity ? _teamCityWriter.OpenFlow() : Disposable.Empty;

        private class StartInfoInFlow: IStartInfo
        {
            private readonly IStartInfo _baseStartIfo;
            private readonly string _flowId;

            public StartInfoInFlow(IStartInfo baseStartIfo, string flowId)
            {
                _baseStartIfo = baseStartIfo;
                _flowId = flowId;
            }

            public string ShortName => _baseStartIfo.ShortName;

            public string ExecutablePath => _baseStartIfo.ExecutablePath;

            public string WorkingDirectory => _baseStartIfo.WorkingDirectory;

            public IReadOnlyList<string> Args => _baseStartIfo.Args;

            public IReadOnlyList<(string name, string value)> Vars => 
                new []{ (TeamCitySettings.FlowIdEnvironmentVariableName, _flowId) }
                    .Concat(_baseStartIfo.Vars).ToArray();
        }
    }
}