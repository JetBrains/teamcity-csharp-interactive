namespace Teamcity.CSharpInteractive.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Moq;
    using Shouldly;
    using Xunit;

    public class CommandsRunnerTests
    {
        private readonly Mock<ICommandRunner> _commandRunner1;
        private readonly Mock<ICommandRunner> _commandRunner2;
        private readonly Mock<IStatistics> _statistics;
        private readonly Mock<IDisposable> _statisticsToken;

        public CommandsRunnerTests()
        {
            _commandRunner1 = new Mock<ICommandRunner>();
            _commandRunner1.Setup(i => i.TryRun(HelpCommand.Shared)).Returns(new CommandResult(HelpCommand.Shared, true));
            _commandRunner1.Setup(i => i.TryRun(ResetCommand.Shared)).Returns(new CommandResult(HelpCommand.Shared, default));
            _commandRunner1.Setup(i => i.TryRun(CodeCommand.Shared)).Returns(new CommandResult(CodeCommand.Shared, default));
            _commandRunner2 = new Mock<ICommandRunner>();
            _commandRunner2.Setup(i => i.TryRun(ResetCommand.Shared)).Returns(new CommandResult(ResetCommand.Shared, false));
            _commandRunner2.Setup(i => i.TryRun(HelpCommand.Shared)).Returns(new CommandResult(HelpCommand.Shared, default));
            _commandRunner2.Setup(i => i.TryRun(CodeCommand.Shared)).Returns(new CommandResult(CodeCommand.Shared, default));
            _statisticsToken = new Mock<IDisposable>();
            _statistics = new Mock<IStatistics>();
            _statistics.Setup(i => i.Start()).Returns(_statisticsToken.Object);
        }

        [Theory]
        [MemberData(nameof(Data))]
        internal void ShouldRunCommands(ICommand[] commands, CommandResult[] expectedResults)
        {
            // Given
            var runner = CreateInstance();

            // When
            var actualResults = runner.Run(commands).ToArray();

            // Then
            actualResults.ShouldBe(expectedResults);
            _statisticsToken.Verify(i => i.Dispose());
        }

        public static IEnumerable<object?[]> Data => new List<object?[]>
        {
            new object[]
            {
                new [] { HelpCommand.Shared },
                new [] { new CommandResult(HelpCommand.Shared, true)}
            },
            new object[]
            {
                new [] { ResetCommand.Shared },
                new [] { new CommandResult(ResetCommand.Shared, false)}
            },
            new object[]
            {
                new [] { CodeCommand.Shared },
                new [] { new CommandResult(CodeCommand.Shared, default)}
            },
            new object[]
            {
                new [] { CodeCommand.Shared, HelpCommand.Shared, ResetCommand.Shared},
                new [] { new CommandResult(CodeCommand.Shared, default), new CommandResult(HelpCommand.Shared, true), new CommandResult(ResetCommand.Shared, false)}
            },
        };

        private CommandsRunner CreateInstance() => 
            new CommandsRunner(new [] {_commandRunner1.Object, _commandRunner2.Object}, _statistics.Object);
    }
}