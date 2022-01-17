namespace TeamCity.CSharpInteractive.Tests;

using System;
using System.Threading;
using System.Threading.Tasks;
using Cmd;
using Contracts;
using Dotnet;
using Moq;
using Xunit;

public class BuildServiceTests
{
    private readonly Mock<IProcessRunner> _processRunner = new();
    private readonly Mock<IHost> _host = new();
    private readonly Mock<ITeamCityContext> _teamCityContext = new();
    private readonly Mock<IBuildResult> _buildResult = new();
    private readonly Func<IBuildResult> _resultFactory;
    private readonly Mock<IBuildMessagesProcessor> _defaultBuildMessagesProcessor = new();
    private readonly Mock<IBuildMessagesProcessor> _customBuildMessagesProcessor = new();
    private readonly Mock<IBuildOutputConverter> _buildOutputConverter = new();
    private readonly Mock<IProcessMonitor> _processMonitor = new();
    private readonly Func<IProcessMonitor> _monitorFactory;
    private readonly Mock<IStartInfo> _startInfo = new();
    private readonly Mock<IProcess> _process = new();
    private static readonly ProcessResult ProcessResult = new(ProcessState.Succeeded, 33);

    public BuildServiceTests()
    {
        var buildResult = new BuildResult(BuildState.Succeeded).AddCommandLines(new CommandLineResult(_startInfo.Object, 33));
        _process.Setup(i => i.GetStartInfo(_host.Object)).Returns(_startInfo.Object);
        _resultFactory = () => _buildResult.Object;
        _monitorFactory = () => _processMonitor.Object;
        _buildResult.Setup(i => i.Create(_startInfo.Object, ProcessState.Succeeded, 33)).Returns(buildResult);
    }

    [Fact]
    public void ShouldRunBuildWhenHasHandler()
    {
        // Given
        var stateProvider = _process.As<IProcessStateProvider>();
        stateProvider.Setup(i => i.GetState(It.IsAny<int>())).Returns(ProcessState.Succeeded);
        var buildMessages = new BuildMessage[]
        {
            new(),
            new()
        };

        var output = new Output(_startInfo.Object, true, "Msg1", 11);
        _buildOutputConverter.Setup(i => i.Convert(output, _buildResult.Object)).Returns(buildMessages);
        
        var buildService = CreateInstance();
        _processRunner.Setup(i => i.Run(_startInfo.Object, It.IsAny<Action<Output>?>(), stateProvider.Object, It.IsAny<IProcessMonitor>(), TimeSpan.FromSeconds(1)))
            .Callback<IStartInfo, Action<Output>?, IProcessStateProvider?, IProcessMonitor, TimeSpan>((_, handler, _, _, _) => handler!(output))
            .Returns(ProcessResult);
        
        var customHandler = Mock.Of<Action<BuildMessage>>();

        // When
        buildService.Run(_process.Object, customHandler, TimeSpan.FromSeconds(1));

        // Then
        _customBuildMessagesProcessor.Verify(i => i.ProcessMessages(output, buildMessages, customHandler));
        _teamCityContext.VerifySet(i => i.TeamCityIntegration = true);
        _teamCityContext.VerifySet(i => i.TeamCityIntegration = false);
    }
    
    [Fact]
    public void ShouldRunBuildWhenHasNoHandler()
    {
        // Given
        var stateProvider = _process.As<IProcessStateProvider>();
        stateProvider.Setup(i => i.GetState(It.IsAny<int>())).Returns(ProcessState.Succeeded);
        var buildMessages = new BuildMessage[]
        {
            new(),
            new()
        };

        var output = new Output(_startInfo.Object, true, "Msg1", 11);
        _buildOutputConverter.Setup(i => i.Convert(output, _buildResult.Object)).Returns(buildMessages);
        
        var buildService = CreateInstance();
        _processRunner.Setup(i => i.Run(_startInfo.Object, It.IsAny<Action<Output>?>(), stateProvider.Object, It.IsAny<IProcessMonitor>(), TimeSpan.FromSeconds(1)))
            .Callback<IStartInfo, Action<Output>?, IProcessStateProvider?, IProcessMonitor, TimeSpan>((_, handler, _, _, _) => handler!(output))
            .Returns(ProcessResult);
        
        // When
        buildService.Run(_process.Object, default, TimeSpan.FromSeconds(1));

        // Then
        _defaultBuildMessagesProcessor.Verify(i => i.ProcessMessages(output, buildMessages, It.IsAny<Action<BuildMessage>>()));
        _teamCityContext.VerifySet(i => i.TeamCityIntegration = true);
        _teamCityContext.VerifySet(i => i.TeamCityIntegration = false);
    }

    [Fact]
    public async Task ShouldRunBuildAsync()
    {
        // Given
        using var cancellationTokenSource = new CancellationTokenSource();
        var token = cancellationTokenSource.Token;
        var stateProvider = _process.As<IProcessStateProvider>();
        var buildService = CreateInstance();
        _processRunner.Setup(i => i.RunAsync(_startInfo.Object, It.IsAny<Action<Output>?>(), stateProvider.Object, It.IsAny<IProcessMonitor>(), token)).Returns(Task.FromResult(ProcessResult));

        // When
        await buildService.RunAsync(_process.Object, Mock.Of<Action<BuildMessage>?>(), token);

        // Then
        _teamCityContext.VerifySet(i => i.TeamCityIntegration = true);
        _teamCityContext.VerifySet(i => i.TeamCityIntegration = false);
    }
    
    private BuildService CreateInstance() =>
        new(
            _processRunner.Object,
            _host.Object,
            _teamCityContext.Object,
            _resultFactory,
            _buildOutputConverter.Object,
            _monitorFactory,
            _defaultBuildMessagesProcessor.Object,
            _customBuildMessagesProcessor.Object);
}