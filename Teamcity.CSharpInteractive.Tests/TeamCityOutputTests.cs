namespace Teamcity.CSharpInteractive.Tests
{
    using JetBrains.TeamCity.ServiceMessages.Write.Special;
    using Moq;
    using Xunit;

    public class TeamCityOutputTests
    {
        private readonly Mock<IFlushableRegistry> _flushableRegistry;
        private readonly Mock<ITeamCityLineAcc> _stdOutAcc;
        private readonly Mock<ITeamCityLineAcc> _stdErrAcc;
        private readonly Mock<ITeamCityMessageWriter> _teamCityMessageWriter;

        public TeamCityOutputTests()
        {
            _flushableRegistry = new Mock<IFlushableRegistry>();
            _stdOutAcc = new Mock<ITeamCityLineAcc>();
            _stdErrAcc = new Mock<ITeamCityLineAcc>();
            _teamCityMessageWriter = new Mock<ITeamCityMessageWriter>();
        }

        [Fact]
        public void ShouldRegisterAsFlushable()
        {
            // Given

            // When
            var output = CreateInstance();

            // Then
            _flushableRegistry.Verify(i => i.Register(output));
        }
        
        [Fact]
        public void ShouldWriteErrorsViaAcc()
        {
            // Given
            var output = (IStdErr)CreateInstance();
            var error = new[] {new Text("err"), Text.NewLine};
            var lines = new[] {"err1", "err2"};
            _stdErrAcc.Setup(i => i.GetLines(false)).Returns(lines);

            // When
            output.Write(error);

            // Then
            _stdErrAcc.Verify(i => i.Write(error));
            _teamCityMessageWriter.Verify(i => i.WriteError("err1", null));
            _teamCityMessageWriter.Verify(i => i.WriteError("err2", null));
        }
        
        [Fact]
        public void ShouldFlushErrors()
        {
            // Given
            var output = CreateInstance();
            var lines = new[] {"err1", "err2"};
            _stdErrAcc.Setup(i => i.GetLines(true)).Returns(lines);

            // When
            output.Flush();

            // Then
            _teamCityMessageWriter.Verify(i => i.WriteError("err1", null));
            _teamCityMessageWriter.Verify(i => i.WriteError("err2", null));
        }
        
        [Fact]
        public void ShouldWriteMessagesViaAcc()
        {
            // Given
            var output = (IStdOut)CreateInstance();
            var message = new[] {new Text("message"), Text.NewLine};
            var lines = new[] {"message1", "message2"};
            _stdOutAcc.Setup(i => i.GetLines(false)).Returns(lines);

            // When
            output.Write(message);

            // Then
            _stdOutAcc.Verify(i => i.Write(message));
            _teamCityMessageWriter.Verify(i => i.WriteMessage("message1"));
            _teamCityMessageWriter.Verify(i => i.WriteMessage("message2"));
        }
        
        [Fact]
        public void ShouldFlushMessages()
        {
            // Given
            var output = CreateInstance();
            var lines = new[] {"message1", "message2"};
            _stdOutAcc.Setup(i => i.GetLines(true)).Returns(lines);

            // When
            output.Flush();

            // Then
            _teamCityMessageWriter.Verify(i => i.WriteMessage("message1"));
            _teamCityMessageWriter.Verify(i => i.WriteMessage("message2"));
        }

        private TeamCityOutput CreateInstance() => 
            new(
                _flushableRegistry.Object,
                _stdOutAcc.Object,
                _stdErrAcc.Object,
                _teamCityMessageWriter.Object);
    }
}