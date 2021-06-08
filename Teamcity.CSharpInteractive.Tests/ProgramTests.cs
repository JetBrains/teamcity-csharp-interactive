namespace Teamcity.CSharpInteractive.Tests
{
    using System;
    using System.Collections.Generic;
    using Moq;
    using Shouldly;
    using Xunit;

    public class ProgramTests
    {
        private readonly Mock<IInfo> _info;
        private readonly Mock<ISettings> _settings;
        private readonly List<IRunner> _runners;
        private readonly Mock<IExitTracker> _exitTracker;
        private readonly Mock<IDisposable> _trackToken;

        public ProgramTests()
        {
            _info = new Mock<IInfo>();
            _settings = new Mock<ISettings>();
            _trackToken = new Mock<IDisposable>();
            _exitTracker = new Mock<IExitTracker>();
            _exitTracker.Setup(i => i.Track()).Returns(_trackToken.Object);
            _settings.SetupGet(i => i.InteractionMode).Returns(InteractionMode.Script);
            Mock<IRunner> interactiveRunner = new Mock<IRunner>();
            interactiveRunner.SetupGet(i => i.InteractionMode).Returns(InteractionMode.Interactive);
            Mock<IRunner> scriptRunner = new Mock<IRunner>();
            scriptRunner.SetupGet(i => i.InteractionMode).Returns(InteractionMode.Script);
            scriptRunner.Setup(i => i.Run()).Returns(ExitCode.Fail);
            _runners = new List<IRunner> { interactiveRunner.Object, scriptRunner.Object};
        }

        [Fact]
        public void ShouldRun()
        {
            // Given
            var program = CreateInstance();

            // When
            var actualResult = program.Run();

            // Then
            _settings.Verify(i => i.Load());
            _info.Verify(i => i.ShowHeader());
            actualResult.ShouldBe((int)ExitCode.Fail);
            _trackToken.Verify(i => i.Dispose());
        }

        private Program CreateInstance() =>
            new Program(_info.Object, _settings.Object, _exitTracker.Object, _runners);
    }
}