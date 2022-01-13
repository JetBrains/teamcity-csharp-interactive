namespace TeamCity.CSharpInteractive.Tests;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cmd;
using JetBrains.TeamCity.ServiceMessages.Write.Special;
using Moq;
using Shouldly;
using Xunit;

public class ProcessInFlowRunnerTests
{
    private readonly Mock<IProcessRunner> _baseProcessRunner = new();
    private readonly Mock<IStartInfo> _startInfo = new();
    private readonly Mock<ITeamCitySettings> _teamCitySettings = new();
    private readonly Mock<ITeamCityWriter> _teamCityWriter = new();
    private readonly Mock<ITeamCityWriter> _blockWriter = new();
    private readonly Mock<IFlowContext> _flowContext = new();
    private readonly Mock<IProcessStateProvider> _stateProvider = new();
    private readonly Mock<IProcessMonitor> _monitor = new();
    private static readonly (string name, string value)[] InitialVars = { ("Var1", "Val 1") }; 
    private static readonly (string name, string value)[] ModifiedVars = new (string name, string value)[]{ (TeamCitySettings.FlowIdEnvironmentVariableName, "FlowId123") }.Concat(InitialVars).ToArray();
    private static readonly ProcessResult ProcessResult = new(ProcessState.Success, 33);

    public ProcessInFlowRunnerTests()
    {
        _flowContext.SetupGet(i => i.CurrentFlowId).Returns("FlowId123");
        _startInfo.SetupGet(i => i.Vars).Returns(InitialVars);
        _teamCityWriter.Setup(i => i.OpenFlow()).Returns(_blockWriter.Object);
    }

    [Fact]
    public void ShouldRunInFlow()
    {
        // Given
        _teamCitySettings.SetupGet(i => i.IsUnderTeamCity).Returns(true);
        _baseProcessRunner.Setup(i => i.Run(It.Is<IStartInfo>(info => info.Vars.SequenceEqual(ModifiedVars)), Handler, _stateProvider.Object, _monitor.Object, TimeSpan.FromDays(1))).Returns(ProcessResult);
        var runner = CreateInstance();

        // When
        var result = runner.Run(_startInfo.Object, Handler, _stateProvider.Object, _monitor.Object, TimeSpan.FromDays(1));

        // Then
        _teamCityWriter.Verify(i => i.OpenFlow());
        _blockWriter.Verify(i => i.Dispose());
        result.ShouldBe(ProcessResult);
    }
    
    [Fact]
    public void ShouldRunWithoutFlowWhenNotUnderTeamCity()
    {
        // Given
        _teamCitySettings.SetupGet(i => i.IsUnderTeamCity).Returns(false);
        _baseProcessRunner.Setup(i => i.Run(_startInfo.Object, Handler, _stateProvider.Object, _monitor.Object, TimeSpan.FromDays(1))).Returns(ProcessResult);
        var runner = CreateInstance();

        // When
        var result = runner.Run(_startInfo.Object, Handler, _stateProvider.Object, _monitor.Object, TimeSpan.FromDays(1));

        // Then
        _teamCityWriter.Verify(i => i.OpenFlow(), Times.Never);
        _blockWriter.Verify(i => i.Dispose(), Times.Never);
        result.ShouldBe(ProcessResult);
    }
    
    [Fact]
    public async Task ShouldRunAsyncInBlock()
    {
        // Given
        using var tokenSource = new CancellationTokenSource();
        var token = tokenSource.Token;
        _teamCitySettings.SetupGet(i => i.IsUnderTeamCity).Returns(true);
        _baseProcessRunner.Setup(i => i.RunAsync(It.Is<IStartInfo>(info => info.Vars.SequenceEqual(ModifiedVars)), Handler, _stateProvider.Object, _monitor.Object, token)).Returns(Task.FromResult(ProcessResult));
        var runner = CreateInstance();

        // When
        var result = await runner.RunAsync(_startInfo.Object, Handler, _stateProvider.Object, _monitor.Object, token);

        // Then
        _teamCityWriter.Verify(i => i.OpenFlow());
        _blockWriter.Verify(i => i.Dispose());
        result.ShouldBe(ProcessResult);
    }
    
    [Fact]
    public async Task ShouldRunAsyncWithoutFlowWhenNotUnderTeamCity()
    {
        // Given
        using var tokenSource = new CancellationTokenSource();
        var token = tokenSource.Token;
        _teamCitySettings.SetupGet(i => i.IsUnderTeamCity).Returns(false);
        _baseProcessRunner.Setup(i => i.RunAsync(_startInfo.Object, Handler, _stateProvider.Object, _monitor.Object, token)).Returns(Task.FromResult(ProcessResult));
        var runner = CreateInstance();

        // When
        var result = await runner.RunAsync(_startInfo.Object, Handler, _stateProvider.Object, _monitor.Object, token);

        // Then
        _teamCityWriter.Verify(i => i.OpenFlow(), Times.Never);
        _blockWriter.Verify(i => i.Dispose(), Times.Never);
        result.ShouldBe(ProcessResult);
    }

    private static void Handler(Output obj) { }

    private ProcessInFlowRunner CreateInstance() =>
        new(_baseProcessRunner.Object, _teamCitySettings.Object, _teamCityWriter.Object, _flowContext.Object);
}