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
    using Dotnet;
    using JetBrains.TeamCity.ServiceMessages;
    using JetBrains.TeamCity.ServiceMessages.Read;
    using Microsoft.Build.Tasks;

    internal class BuildService: IBuild
    {
        private readonly ICommandLine _commandLine;
        private readonly Func<IBuildResult> _resultFactory;
        private readonly ITeamCitySettings _teamCitySettings;
        private readonly IProcessOutputWriter _processOutputWriter;
        private readonly IBuildMessageLogWriter _buildMessageLogWriter;
        private readonly IServiceMessageParser _serviceMessageParser;

        public BuildService(
            ICommandLine commandLine,
            Func<IBuildResult> resultFactory,
            ITeamCitySettings teamCitySettings,
            IProcessOutputWriter processOutputWriter,
            IBuildMessageLogWriter buildMessageLogWriter,
            IServiceMessageParser serviceMessageParser)
        {
            _commandLine = commandLine;
            _resultFactory = resultFactory;
            _teamCitySettings = teamCitySettings;
            _processOutputWriter = processOutputWriter;
            _buildMessageLogWriter = buildMessageLogWriter;
            _serviceMessageParser = serviceMessageParser;
        }

        public Dotnet.BuildResult Run(IProcess process, Action<Output>? handler = default, TimeSpan timeout = default)
        {
            var ctx = _resultFactory();
            var exitCode = _commandLine.Run(process, output => Handle(output.StartInfo, handler, output, ctx), timeout);
            return ctx.CreateResult(exitCode);
        }

        public async Task<Dotnet.BuildResult> RunAsync(IProcess process, Action<Output>? handler = default, CancellationToken cancellationToken = default)
        {
            var ctx = _resultFactory();
            var exitCode = await _commandLine.RunAsync(process, output => Handle(output.StartInfo, handler, output, ctx), cancellationToken);
            return ctx.CreateResult(exitCode);
        }
        
        private void Handle(IStartInfo info, Action<Output>? handler, in Output output, IBuildResult result)
        {
            var startInfo = output.StartInfo;
            var messages = _serviceMessageParser.ParseServiceMessages(output.Line)
                .SelectMany(message => result.ProcessMessage(startInfo, message))
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