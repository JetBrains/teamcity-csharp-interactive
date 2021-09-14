namespace Teamcity.CSharpInteractive.Tests
{
    using Moq;
    using Xunit;

    public class SettingCommandRunnerTests
    {
        [Fact]
        public void ShouldSetValue()
        {
            // Given
            var settingSetter = new Mock<ISettingSetter<VerbosityLevel>>();
            var runner = new SettingCommandRunner<VerbosityLevel>(Mock.Of<ILog<SettingCommandRunner<VerbosityLevel>>>(), settingSetter.Object);

            // When
            runner.TryRun(new SettingCommand<VerbosityLevel>(VerbosityLevel.Trace));

            // Then
            settingSetter.Verify(i => i.SetSetting(VerbosityLevel.Trace));
        }
        
        [Fact]
        public void ShouldNotSetValueWhenOtherCommand()
        {
            // Given
            var settingSetter = new Mock<ISettingSetter<VerbosityLevel>>();
            var runner = new SettingCommandRunner<VerbosityLevel>(Mock.Of<ILog<SettingCommandRunner<VerbosityLevel>>>(), settingSetter.Object);
            
            // When
            runner.TryRun(HelpCommand.Shared);

            // Then
            settingSetter.Verify(i => i.SetSetting(It.IsAny<VerbosityLevel>()), Times.Never);
        }
    }
}