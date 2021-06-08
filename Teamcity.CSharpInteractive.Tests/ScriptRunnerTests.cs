namespace Teamcity.CSharpInteractive.Tests
{
    using System.Collections.Generic;
    using Moq;
    using Shouldly;
    using Xunit;

    public class ScriptRunnerTests
    {
        private readonly Mock<ICommandSource> _commandSource;
        private readonly Mock<ICommandsRunner> _commandsRunner;
        private readonly Mock<IStatistics> _statistics;
        private readonly Mock<IPresenter<IStatistics>> _statisticsPresenter;
        private readonly Mock<ILog<ScriptRunner>> _log;
        private readonly IEnumerable<ICommand> _commands;

        public ScriptRunnerTests()
        {
            _log = new Mock<ILog<ScriptRunner>>();
            _commands = Mock.Of<IEnumerable<ICommand>>();
            _commandSource = new Mock<ICommandSource>();
            _commandSource.Setup(i => i.GetCommands()).Returns(_commands);
            _commandsRunner = new Mock<ICommandsRunner>();
            _statistics = new Mock<IStatistics>();
            _statisticsPresenter = new Mock<IPresenter<IStatistics>>();
        }
        
        [Fact]
        public void ShouldProvideMode()
        {
            // Given
            var runner = CreateInstance();

            // When
            var mode = runner.InteractionMode;
            
            // Then
            mode.ShouldBe(InteractionMode.Script);
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
                new CommandResult[] { new(CodeCommand.Shared, true), new(CodeCommand.Shared, true), new(new ScriptCommand(string.Empty, string.Empty), true)},
                System.Array.Empty<string>(),
                System.Array.Empty<string>(),
                ExitCode.Success
            },
            
            new object[]
            {
                new CommandResult[] { new(CodeCommand.Shared, default), new(CodeCommand.Shared, default), new(new ScriptCommand(string.Empty, string.Empty), default)},
                System.Array.Empty<string>(),
                System.Array.Empty<string>(),
                ExitCode.Success
            },
            
            // Warnings
            new object[]
            {
                new CommandResult[] { new(CodeCommand.Shared, default), new(CodeCommand.Shared, default), new(new ScriptCommand(string.Empty, string.Empty), true)},
                System.Array.Empty<string>(),
                new[] {"warn"},
                ExitCode.Success
            },
            
            // Errors
            new object[]
            {
                new CommandResult[] { new(CodeCommand.Shared, null), new(CodeCommand.Shared, null), new(new ScriptCommand(string.Empty, string.Empty), true)},
                new[] {"err"},
                new[] {"warn"},
                ExitCode.Fail
            },
            
            // Failed
            new object[]
            {
                new CommandResult[] { new(CodeCommand.Shared, null), new(CodeCommand.Shared, null), new(new ScriptCommand(string.Empty, string.Empty), false)},
                System.Array.Empty<string>(),
                System.Array.Empty<string>(),
                ExitCode.Fail
            },
        };

        private ScriptRunner CreateInstance() =>
            new(
                _log.Object,
                _commandSource.Object,
                _commandsRunner.Object,
                _statistics.Object,
                _statisticsPresenter.Object);
    }
}