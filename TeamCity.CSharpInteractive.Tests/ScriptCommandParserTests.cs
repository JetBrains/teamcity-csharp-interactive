// ReSharper disable ReturnValueOfPureMethodIsNotUsed
namespace TeamCity.CSharpInteractive.Tests;

using System;

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
        commands.ShouldBe(new[] {(ICommand)new CodeCommand()});
    }

    [Fact]
    public void ShouldReturnScriptCommandWhenCompleteSubmission()
    {
        // Given
        var scriptSubmissionAnalyzer = new Mock<IScriptSubmissionAnalyzer>();
        scriptSubmissionAnalyzer.Setup(i => i.IsCompleteSubmission("code" + Environment.NewLine, ScriptCommandFactory.ParseOptions)).Returns(true);
        var parser = new ScriptCommandFactory(Mock.Of<ILog<ScriptCommandFactory>>(), scriptSubmissionAnalyzer.Object);

        // When
        var commands = parser.Create(new ScriptCommand("origin", "code")).ToArray();

        // Then
        commands.ShouldBe(new ICommand[] {new ScriptCommand("origin", "code" + Environment.NewLine)});
    }

    [Fact]
    public void ShouldReturnScriptCommandWhenSeveralCompleteSubmission()
    {
        // Given
        var scriptSubmissionAnalyzer = new Mock<IScriptSubmissionAnalyzer>();
        scriptSubmissionAnalyzer.Setup(i => i.IsCompleteSubmission("code1" + Environment.NewLine, ScriptCommandFactory.ParseOptions)).Returns(false);
        scriptSubmissionAnalyzer.Setup(i => i.IsCompleteSubmission("code1" + Environment.NewLine + "code2" + Environment.NewLine, ScriptCommandFactory.ParseOptions)).Returns(true);
        scriptSubmissionAnalyzer.Setup(i => i.IsCompleteSubmission("code2" + Environment.NewLine, ScriptCommandFactory.ParseOptions)).Returns(true);
        var parser = new ScriptCommandFactory(Mock.Of<ILog<ScriptCommandFactory>>(), scriptSubmissionAnalyzer.Object);

        // When
#pragma warning disable CA1806
        parser.Create(new ScriptCommand("origin", "code1")).ToArray();
#pragma warning restore CA1806
        var commands = parser.Create(new ScriptCommand("origin", "code2")).ToArray();
        var commands2 = parser.Create(new ScriptCommand("origin", "code2")).ToArray();

        // Then
        commands.ShouldBe(new ICommand[] {new ScriptCommand("origin", "code1" + Environment.NewLine + "code2" + Environment.NewLine)});
        commands2.ShouldBe(new ICommand[] {new ScriptCommand("origin", "code2" + Environment.NewLine)});
    }
}