// ReSharper disable InconsistentNaming
// ReSharper disable UnusedType.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable StringLiteralTypo
// ReSharper disable InvertIf
// ReSharper disable UseDeconstructionOnParameter
namespace TeamCity.CSharpInteractive
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Cmd;
    using Contracts;
    using Dotnet;
    using JetBrains.TeamCity.ServiceMessages.Read;

    internal class BuildService: IBuild
    {
        private readonly IProcessRunner _processRunner;
        private readonly IHost _host;
        private readonly ITeamCityContext _teamCityContext;
        private readonly Func<IBuildResult> _resultFactory;
        private readonly ITeamCitySettings _teamCitySettings;
        private readonly IProcessOutputWriter _processOutputWriter;
        private readonly IBuildMessageLogWriter _buildMessageLogWriter;
        private readonly IServiceMessageParser _serviceMessageParser;
        private readonly Func<IProcessMonitor> _monitorFactory;

        public BuildService(
            IProcessRunner processRunner,
            IHost host,
            ITeamCityContext teamCityContext,
            Func<IBuildResult> resultFactory,
            ITeamCitySettings teamCitySettings,
            IProcessOutputWriter processOutputWriter,
            IBuildMessageLogWriter buildMessageLogWriter,
            IServiceMessageParser serviceMessageParser,
            Func<IProcessMonitor> monitorFactory)
        {
            _processRunner = processRunner;
            _host = host;
            _teamCityContext = teamCityContext;
            _resultFactory = resultFactory;
            _teamCitySettings = teamCitySettings;
            _processOutputWriter = processOutputWriter;
            _buildMessageLogWriter = buildMessageLogWriter;
            _serviceMessageParser = serviceMessageParser;
            _monitorFactory = monitorFactory;
        }

        public Dotnet.BuildResult Run(IProcess process, Action<Output>? handler = default, TimeSpan timeout = default)
        {
            var ctx = _resultFactory();
            var exitCode = _processRunner.Run(CreateStartInfo(process), output => Handle(output.StartInfo, handler, output, ctx), process as IProcessStateProvider, _monitorFactory(), timeout);
            return ctx.CreateResult(exitCode);
        }

        public async Task<Dotnet.BuildResult> RunAsync(IProcess process, Action<Output>? handler = default, CancellationToken cancellationToken = default)
        {
            var ctx = _resultFactory();
            var exitCode = await _processRunner.RunAsync(CreateStartInfo(process), output => Handle(output.StartInfo, handler, output, ctx), process as IProcessStateProvider, _monitorFactory(), cancellationToken);
            return ctx.CreateResult(exitCode);
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
        
        private void Handle(IStartInfo info, Action<Output>? handler, in Output output, IBuildResult result)
        {
            var startInfo = output.StartInfo;
            var messages = _serviceMessageParser.ParseServiceMessages(output.Line)
                .SelectMany(message => Enumerable.Repeat(new BuildMessage(BuildMessageState.ServiceMessage, message), 1).Concat(result.ProcessMessage(startInfo, message)))
                .DefaultIfEmpty(new BuildMessage(output.IsError ? BuildMessageState.Error : BuildMessageState.Info, default, output.Line));

            if (handler != default)
            {
                foreach (var buildMessage in messages)
                {
                    handler(new Output(info, buildMessage.State > BuildMessageState.Warning, buildMessage.Text));
                }
            }
            else
            {
                var curMessages = messages.ToArray();
                if (_teamCitySettings.IsUnderTeamCity && curMessages.Any(i => i.State == BuildMessageState.ServiceMessage))
                {
                    _processOutputWriter.Write(output);
                }
                else
                {
                    foreach (var buildMessage in curMessages)
                    {
                        _buildMessageLogWriter.Write(buildMessage);
                    }
                }
            }
        }
    }
}