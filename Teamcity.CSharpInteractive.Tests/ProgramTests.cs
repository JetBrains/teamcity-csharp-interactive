namespace Teamcity.CSharpInteractive.Tests
{
    using System;
    using Moq;
    using Shouldly;
    using Xunit;

    public class ProgramTests
    {
        private readonly Mock<IInfo> _info;
        private readonly Mock<ISettingsManager> _settingsManager;
        private readonly Mock<ISettings> _settings;
        private readonly Mock<IExitTracker> _exitTracker;
        private readonly Mock<IDisposable> _trackToken;
        private readonly Mock<IRunner> _runner;
        private readonly Mock<IActive> _active;
        private readonly Mock<IDisposable> _activationToken;

        public ProgramTests()
        {
            _info = new Mock<IInfo>();
            _settingsManager = new Mock<ISettingsManager>();
            _settings = new Mock<ISettings>();
            _trackToken = new Mock<IDisposable>();
            _exitTracker = new Mock<IExitTracker>();
            _exitTracker.Setup(i => i.Track()).Returns(_trackToken.Object);
            _runner = new Mock<IRunner>();
            _runner.Setup(i => i.Run()).Returns(ExitCode.Fail);
            _activationToken = new Mock<IDisposable>();
            _active = new Mock<IActive>();
            _active.Setup(i => i.Activate()).Returns(_activationToken.Object);
        }

        [Fact]
        public void ShouldRun()
        {
            // Given
            var program = CreateInstance();

            // When
            var actualResult = program.Run();

            // Then
            _settingsManager.Verify(i => i.Load());
            _info.Verify(i => i.ShowHeader());
            actualResult.ShouldBe((int)ExitCode.Fail);
            _trackToken.Verify(i => i.Dispose());
            _info.Verify(i => i.ShowFooter());
            _active.Verify(i => i.Activate());
            _activationToken.Verify(i => i.Dispose());
        }
        
        [Fact]
        public void ShouldShowVersion()
        {
            // Given
            var program = CreateInstance();

            // When
            _settings.SetupGet(i => i.ShowVersionAndExit).Returns(true);
            var actualResult = program.Run();

            // Then
            _settingsManager.Verify(i => i.Load());
            _info.Verify(i => i.ShowVersion());
            actualResult.ShouldBe((int)ExitCode.Success);
        }
        
        [Fact]
        public void ShouldShowHelp()
        {
            // Given
            var program = CreateInstance();

            // When
            _settings.SetupGet(i => i.ShowHelpAndExit).Returns(true);
            var actualResult = program.Run();

            // Then
            _settingsManager.Verify(i => i.Load());
            _info.Verify(i => i.ShowHeader());
            _info.Verify(i => i.ShowHelp());
            actualResult.ShouldBe((int)ExitCode.Success);
        }

        private Program CreateInstance() =>
            new(new []{_active.Object}, _info.Object, _settingsManager.Object, _settings.Object, _exitTracker.Object, () => _runner.Object);
    }
}