namespace TeamCity.CSharpInteractive.Tests;

using HostApi;
using JetBrains.TeamCity.ServiceMessages;

public class BuildRunnerTests
{
    private readonly Mock<IProcessRunner> _processRunner = new();
    private readonly Mock<IHost> _host = new();
    private readonly Mock<ITeamCityContext> _teamCityContext = new();
    private readonly Mock<IBuildContext> _buildResult = new();
    private readonly Func<IBuildContext> _resultFactory;
    private readonly Mock<IBuildMessagesProcessor> _defaultBuildMessagesProcessor = new();
    private readonly Mock<IBuildMessagesProcessor> _customBuildMessagesProcessor = new();
    private readonly Mock<IBuildOutputProcessor> _buildOutputConverter = new();
    private readonly Mock<IProcessMonitor> _processMonitor = new();
    private readonly Func<IProcessMonitor> _monitorFactory;
    private readonly Mock<IStartInfo> _startInfo = new();
    private readonly Mock<ICommandLine> _process = new();
    private readonly Mock<IProcessResultHandler> _processResultHandler = new();
    private readonly ProcessResult _processResult;

    public BuildRunnerTests()
    {
        _processResult = ProcessResult.RanToCompletion(_startInfo.Object, 99, 12, 33);
        var buildResult = new BuildResult(_startInfo.Object).WithExitCode(33);
        _process.Setup(i => i.GetStartInfo(_host.Object)).Returns(_startInfo.Object);
        _resultFactory = () => _buildResult.Object;
        _monitorFactory = () => _processMonitor.Object;
        _buildResult.Setup(i => i.Create(_startInfo.Object, 33)).Returns(buildResult);
    }

    [Fact]
    public void ShouldRunBuildWhenHasHandler()
    {
        // Given
        var buildMessages = new BuildMessage[]
        {
            new(),
            new()
        };

        var output = new Output(_startInfo.Object, true, "Msg1", 11);
        _buildOutputConverter.Setup(i => i.Convert(output, _buildResult.Object)).Returns(buildMessages);

        var buildService = CreateInstance();
        _processRunner.Setup(i => i.Run(It.IsAny<ProcessInfo>(), TimeSpan.FromSeconds(1)))
            .Callback<ProcessInfo, TimeSpan>((processRun, _) => processRun.Handler!(output))
            .Returns(_processResult);

        var customHandler = Mock.Of<Action<BuildMessage>>();

        // When
        buildService.Run(_process.Object, customHandler, TimeSpan.FromSeconds(1));

        // Then
        _customBuildMessagesProcessor.Verify(i => i.ProcessMessages(output, buildMessages, customHandler));
        _teamCityContext.VerifySet(i => i.TeamCityIntegration = true);
        _teamCityContext.VerifySet(i => i.TeamCityIntegration = false);
        _processResultHandler.Verify(i => i.Handle(_processResult, customHandler));
    }

    [Fact]
    public void ShouldRunBuildWhenHasNoHandler()
    {
        // Given
        var buildMessages = new BuildMessage[]
        {
            new(),
            new()
        };

        var output = new Output(_startInfo.Object, true, "Msg1", 11);
        _buildOutputConverter.Setup(i => i.Convert(output, _buildResult.Object)).Returns(buildMessages);

        var buildService = CreateInstance();
        _processRunner.Setup(i => i.Run(It.IsAny<ProcessInfo>(), TimeSpan.FromSeconds(1)))
            .Callback<ProcessInfo, TimeSpan>((processRun, _) => processRun.Handler!(output))
            .Returns(_processResult);

        // When
        buildService.Run(_process.Object, default, TimeSpan.FromSeconds(1));

        // Then
        _defaultBuildMessagesProcessor.Verify(i => i.ProcessMessages(output, buildMessages, It.IsAny<Action<BuildMessage>>()));
        _teamCityContext.VerifySet(i => i.TeamCityIntegration = true);
        _teamCityContext.VerifySet(i => i.TeamCityIntegration = false);
        _processResultHandler.Verify(i => i.Handle(_processResult, default(Action<BuildMessage>)));
    }

    [Fact]
    public void ShouldInvokePreRunMethodBeforeRunningCommand()
    {
        // Given
        var commandLineMock = new Mock<ICommandLine>(MockBehavior.Strict);
        var sequence = new MockSequence();
        commandLineMock.InSequence(sequence).Setup(x => x.PreRun(It.IsAny<IHost>()));
        commandLineMock.InSequence(sequence).Setup(x => x.GetStartInfo(It.IsAny<IHost>())).Returns(_startInfo.Object);
        commandLineMock.Setup(x => x.GetNonStdOutServiceMessages(It.IsAny<IHost>())).Returns(Enumerable.Empty<IServiceMessage>());

        _processRunner.Setup(i => i.Run(It.IsAny<ProcessInfo>(), It.IsAny<TimeSpan>())).Returns(_processResult);

        var buildService = CreateInstance();

        // When
        buildService.Run(commandLineMock.Object);

        // Then
        commandLineMock.Verify(x => x.PreRun(It.IsAny<IHost>()));
        commandLineMock.Verify(x => x.GetStartInfo(It.IsAny<IHost>()));
    }

    [Fact]
    public void ShouldProcessNonStdOutServiceMessagesAfterRunningCommand()
    {
        // Given
        var commandLineMock = new Mock<ICommandLine>(MockBehavior.Strict);
        var sequence = new MockSequence();
        commandLineMock.Setup(x => x.PreRun(It.IsAny<IHost>()));
        commandLineMock.InSequence(sequence).Setup(x => x.GetStartInfo(It.IsAny<IHost>())).Returns(_startInfo.Object);
        commandLineMock.InSequence(sequence).Setup(x => x.GetNonStdOutServiceMessages(It.IsAny<IHost>())).Returns(Enumerable.Empty<IServiceMessage>());

        _processRunner.Setup(i => i.Run(It.IsAny<ProcessInfo>(), It.IsAny<TimeSpan>())).Returns(_processResult);

        var buildService = CreateInstance();

        // When
        buildService.Run(commandLineMock.Object);

        // Then
        commandLineMock.Verify(x => x.GetStartInfo(It.IsAny<IHost>()));
        commandLineMock.Verify(x => x.GetNonStdOutServiceMessages(It.IsAny<IHost>()));
    }

    [Fact]
    public async Task ShouldRunBuildAsync()
    {
        // Given
        using var cancellationTokenSource = new CancellationTokenSource();
        var token = cancellationTokenSource.Token;
        var buildService = CreateInstance();
        _processRunner.Setup(i => i.RunAsync(It.IsAny<ProcessInfo>(), token)).Returns(Task.FromResult(_processResult));
        var handler = Mock.Of<Action<BuildMessage>?>();

        // When
        await buildService.RunAsync(_process.Object, handler, token);

        // Then
        _teamCityContext.VerifySet(i => i.TeamCityIntegration = true);
        _teamCityContext.VerifySet(i => i.TeamCityIntegration = false);
        _processResultHandler.Verify(i => i.Handle(_processResult, handler));
    }

    [Fact]
    public async Task ShouldInvokePreRunMethodBeforeRunningCommandAsync()
    {
        // Given
        var commandLineMock = new Mock<ICommandLine>(MockBehavior.Strict);
        var sequence = new MockSequence();
        commandLineMock.InSequence(sequence).Setup(x => x.PreRun(It.IsAny<IHost>()));
        commandLineMock.InSequence(sequence).Setup(x => x.GetStartInfo(It.IsAny<IHost>())).Returns(_startInfo.Object);
        commandLineMock.Setup(x => x.GetNonStdOutServiceMessages(It.IsAny<IHost>())).Returns(Enumerable.Empty<IServiceMessage>());

        _processRunner.Setup(i => i.RunAsync(It.IsAny<ProcessInfo>(), It.IsAny<CancellationToken>())).ReturnsAsync(_processResult);

        var buildService = CreateInstance();

        // When
        await buildService.RunAsync(commandLineMock.Object);

        // Then
        commandLineMock.Verify(x => x.PreRun(It.IsAny<IHost>()));
        commandLineMock.Verify(x => x.GetStartInfo(It.IsAny<IHost>()));
    }

    [Fact]
    public async Task ShouldProcessNonStdOutServiceMessagesAfterRunningCommandAsync()
    {
        // Given
        var commandLineMock = new Mock<ICommandLine>(MockBehavior.Strict);
        var sequence = new MockSequence();
        commandLineMock.Setup(x => x.PreRun(It.IsAny<IHost>()));
        commandLineMock.InSequence(sequence).Setup(x => x.GetStartInfo(It.IsAny<IHost>())).Returns(_startInfo.Object);
        commandLineMock.InSequence(sequence).Setup(x => x.GetNonStdOutServiceMessages(It.IsAny<IHost>())).Returns(Enumerable.Empty<IServiceMessage>());

        _processRunner.Setup(i => i.RunAsync(It.IsAny<ProcessInfo>(), It.IsAny<CancellationToken>())).ReturnsAsync(_processResult);

        var buildService = CreateInstance();

        // When
        await buildService.RunAsync(commandLineMock.Object);

        // Then
        commandLineMock.Verify(x => x.GetStartInfo(It.IsAny<IHost>()));
        commandLineMock.Verify(x => x.GetNonStdOutServiceMessages(It.IsAny<IHost>()));
    }

    private BuildRunner CreateInstance() =>
        new(
            _processRunner.Object,
            _host.Object,
            _teamCityContext.Object,
            _resultFactory,
            _buildOutputConverter.Object,
            _monitorFactory,
            _defaultBuildMessagesProcessor.Object,
            _customBuildMessagesProcessor.Object,
            _processResultHandler.Object);
}
