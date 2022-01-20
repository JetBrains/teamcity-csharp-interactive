namespace TeamCity.CSharpInteractive.Tests;

public class ScriptRunnerTests
{
    private readonly Mock<ICommandSource> _commandSource;
    private readonly Mock<ICommandsRunner> _commandsRunner;
    private readonly Mock<IStatistics> _statistics;
    private readonly Mock<IPresenter<IStatistics>> _statisticsPresenter;
    private readonly Mock<ILog<ScriptRunner>> _log;

    public ScriptRunnerTests()
    {
        _log = new Mock<ILog<ScriptRunner>>();
        var commands = Mock.Of<IEnumerable<ICommand>>();
        _commandSource = new Mock<ICommandSource>();
        _commandSource.Setup(i => i.GetCommands()).Returns(commands);
        _commandsRunner = new Mock<ICommandsRunner>();
        _statistics = new Mock<IStatistics>();
        _statisticsPresenter = new Mock<IPresenter<IStatistics>>();
    }
        
    [Theory]
    [MemberData(nameof(Data))]
    internal void ShouldRun(CommandResult[] results, string[] errors, string[] warnings, ExitCode expectedExitCode)
    {
        // Given
        var runner = CreateInstance();
        _commandsRunner.Setup(i => i.Run(It.IsAny<IEnumerable<ICommand>>())).Returns(results);
        _statistics.SetupGet(i => i.Errors).Returns(errors);
        _statistics.SetupGet(i => i.Warnings).Returns(warnings);
            
        // When
        var actualExitCode = runner.Run();

        // Then
        actualExitCode.ShouldBe(expectedExitCode);
    }
        
    public static IEnumerable<object?[]> Data => new List<object?[]> 
    {
        // Success
        new object[]
        {
            new CommandResult[] { new(new CodeCommand(), true), new(new CodeCommand(), true), new(new ScriptCommand(string.Empty, string.Empty), true)},
            System.Array.Empty<string>(),
            System.Array.Empty<string>(),
            ExitCode.Success
        },
            
        new object[]
        {
            new CommandResult[] { new(new CodeCommand(), default), new(new CodeCommand(), default), new(new ScriptCommand(string.Empty, string.Empty), default)},
            System.Array.Empty<string>(),
            System.Array.Empty<string>(),
            ExitCode.Success
        },
            
        // Warnings
        new object[]
        {
            new CommandResult[] { new(new CodeCommand(), default), new(new CodeCommand(), default), new(new ScriptCommand(string.Empty, string.Empty), true)},
            System.Array.Empty<string>(),
            new[] {"warn"},
            ExitCode.Success
        },
            
        // Errors
        new object[]
        {
            new CommandResult[] { new(new CodeCommand(), null), new(new CodeCommand(), null), new(new ScriptCommand(string.Empty, string.Empty), true)},
            new[] {"err"},
            new[] {"warn"},
            ExitCode.Fail
        },
            
        // Failed
        new object[]
        {
            new CommandResult[] { new(new CodeCommand(), null), new(new CodeCommand(), null), new(new ScriptCommand(string.Empty, string.Empty), false)},
            System.Array.Empty<string>(),
            System.Array.Empty<string>(),
            ExitCode.Fail
        }
    };
        
    [Fact]
    public void ShouldShowErrorWhenScriptIsUncompleted()
    {
        // Given
        var runner = CreateInstance();
        //_log.Setup(i => i.Error(ErrorId.UncompletedScript, It.IsAny<string>()));
        _commandSource.Setup(i => i.GetCommands()).Returns(new ICommand[] {new ScriptCommand(string.Empty, string.Empty), new CodeCommand()});
        _statistics.Setup(i => i.Errors).Returns(new List<string>());
        // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
        _commandsRunner.Setup(i => i.Run(It.IsAny<IEnumerable<ICommand>>())).Callback<IEnumerable<ICommand>>(i => i.Count()).Returns(new CommandResult[] { new(new ScriptCommand(string.Empty, string.Empty), true) });
            
        // When
        runner.Run();

        // Then
        _log.Verify(i => i.Error(ErrorId.UncompletedScript, It.IsAny<Text[]>()));
    }

    private ScriptRunner CreateInstance() =>
        new(
            _log.Object,
            _commandSource.Object,
            _commandsRunner.Object,
            _statistics.Object,
            _statisticsPresenter.Object);
}