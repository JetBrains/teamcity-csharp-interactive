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
    using Dotnet;

    internal class BuildService: IBuild
    {
        private readonly ICommandLine _commandLine;
        private readonly Func<IBuildResult> _resultFactory;
        private readonly ITeamCitySettings _teamCitySettings;
        private readonly ICommandLineOutputWriter _commandLineOutputWriter;
        private readonly IBuildMessageLogWriter _buildMessageLogWriter;

        public BuildService(
            ICommandLine commandLine,
            Func<IBuildResult> resultFactory,
            ITeamCitySettings teamCitySettings,
            ICommandLineOutputWriter commandLineOutputWriter,
            IBuildMessageLogWriter buildMessageLogWriter)
        {
            _commandLine = commandLine;
            _resultFactory = resultFactory;
            _teamCitySettings = teamCitySettings;
            _commandLineOutputWriter = commandLineOutputWriter;
            _buildMessageLogWriter = buildMessageLogWriter;
        }

        public Dotnet.BuildResult Run(in CommandLine commandLine, Action<CommandLineOutput>? handler = default, TimeSpan timeout = default)
        {
            var ctx = _resultFactory();
            var curCommandLine = commandLine;
            var exitCode = _commandLine.Run(commandLine, output => Handler(curCommandLine, handler, output, ctx), timeout);
            return ctx.CreateResult(exitCode);
        }

        public async Task<Dotnet.BuildResult> RunAsync(CommandLine commandLine, Action<CommandLineOutput>? handler = default, CancellationToken cancellationToken = default)
        {
            var ctx = _resultFactory();
            var exitCode = await _commandLine.RunAsync(commandLine, output => Handler(commandLine, handler, output, ctx), cancellationToken);
            return ctx.CreateResult(exitCode);
        }

        private void Handler(in CommandLine commandLine, Action<CommandLineOutput>? handler, in CommandLineOutput output, IBuildResult result)
        {
            var messages = result.ProcessOutput(output);
            if (handler != default)
            {
                foreach (var buildMessage in messages)
                {
                    handler(new CommandLineOutput(commandLine, buildMessage.State > BuildMessageState.Warning, buildMessage.Text));
                }
            }
            else
            {
                if (_teamCitySettings.IsUnderTeamCity)
                {
                    _commandLineOutputWriter.Write(output);
                }
                else
                {
                    foreach (var buildMessage in messages)
                    {
                        _buildMessageLogWriter.Write(buildMessage);
                    }
                }
            }
        }
    }
}