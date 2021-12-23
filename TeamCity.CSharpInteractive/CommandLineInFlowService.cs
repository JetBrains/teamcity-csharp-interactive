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

    internal class CommandLineInFlowService: ICommandLine
    {
        private readonly ILog<CommandLineInFlowService> _log;
        private readonly ICommandLine _baseCommandLine;
        private readonly ITeamCitySettings _teamCitySettings;
        private readonly ITeamCityWriter _teamCityWriter;
        private readonly IFlowContext _flowContext;

        public CommandLineInFlowService(
            ILog<CommandLineInFlowService> log,
            [Tag("base")] ICommandLine baseCommandLine,
            ITeamCitySettings teamCitySettings,
            ITeamCityWriter teamCityWriter,
            IFlowContext flowContext)
        {
            _log = log;
            _baseCommandLine = baseCommandLine;
            _teamCitySettings = teamCitySettings;
            _teamCityWriter = teamCityWriter;
            _flowContext = flowContext;
        }

        public int? Run(IProcess process, Action<Output>? handler = default, TimeSpan timeout = default)
        {
            using var flow = CreateFlow();
            using var block = CreateBlock(process);
            return _baseCommandLine.Run(WrapProcess(process), handler, timeout);
        }

        public Task<int?> RunAsync(IProcess process, Action<Output>? handler = default, CancellationToken cancellationToken = default)
        {
            var flow = CreateFlow();
            var block = CreateBlock(process);
            return _baseCommandLine.RunAsync(WrapProcess(process), handler, cancellationToken)
                .ContinueWith(
                    task =>
                    {
                        block.Dispose();
                        flow.Dispose();
                        return task.Result;
                    }, cancellationToken);
        }

        private IProcess WrapProcess(IProcess process) =>
            _teamCitySettings.IsUnderTeamCity
                ? new ProcessInFlow(process, _flowContext.CurrentFlowId)
                : process;
        
        private IDisposable CreateBlock(IProcess process) =>
            _log.Block(new []{new Text(process.ShortName)});

        private IDisposable CreateFlow() =>
            _teamCitySettings.IsUnderTeamCity ? _teamCityWriter.OpenFlow() : Disposable.Empty;

        private class ProcessInFlow : IProcess
        {
            private readonly IProcess _baseProcess;
            private readonly string _flowId;

            public ProcessInFlow(IProcess baseProcess, string flowId)
            {
                _baseProcess = baseProcess;
                _flowId = flowId;
            }
            
            public string ShortName => _baseProcess.ShortName;

            public IStartInfo GetStartInfo(IHost host) => new StartInfoInFlow(_baseProcess.GetStartInfo(host), _flowId);

            public ProcessState GetState(int exitCode) => _baseProcess.GetState(exitCode);

            public override string ToString() => _baseProcess.ToString() ?? string.Empty;

            private class StartInfoInFlow: IStartInfo
            {
                private readonly IStartInfo _baseStartIfo;
                private readonly string _flowId;

                public StartInfoInFlow(IStartInfo baseStartIfo, string flowId)
                {
                    _baseStartIfo = baseStartIfo;
                    _flowId = flowId;
                }

                public string ExecutablePath => _baseStartIfo.ExecutablePath;

                public string WorkingDirectory => _baseStartIfo.WorkingDirectory;

                public IEnumerable<string> Args => _baseStartIfo.Args;

                public IEnumerable<(string name, string value)> Vars => 
                    new []{ (TeamCitySettings.FlowIdEnvironmentVariableName, _flowId) }
                        .Concat(_baseStartIfo.Vars);
            }
        }
    }
}