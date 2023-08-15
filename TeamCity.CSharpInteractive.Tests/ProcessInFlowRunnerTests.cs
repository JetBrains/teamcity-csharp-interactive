namespace TeamCity.CSharpInteractive.Tests;

using HostApi;
using JetBrains.TeamCity.ServiceMessages.Write.Special;

public class ProcessInFlowRunnerTests
{
    private readonly Mock<IProcessRunner> _baseProcessRunner = new();
    private readonly Mock<IStartInfo> _startInfo = new();
    private readonly Mock<ITeamCitySettings> _teamCitySettings = new();
    private readonly Mock<ITeamCityWriter> _teamCityWriter = new();
    private readonly Mock<ITeamCityWriter> _blockWriter = new();
    private readonly Mock<IFlowContext> _flowContext = new();
    private readonly Mock<IProcessMonitor> _monitor = new();
    private readonly ProcessResult _processResult;
    private static readonly (string name, string value)[] InitialVars = {("Var1", "Val 1")};
    private static readonly (string name, string value)[] ModifiedVars = new (string name, string value)[] {(TeamCitySettings.FlowIdEnvironmentVariableName, "FlowId123")}.Concat(InitialVars).ToArray();

    public ProcessInFlowRunnerTests()
    {
        _processResult = new ProcessResult(_startInfo.Object, 0, ProcessState.Finished, 33, Array.Empty<Text>(), 12);
        _flowContext.SetupGet(i => i.CurrentFlowId).Returns("FlowId123");
        _startInfo.SetupGet(i => i.Vars).Returns(InitialVars);
        _teamCityWriter.Setup(i => i.OpenFlow()).Returns(_blockWriter.Object);
    }

    [Fact]
    public void ShouldRunInFlow()
    {
        // Given
        _teamCitySettings.SetupGet(i => i.IsUnderTeamCity).Returns(true);
        _baseProcessRunner.Setup(i => i.Run(It.Is<ProcessInfo>(processRun => processRun.StartInfo.Vars.SequenceEqual(ModifiedVars)), TimeSpan.FromDays(1))).Returns(_processResult);
        var runner = CreateInstance();

        // When
        var result = runner.Run(new ProcessInfo(_startInfo.Object, _monitor.Object, Handler), TimeSpan.FromDays(1));

        // Then
        _teamCityWriter.Verify(i => i.OpenFlow());
        _blockWriter.Verify(i => i.Dispose());
        result.ShouldBe(_processResult);
    }

    [Fact]
    public void ShouldRunWithoutFlowWhenNotUnderTeamCity()
    {
        // Given
        _teamCitySettings.SetupGet(i => i.IsUnderTeamCity).Returns(false);
        _baseProcessRunner.Setup(i => i.Run(new ProcessInfo(_startInfo.Object, _monitor.Object, Handler), TimeSpan.FromDays(1))).Returns(_processResult);
        var runner = CreateInstance();

        // When
        var result = runner.Run(new ProcessInfo(_startInfo.Object, _monitor.Object, Handler), TimeSpan.FromDays(1));

        // Then
        _teamCityWriter.Verify(i => i.OpenFlow(), Times.Never);
        _blockWriter.Verify(i => i.Dispose(), Times.Never);
        result.ShouldBe(_processResult);
    }

    [Fact]
    public async Task ShouldRunAsyncInBlock()
    {
        // Given
        using var tokenSource = new CancellationTokenSource();
        var token = tokenSource.Token;
        _teamCitySettings.SetupGet(i => i.IsUnderTeamCity).Returns(true);
        _baseProcessRunner.Setup(i => i.RunAsync(It.Is<ProcessInfo>(processRun => processRun.StartInfo.Vars.SequenceEqual(ModifiedVars)), token)).Returns(Task.FromResult(_processResult));
        var runner = CreateInstance();

        // When
        var result = await runner.RunAsync(new ProcessInfo(_startInfo.Object, _monitor.Object, Handler), token);

        // Then
        _teamCityWriter.Verify(i => i.OpenFlow());
        _blockWriter.Verify(i => i.Dispose());
        result.ShouldBe(_processResult);
    }

    [Fact]
    public async Task ShouldRunAsyncWithoutFlowWhenNotUnderTeamCity()
    {
        // Given
        using var tokenSource = new CancellationTokenSource();
        var token = tokenSource.Token;
        var processRun = new ProcessInfo(_startInfo.Object, _monitor.Object, Handler);
        _teamCitySettings.SetupGet(i => i.IsUnderTeamCity).Returns(false);
        _baseProcessRunner.Setup(i => i.RunAsync(processRun, token)).Returns(Task.FromResult(_processResult));
        var runner = CreateInstance();

        // When
        var result = await runner.RunAsync(processRun, token);

        // Then
        _teamCityWriter.Verify(i => i.OpenFlow(), Times.Never);
        _blockWriter.Verify(i => i.Dispose(), Times.Never);
        result.ShouldBe(_processResult);
    }

    private static void Handler(Output obj) { }

    private ProcessInFlowRunner CreateInstance() =>
        new(_baseProcessRunner.Object, _teamCitySettings.Object, _teamCityWriter.Object, _flowContext.Object);
}
