namespace TeamCity.CSharpInteractive.Tests
{
    using JetBrains.TeamCity.ServiceMessages.Write.Special;
    using Moq;
    using Xunit;

    public class TeamCityOutputTests
    {
        private readonly Mock<ITeamCityLineFormatter> _lineFormatter;
        private readonly Mock<ITeamCityWriter> _teamCityWriter;

        public TeamCityOutputTests()
        {
            _lineFormatter = new Mock<ITeamCityLineFormatter>();
            _lineFormatter.Setup(i => i.Format(It.IsAny<Text[]>())).Returns<Text[]>(i => "F_" + i.ToSimpleString());
            _teamCityWriter = new Mock<ITeamCityWriter>();
        }

        [Fact]
        public void ShouldWriteError()
        {
            // Given
            var output = (IStdErr)CreateInstance();
            
            // When
            output.WriteLine(new[] {new Text("err")});

            // Then
            _teamCityWriter.Verify(i => i.WriteError("F_err", null));
        }
        
        [Fact]
        public void ShouldWriteMessage()
        {
            // Given
            var output = (IStdOut)CreateInstance();
            
            // When
            output.WriteLine(new[] {new Text("message")});

            // Then
            _teamCityWriter.Verify(i => i.WriteMessage("F_message"));
        }
        
        private TeamCityOutput CreateInstance() => 
            new(
                _lineFormatter.Object,
                _teamCityWriter.Object);
    }
}