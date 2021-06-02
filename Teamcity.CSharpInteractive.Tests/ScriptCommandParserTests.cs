namespace Teamcity.CSharpInteractive.Tests
{
    using Moq;
    using Shouldly;
    using Xunit;

    public class ScriptCommandParserTests
    {
        [Fact]
        public void ShouldReturnCodeCommandWhenIncompleteSubmission()
        {
            // Given
            var scriptSubmissionAnalyzer = new Mock<IScriptSubmissionAnalyzer>();
            scriptSubmissionAnalyzer.Setup(i => i.IsCompleteSubmission("code", ScriptCommandFactory.ParseOptions)).Returns(false);
            var parser = new ScriptCommandFactory(Mock.Of<ILog<ScriptCommandFactory>>(), scriptSubmissionAnalyzer.Object);

            // When
            var command = parser.Create("origin", "code");

            // Then
            command.ShouldBe(CodeCommand.Shared); 
        }
        
        [Fact]
        public void ShouldReturnScriptCommandWhenCompleteSubmission()
        {
            // Given
            var scriptSubmissionAnalyzer = new Mock<IScriptSubmissionAnalyzer>();
            scriptSubmissionAnalyzer.Setup(i => i.IsCompleteSubmission("code" + System.Environment.NewLine, ScriptCommandFactory.ParseOptions)).Returns(true);
            var parser = new ScriptCommandFactory(Mock.Of<ILog<ScriptCommandFactory>>(), scriptSubmissionAnalyzer.Object);

            // When
            var command = parser.Create("origin", "code");

            // Then
            command.ShouldBe(new ScriptCommand("origin", "code" + System.Environment.NewLine)); 
        }
        
        [Fact]
        public void ShouldReturnScriptCommandWhenSeveralCompleteSubmission()
        {
            // Given
            var scriptSubmissionAnalyzer = new Mock<IScriptSubmissionAnalyzer>();
            scriptSubmissionAnalyzer.Setup(i => i.IsCompleteSubmission("code1" + System.Environment.NewLine, ScriptCommandFactory.ParseOptions)).Returns(false);
            scriptSubmissionAnalyzer.Setup(i => i.IsCompleteSubmission("code1" + System.Environment.NewLine + "code2" + System.Environment.NewLine, ScriptCommandFactory.ParseOptions)).Returns(true);
            scriptSubmissionAnalyzer.Setup(i => i.IsCompleteSubmission("code2" + System.Environment.NewLine, ScriptCommandFactory.ParseOptions)).Returns(true);
            var parser = new ScriptCommandFactory(Mock.Of<ILog<ScriptCommandFactory>>(), scriptSubmissionAnalyzer.Object);

            // When
            parser.Create("origin", "code1");
            var command = parser.Create("origin", "code2");
            var command2 = parser.Create("origin", "code2");

            // Then
            command.ShouldBe(new ScriptCommand("origin", "code1" + System.Environment.NewLine + "code2" + System.Environment.NewLine));
            command2.ShouldBe(new ScriptCommand("origin", "code2" + System.Environment.NewLine));
        }
    }
}