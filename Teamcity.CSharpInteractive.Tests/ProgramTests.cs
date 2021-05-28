namespace Teamcity.CSharpInteractive.Tests
{
    using System.Collections.Generic;
    using Moq;
    using Shouldly;
    using Xunit;

    public class ProgramTests
    {
        private readonly Mock<IInfo> _info;
        private readonly Mock<ISettings> _settings;
        private readonly List<IRunner> _runners;
        private readonly Mock<IRunner> _interactiveRunner;
        private readonly Mock<IRunner> _scriptRunner;

        public ProgramTests()
        {
            _info = new Mock<IInfo>();
            _settings = new Mock<ISettings>();
            _settings.SetupGet(i => i.InteractionMode).Returns(InteractionMode.Script);
            _interactiveRunner = new Mock<IRunner>();
            _interactiveRunner.SetupGet(i => i.InteractionMode).Returns(InteractionMode.Interactive);
            _scriptRunner = new Mock<IRunner>();
            _scriptRunner.SetupGet(i => i.InteractionMode).Returns(InteractionMode.Script);
            _scriptRunner.Setup(i => i.Run()).Returns(ExitCode.Fail);
            _runners = new List<IRunner> { _interactiveRunner.Object, _scriptRunner.Object};
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
        }

        private Program CreateInstance() =>
            new Program(_info.Object, _settings.Object, _runners);
    }
}