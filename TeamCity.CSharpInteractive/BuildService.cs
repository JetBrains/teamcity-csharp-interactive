// ReSharper disable InconsistentNaming
// ReSharper disable UnusedType.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable StringLiteralTypo
// ReSharper disable InvertIf
// ReSharper disable UseDeconstructionOnParameter
namespace TeamCity.CSharpInteractive;

using Cmd;
using CSharpInteractive;
using DotNet;
using Pure.DI;
using Script;

internal class BuildService: IBuild
{
    private readonly IProcessRunner _processRunner;
    private readonly IHost _host;
    private readonly ITeamCityContext _teamCityContext;
    private readonly Func<IBuildContext> _resultFactory;
    private readonly IBuildOutputProcessor _buildOutputProcessor;
    private readonly Func<IProcessMonitor> _monitorFactory;
    private readonly IBuildMessagesProcessor _defaultBuildMessagesProcessor;
    private readonly IBuildMessagesProcessor _customBuildMessagesProcessor;

    public BuildService(
        IProcessRunner processRunner,
        IHost host,
        ITeamCityContext teamCityContext,
        Func<IBuildContext> resultFactory,
        IBuildOutputProcessor buildOutputProcessor,
        Func<IProcessMonitor> monitorFactory,
        [Tag("default")] IBuildMessagesProcessor defaultBuildMessagesProcessor,
        [Tag("custom")] IBuildMessagesProcessor customBuildMessagesProcessor)
    {
        _processRunner = processRunner;
        _host = host;
        _teamCityContext = teamCityContext;
        _resultFactory = resultFactory;
        _buildOutputProcessor = buildOutputProcessor;
        _monitorFactory = monitorFactory;
        _defaultBuildMessagesProcessor = defaultBuildMessagesProcessor;
        _customBuildMessagesProcessor = customBuildMessagesProcessor;
    }

    public IResult Run(IProcess process, Action<BuildMessage>? handler = default, TimeSpan timeout = default)
    {
        var buildResult = _resultFactory();
        var startInfo = CreateStartInfo(process);
        var processInfo = new ProcessRun(startInfo, _monitorFactory(), output => Handle(handler, output, buildResult));
        var result = _processRunner.Run(processInfo, timeout);
        return buildResult.Create(startInfo, result.ExitCode);
    }

    public async Task<IResult> RunAsync(IProcess process, Action<BuildMessage>? handler = default, CancellationToken cancellationToken = default)
    {
        var buildResult = _resultFactory();
        var startInfo = CreateStartInfo(process);
        var processInfo = new ProcessRun(startInfo, _monitorFactory(), output => Handle(handler, output, buildResult));
        var result = await _processRunner.RunAsync(processInfo, cancellationToken);
        return buildResult.Create(startInfo, result.ExitCode);
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
        
    private void Handle(Action<BuildMessage>? handler, in Output output, IBuildContext context)
    {
        var messages = _buildOutputProcessor.Convert(output, context);
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