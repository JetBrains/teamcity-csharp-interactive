namespace Teamcity.CSharpInteractive.Tests
{
    using System;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;
    using Moq;
    using Shouldly;
    using Xunit;

    public class TeamCityLogTests
    {
        private readonly Mock<ISettings> _settings;
        private readonly Mock<Func<ITeamCityLineAcc>> _teamCityLineAccFactory;
        private readonly Mock<ITeamCityBlockWriter<IDisposable>> _blockWriter;
        private readonly Mock<ITeamCityMessageWriter> _teamCityMessageWriter;
        private readonly Mock<ITeamCityLineAcc> _teamCityLineAcc;
        private readonly Text[] _text = {new("line1"), new("line2")};
        private readonly string[] _lines = {"line1", "line2"};
        private readonly Mock<IStatistics> _statistics;
        private readonly Mock<ITeamCityBuildProblemWriter> _teamCityBuildProblemWriter;

        public TeamCityLogTests()
        {
            _settings = new Mock<ISettings>();
            _teamCityLineAccFactory = new Mock<Func<ITeamCityLineAcc>>();
            _teamCityLineAcc = new Mock<ITeamCityLineAcc>();
            _teamCityLineAcc.Setup(i => i.GetLines(true)).Returns(_lines);
            _teamCityLineAccFactory.Setup(i => i()).Returns(_teamCityLineAcc.Object);
            _blockWriter = new Mock<ITeamCityBlockWriter<IDisposable>>();
            _teamCityMessageWriter = new Mock<ITeamCityMessageWriter>();
            _teamCityBuildProblemWriter = new Mock<ITeamCityBuildProblemWriter>();
            _statistics = new Mock<IStatistics>();
        }

        [Fact]
        public void ShouldSupportBlock()
        {
            // Given
            var log = CreateInstance();
            var blockToken = Mock.Of<IDisposable>();
            _blockWriter.Setup(i => i.OpenBlock("line1line2")).Returns(blockToken);

            // When
            var actualBlockToken = log.Block(_text);

            // Then
            _blockWriter.Verify(i => i.OpenBlock("line1line2"));
            actualBlockToken.ShouldBe(blockToken);
        }
        
        [Theory]
        [InlineData(VerbosityLevel.Normal)]
        [InlineData(VerbosityLevel.Quit)]
        [InlineData(VerbosityLevel.Trace)]
        internal void ShouldSupportError(VerbosityLevel verbosityLevel)
        {
            // Given
            var log = CreateInstance();
            _settings.SetupGet(i => i.VerbosityLevel).Returns(verbosityLevel);
            
            // When
            log.Error(new ErrorId("id"), _text);

            // Then
            _teamCityLineAcc.Verify(i => i.Write(_text));
            _teamCityBuildProblemWriter.Verify(i => i.WriteBuildProblem("id", "line1"));
            _teamCityBuildProblemWriter.Verify(i => i.WriteBuildProblem("id", "line2"));
            _statistics.Verify(i => i.RegisterError("line1line2"));
        }
        
        [Theory]
        [InlineData(VerbosityLevel.Normal)]
        [InlineData(VerbosityLevel.Quit)]
        [InlineData(VerbosityLevel.Trace)]
        internal void ShouldSupportWarning(VerbosityLevel verbosityLevel)
        {
            // Given
            var log = CreateInstance();
            _settings.SetupGet(i => i.VerbosityLevel).Returns(verbosityLevel);
            
            // When
            log.Warning(_text);

            // Then
            _teamCityLineAcc.Verify(i => i.Write(_text));
            _teamCityMessageWriter.Verify(i => i.WriteWarning("line1"));
            _teamCityMessageWriter.Verify(i => i.WriteWarning("line2"));
            _statistics.Verify(i => i.RegisterWarning("line1line2"));
        }
        
        [Theory]
        [InlineData(VerbosityLevel.Normal, true)]
        [InlineData(VerbosityLevel.Quit, false)]
        [InlineData(VerbosityLevel.Trace, true)]
        internal void ShouldSupportInfo(VerbosityLevel verbosityLevel, bool enabled)
        {
            // Given
            var times = Times.Exactly(enabled ? 1 : 0);
            var log = CreateInstance();
            _settings.SetupGet(i => i.VerbosityLevel).Returns(verbosityLevel);
            
            // When
            log.Info(_text);

            // Then
            _teamCityLineAcc.Verify(i => i.Write(_text), times);
            _teamCityMessageWriter.Verify(i => i.WriteMessage("line1"), times);
            _teamCityMessageWriter.Verify(i => i.WriteMessage("line2"), times);
        }
        
        [Theory]
        [InlineData(VerbosityLevel.Normal, false)]
        [InlineData(VerbosityLevel.Quit, false)]
        [InlineData(VerbosityLevel.Trace, true)]
        internal void ShouldSupportTrace(VerbosityLevel verbosityLevel, bool enabled)
        {
            // Given
            var times = Times.Exactly(enabled ? 1 : 0);
            var log = CreateInstance();
            _settings.SetupGet(i => i.VerbosityLevel).Returns(verbosityLevel);
            
            // When
            log.Trace(_text);

            // Then
            _teamCityLineAcc.Verify(i => i.Write(It.IsAny<Text[]>()), times);
            _teamCityMessageWriter.Verify(i => i.WriteMessage("line1"), times);
            _teamCityMessageWriter.Verify(i => i.WriteMessage("line2"), times);
        }

        private TeamCityLog<string> CreateInstance() =>
            new(
                _settings.Object,
                _teamCityLineAccFactory.Object,
                _blockWriter.Object,
                _teamCityMessageWriter.Object,
                _teamCityBuildProblemWriter.Object,
                _statistics.Object);
    }
}