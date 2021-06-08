namespace Teamcity.CSharpInteractive.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using Moq;
    using Shouldly;
    using Xunit;

    public class InteractiveRunnerTests
    {
        private readonly Mock<ICommandSource> _commandSource;
        private readonly Mock<ICommandsRunner> _commandsRunner;
        private readonly Mock<IStdOut> _stdOut;
        private readonly List<string> _out = new();
        private readonly IEnumerable<ICommand> _commands;

        public InteractiveRunnerTests()
        {
            _commands = Mock.Of<IEnumerable<ICommand>>();
            _commandSource = new Mock<ICommandSource>();
            _commandSource.Setup(i => i.GetCommands()).Returns(_commands);
            _commandsRunner = new Mock<ICommandsRunner>();
            _stdOut = new Mock<IStdOut>();
            _stdOut.Setup(i => i.Write(It.IsAny<Text[]>())).Callback<Text[]>(text => _out.Add(string.Join(string.Empty, text.Select(i => i.Value))));
        }

        [Fact]
        public void ShouldProvideMode()
        {
            // Given
            var runner = CreateInstance();

            // When
            var mode = runner.InteractionMode;
            
            // Then
            mode.ShouldBe(InteractionMode.Interactive);
        }

        [Theory]
        [MemberData(nameof(Data))]
        internal void ShouldRun(CommandResult[] results, ExitCode expectedExitCode, string[] expectedOut)
        {
            // Given
            var runner = CreateInstance();
            _commandsRunner.Setup(i => i.Run(_commands)).Returns(results);

            // When
            var actualExitCode = runner.Run();

            // Then
            actualExitCode.ShouldBe(expectedExitCode);
            _out.ToArray().ShouldBe(expectedOut);
        }
        
        public static IEnumerable<object?[]> Data => new List<object?[]> 
        {
            // Success
            new object[]
            {
                new CommandResult[] { new(CodeCommand.Shared, null), new(CodeCommand.Shared, null), new(new ScriptCommand(string.Empty, string.Empty), null)},
                ExitCode.Success,
                new [] {"> ", ". ", ". ", "> "}
            },
        };

        private InteractiveRunner CreateInstance() =>
            new(_commandSource.Object, _commandsRunner.Object, _stdOut.Object);
    }
}