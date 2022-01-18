// ReSharper disable InconsistentNaming
// ReSharper disable UnusedType.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable StringLiteralTypo
// ReSharper disable InvertIf
// ReSharper disable UseDeconstructionOnParameter
namespace TeamCity.CSharpInteractive
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Cmd;
    using Contracts;
    using Dotnet;
    using Pure.DI;

    internal class BuildService: IBuild
    {
        private readonly IProcessRunner _processRunner;
        private readonly IHost _host;
        private readonly ITeamCityContext _teamCityContext;
        private readonly Func<IBuildResult> _resultFactory;
        private readonly IBuildOutputConverter _buildOutputConverter;
        private readonly Func<IProcessMonitor> _monitorFactory;
        private readonly IBuildMessagesProcessor _defaultBuildMessagesProcessor;
        private readonly IBuildMessagesProcessor _customBuildMessagesProcessor;

        public BuildService(
            IProcessRunner processRunner,
            IHost host,
            ITeamCityContext teamCityContext,
            Func<IBuildResult> resultFactory,
            IBuildOutputConverter buildOutputConverter,
            Func<IProcessMonitor> monitorFactory,
            [Tag("default")] IBuildMessagesProcessor defaultBuildMessagesProcessor,
            [Tag("custom")] IBuildMessagesProcessor customBuildMessagesProcessor)
        {
            _processRunner = processRunner;
            _host = host;
            _teamCityContext = teamCityContext;
            _resultFactory = resultFactory;
            _buildOutputConverter = buildOutputConverter;
            _monitorFactory = monitorFactory;
            _defaultBuildMessagesProcessor = defaultBuildMessagesProcessor;
            _customBuildMessagesProcessor = customBuildMessagesProcessor;
        }

        public Dotnet.BuildResult Run(IProcess process, Action<BuildMessage>? handler = default, TimeSpan timeout = default)
        {
            var buildResult = _resultFactory();
            var startInfo = CreateStartInfo(process);
            var processInfo = new ProcessRun(startInfo, _monitorFactory(), output => Handle(handler, output, buildResult), process as IProcessStateProvider);
            var (processState, exitCode) = _processRunner.Run(processInfo, timeout);
            return buildResult.Create(startInfo, processState, exitCode);
        }

        public async Task<Dotnet.BuildResult> RunAsync(IProcess process, Action<BuildMessage>? handler = default, CancellationToken cancellationToken = default)
        {
            var buildResult = _resultFactory();
            var startInfo = CreateStartInfo(process);
            var processInfo = new ProcessRun(startInfo, _monitorFactory(), output => Handle(handler, output, buildResult), process as IProcessStateProvider);
            var (processState, exitCode) = await _processRunner.RunAsync(processInfo, cancellationToken);
            return buildResult.Create(startInfo, processState, exitCode);
        }

        private IStartInfo CreateStartInfo(IProcess process)
        {
            try
            {
                _teamCityContext.TeamCityIntegration = true;
                return process.GetStartInfo(_host);
            }
            finally
            {
                _teamCityContext.TeamCityIntegration = false;
            }
        }
        
        private void Handle(Action<BuildMessage>? handler, in Output output, IBuildResult result)
        {
            var messages = _buildOutputConverter.Convert(output, result);
            if (handler != default)
            {
                _customBuildMessagesProcessor.ProcessMessages(output, messages, handler);
            }
            else
            {
                _defaultBuildMessagesProcessor.ProcessMessages(output, messages, EmptyHandler);
            }
        }

        private static void EmptyHandler(BuildMessage obj) { }
    }
}