namespace Teamcity.CSharpInteractive.Tests
{
    using System.Linq;
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
            var commands = parser.Create(new ScriptCommand("origin", "code")).ToArray();

            // Then
            commands.ShouldBe(new []{CodeCommand.Shared}); 
        }
        
        [Fact]
        public void ShouldReturnScriptCommandWhenCompleteSubmission()
        {
            // Given
            var scriptSubmissionAnalyzer = new Mock<IScriptSubmissionAnalyzer>();
            scriptSubmissionAnalyzer.Setup(i => i.IsCompleteSubmission("code" + System.Environment.NewLine, ScriptCommandFactory.ParseOptions)).Returns(true);
            var parser = new ScriptCommandFactory(Mock.Of<ILog<ScriptCommandFactory>>(), scriptSubmissionAnalyzer.Object);

            // When
            var commands = parser.Create(new ScriptCommand("origin", "code")).ToArray();

            // Then
            commands.ShouldBe(new []{new ScriptCommand("origin", "code" + System.Environment.NewLine)}); 
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
            parser.Create(new ScriptCommand("origin", "code1")).ToArray();
            var commands = parser.Create(new ScriptCommand("origin", "code2")).ToArray();
            var commands2 = parser.Create(new ScriptCommand("origin", "code2")).ToArray();

            // Then
            commands.ShouldBe(new []{new ScriptCommand("origin", "code1" + System.Environment.NewLine + "code2" + System.Environment.NewLine)});
            commands2.ShouldBe(new []{new ScriptCommand("origin", "code2" + System.Environment.NewLine)});
        }
    }
}