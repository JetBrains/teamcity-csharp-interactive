namespace TeamCity.CSharpInteractive.Tests;

public class HelpCommandRunnerTests
{
    [Fact]
    public void ShouldShowHelp()
    {
        // Given
        var info = new Mock<IInfo>();
        var runner = new HelpCommandRunner(info.Object);

        // When
        var actualResult = runner.TryRun(HelpCommand.Shared);

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