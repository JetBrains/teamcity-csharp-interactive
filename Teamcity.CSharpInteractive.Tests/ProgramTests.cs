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
        private readonly Mock<IExitTracker> _exitTracker;
        private readonly Mock<IDisposable> _trackToken;
        private readonly Mock<IRunner> _runner;

        public ProgramTests()
        {
            _info = new Mock<IInfo>();
            _settingsManager = new Mock<ISettingsManager>();
            _trackToken = new Mock<IDisposable>();
            _exitTracker = new Mock<IExitTracker>();
            _exitTracker.Setup(i => i.Track()).Returns(_trackToken.Object);
            _runner = new Mock<IRunner>();
            _runner.Setup(i => i.Run()).Returns(ExitCode.Fail);
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
        }

        private Program CreateInstance() =>
            new(_info.Object, _settingsManager.Object, _exitTracker.Object, () => _runner.Object);
    }
}