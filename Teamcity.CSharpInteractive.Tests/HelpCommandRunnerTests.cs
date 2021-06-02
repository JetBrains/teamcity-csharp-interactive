namespace Teamcity.CSharpInteractive.Tests
{
    using Moq;
    using Shouldly;
    using Xunit;

    public class HelpCommandRunnerTests
    {
        [Fact]
        public void ShouldShowHelp()
        {
            // Given
            var info = new Mock<IInfo>();
            var runner = new HelpCommandRunner(info.Object);

            // When
            var actualResult = runner.TryRun(new HelpCommand());

            // Then
            actualResult.Success.ShouldBe(true);
            info.Verify(i => i.ShowReplHelp());
        }
        
        [Fact]
        public void ShouldNotShowHelpWhenOtherCommand()
        {
            // Given
            var info = new Mock<IInfo>();
            var runner = new HelpCommandRunner(info.Object);

            // When
            var actualResult = runner.TryRun(Mock.Of<ICommand>());

            // Then
            actualResult.Success.ShouldBe(null);
            info.Verify(i => i.ShowReplHelp(), Times.Never);
        }
    }
}