namespace Teamcity.CSharpInteractive.Tests
{
    using Moq;
    using Xunit;

    public class SetVerbosityLevelCommandRunnerTests
    {
        [Fact]
        public void ShouldSetVerbosityLevel()
        {
            // Given
            var settings = new Mock<ISettings>();
            var runner = new SetVerbosityLevelCommandRunner(Mock.Of<ILog<SetVerbosityLevelCommandRunner>>(), settings.Object);
            
            // When
            runner.TryRun(new SetVerbosityLevelCommand(VerbosityLevel.Trace));

            // Then
            settings.VerifySet(i => i.VerbosityLevel = VerbosityLevel.Trace);
        }
        
        [Fact]
        public void ShouldNotSetVerbosityLevelWhenOtherCommand()
        {
            // Given
            var settings = new Mock<ISettings>();
            var runner = new SetVerbosityLevelCommandRunner(Mock.Of<ILog<SetVerbosityLevelCommandRunner>>(), settings.Object);
            
            // When
            runner.TryRun(HelpCommand.Shared);

            // Then
            settings.VerifySet(i => i.VerbosityLevel = It.IsAny<VerbosityLevel>(), Times.Never);
        }
    }
}