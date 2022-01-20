namespace TeamCity.CSharpInteractive.Tests;

public class ScriptRunnerTests
{
    private readonly Mock<ICommandSource> _commandSource;
    private readonly Mock<ICommandsRunner> _commandsRunner;
    private readonly Mock<IStatistics> _statistics;
    private readonly Mock<IPresenter<Summary>> _summaryPresenter;
    private readonly Mock<ILog<ScriptRunner>> _log;

    public ScriptRunnerTests()
    {
        _log = new Mock<ILog<ScriptRunner>>();
        var commands = Mock.Of<IEnumerable<ICommand>>();
        _commandSource = new Mock<ICommandSource>();
        _commandSource.Setup(i => i.GetCommands()).Returns(commands);
        _commandsRunner = new Mock<ICommandsRunner>();
        _statistics = new Mock<IStatistics>();
        _summaryPresenter = new Mock<IPresenter<Summary>>();
    }
        
    [Theory]
    [MemberData(nameof(Data))]
    internal void ShouldRun(CommandResult[] results, string[] errors, string[] warnings, int expectedExitCode)
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
            Array.Empty<string>(),
            Array.Empty<string>(),
            0
        },
        
        // Success
        new object[]
        {
            new CommandResult[] { new(new CodeCommand(), true), new(new CodeCommand(), true), new(new ScriptCommand(string.Empty, string.Empty), true, 33)},
            Array.Empty<string>(),
            Array.Empty<string>(),
            33
        },
            
        new object[]
        {
            new CommandResult[] { new(new CodeCommand(), default), new(new CodeCommand(), default), new(new ScriptCommand(string.Empty, string.Empty), default)},
            Array.Empty<string>(),
            Array.Empty<string>(),
            0
        },
            
        // Warnings
        new object[]
        {
            new CommandResult[] { new(new CodeCommand(), default), new(new CodeCommand(), default), new(new ScriptCommand(string.Empty, string.Empty), true)},
            Array.Empty<string>(),
            new[] {"warn"},
            0
        },
            
        // Errors
        new object[]
        {
            new CommandResult[] { new(new CodeCommand(), null), new(new CodeCommand(), null), new(new ScriptCommand(string.Empty, string.Empty), true)},
            new[] {"err"},
            new[] {"warn"},
            1
        },
        
        new object[]
        {
            new CommandResult[] { new(new CodeCommand(), null), new(new CodeCommand(), null), new(new ScriptCommand(string.Empty, string.Empty), true, 44)},
            new[] {"err"},
            new[] {"warn"},
            44
        },
            
        // Failed
        new object[]
        {
            new CommandResult[] { new(new CodeCommand(), null), new(new CodeCommand(), null), new(new ScriptCommand(string.Empty, string.Empty), false)},
            Array.Empty<string>(),
            Array.Empty<string>(),
            1
        },
        
        new object[]
        {
            new CommandResult[] { new(new CodeCommand(), null), new(new CodeCommand(), null), new(new ScriptCommand(string.Empty, string.Empty), false, 55)},
            Array.Empty<string>(),
            Array.Empty<string>(),
            1
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
            _summaryPresenter.Object);
}