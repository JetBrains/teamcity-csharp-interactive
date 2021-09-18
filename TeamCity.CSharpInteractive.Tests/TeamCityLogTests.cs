namespace TeamCity.CSharpInteractive.Tests
{
    using System;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;
    using Moq;
    using Shouldly;
    using Xunit;

    public class TeamCityLogTests
    {
        private readonly Mock<ISettings> _settings;
        private readonly Mock<ITeamCityLineFormatter> _lineFormatter;
        private readonly Mock<ITeamCityBlockWriter<IDisposable>> _blockWriter;
        private readonly Mock<ITeamCityMessageWriter> _teamCityMessageWriter;
        private readonly Text[] _text = {new("line1"), new("line2")};
        private readonly Mock<IStatistics> _statistics;
        private readonly Mock<ITeamCityBuildProblemWriter> _teamCityBuildProblemWriter;

        public TeamCityLogTests()
        {
            _settings = new Mock<ISettings>();
            _lineFormatter = new Mock<ITeamCityLineFormatter>();
            _lineFormatter.Setup(i => i.Format(It.IsAny<Text[]>())).Returns<Text[]>(i => "F_" + i.ToSimpleString());
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
        [InlineData(VerbosityLevel.Quiet)]
        [InlineData(VerbosityLevel.Trace)]
        internal void ShouldSupportError(VerbosityLevel verbosityLevel)
        {
            // Given
            var log = CreateInstance();
            _settings.SetupGet(i => i.VerbosityLevel).Returns(verbosityLevel);
            
            // When
            log.Error(new ErrorId("id"), _text);

            // Then
            _teamCityBuildProblemWriter.Verify(i => i.WriteBuildProblem("id", "line1line2"));
            _statistics.Verify(i => i.RegisterError("line1line2"));
        }
        
        [Theory]
        [InlineData(VerbosityLevel.Normal)]
        [InlineData(VerbosityLevel.Quiet)]
        [InlineData(VerbosityLevel.Trace)]
        internal void ShouldSupportWarning(VerbosityLevel verbosityLevel)
        {
            // Given
            var log = CreateInstance();
            _settings.SetupGet(i => i.VerbosityLevel).Returns(verbosityLevel);
            
            // When
            log.Warning(_text);

            // Then
            _teamCityMessageWriter.Verify(i => i.WriteWarning("line1line2"));
            _statistics.Verify(i => i.RegisterWarning("line1line2"));
        }
        
        [Theory]
        [InlineData(VerbosityLevel.Normal, true)]
        [InlineData(VerbosityLevel.Quiet, false)]
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
            _teamCityMessageWriter.Verify(i => i.WriteMessage("F_line1line2"), times);
        }
        
        [Theory]
        [InlineData(VerbosityLevel.Normal, false)]
        [InlineData(VerbosityLevel.Quiet, false)]
        [InlineData(VerbosityLevel.Trace, true)]
        internal void ShouldSupportTrace(VerbosityLevel verbosityLevel, bool enabled)
        {
            // Given
            var times = Times.Exactly(enabled ? 1 : 0);
            var log = CreateInstance();
            _settings.SetupGet(i => i.VerbosityLevel).Returns(verbosityLevel);
            
            // When
            log.Trace("Orig", _text);

            // Then
            _teamCityMessageWriter.Verify(i => i.WriteMessage($"F_{"Orig", -40}line1line2"), times);
        }

        private TeamCityLog<string> CreateInstance() =>
            new(
                _settings.Object,
                _lineFormatter.Object,
                _blockWriter.Object,
                _teamCityMessageWriter.Object,
                _teamCityBuildProblemWriter.Object,
                _statistics.Object);
    }
}