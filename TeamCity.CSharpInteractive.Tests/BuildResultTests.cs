namespace TeamCity.CSharpInteractive.Tests;

using System;
using System.Linq;
using Cmd;
using Dotnet;
using JetBrains.TeamCity.ServiceMessages.Write;
using Moq;
using Shouldly;
using Xunit;
using BuildResult = BuildResult;

public class BuildResultTests
{
    private readonly Mock<ITestDisplayNameToFullyQualifiedNameConverter> _testDisplayNameToFullyQualifiedNameConverter = new();
    private readonly Mock<IStartInfo> _startInfo = new();

    public BuildResultTests()
    {
        _testDisplayNameToFullyQualifiedNameConverter.Setup(i => i.Convert(It.IsAny<string>())).Returns<string>(displayName => $"Full {displayName}");
    }

    [Fact]
    public void ShouldProcessTestFinished()
    {
        // Given
        var result = CreateInstance();
        var testSuiteStarted = new ServiceMessage("testSuiteStarted")
        {
            { "name", "Assembly1" },
            { "flowId", "11" }
        };
        
        var testStdout = new ServiceMessage("testStdout")
        {
            { "name", "Test1" },
            { "flowId", "11" },
            { "out", "Some output" }
        };
        
        var testStderr = new ServiceMessage("testStderr")
        {
            { "name", "Test1" },
            { "flowId", "11" },
            { "out", "Some error" }
        };
        
        var testFinished = new ServiceMessage("testFinished")
        {
            { "name", "Test1" },
            { "flowId", "11" },
            { "duration", "123" }
        };
        
        var testSuiteFinished = new ServiceMessage("testSuiteFinished")
        {
            { "name", "Assembly1" },
            { "flowId", "11" }
        };

        // When
        result.ProcessMessage(_startInfo.Object, testSuiteStarted).ShouldBeEmpty();
        result.ProcessMessage(_startInfo.Object, testStdout).ToArray().ShouldBe(new []{new BuildMessage(BuildMessageState.Info).WithText("Some output")});
        result.ProcessMessage(_startInfo.Object, testStderr).ToArray().ShouldBe(new []{new BuildMessage(BuildMessageState.Error).WithText("Some error")});
        result.ProcessMessage(_startInfo.Object, testFinished).ShouldBeEmpty();
        result.ProcessMessage(_startInfo.Object, testSuiteFinished).ShouldBeEmpty();
        var buildResult = result.Create();

        // Then
        buildResult.Tests.Count.ShouldBe(1);
        var test = buildResult.Tests[0];
        test.AssemblyName.ShouldBe("Assembly1");
        test.DisplayName.ShouldBe("Test1");
        test.FullyQualifiedName.ShouldBe("Full Test1");
        test.Duration.ShouldBe(TimeSpan.FromMilliseconds(123));
        test.State.ShouldBe(TestState.Passed);
        test.Message.ShouldBeEmpty();
        test.Details.ShouldBeEmpty();
        test.Output.ShouldBe(new []
        {
            new Output(_startInfo.Object, false, "Some output"),
            new Output(_startInfo.Object, true, "Some error")
        });
    }
    
    [Fact]
    public void ShouldProcessTestFailed()
    {
        // Given
        var result = CreateInstance();
        var testSuiteStarted = new ServiceMessage("testSuiteStarted")
        {
            { "name", "Assembly1" },
            { "flowId", "11" }
        };
        
        var testStdout = new ServiceMessage("testStdout")
        {
            { "name", "Test1" },
            { "flowId", "11" },
            { "out", "Some output" }
        };
        
        var testFailed = new ServiceMessage("testFailed")
        {
            { "name", "Test1" },
            { "flowId", "11" },
            { "message", "Some message" },
            { "details", "Error details" }
        };
        
        var testSuiteFinished = new ServiceMessage("testSuiteFinished")
        {
            { "name", "Assembly1" },
            { "flowId", "11" }
        };

        // When
        result.ProcessMessage(_startInfo.Object, testSuiteStarted).ShouldBeEmpty();
        result.ProcessMessage(_startInfo.Object, testStdout).ToArray().ShouldBe(new []{new BuildMessage(BuildMessageState.Info).WithText("Some output")});
        result.ProcessMessage(_startInfo.Object, testFailed).ShouldBeEmpty();
        result.ProcessMessage(_startInfo.Object, testSuiteFinished).ShouldBeEmpty();
        var buildResult = result.Create();

        // Then
        buildResult.Tests.Count.ShouldBe(1);
        var test = buildResult.Tests[0];
        test.AssemblyName.ShouldBe("Assembly1");
        test.DisplayName.ShouldBe("Test1");
        test.FullyQualifiedName.ShouldBe("Full Test1");
        test.Duration.ShouldBe(TimeSpan.Zero);
        test.State.ShouldBe(TestState.Failed);
        test.Message.ShouldBe("Some message");
        test.Details.ShouldBe("Error details");
        test.Output.ShouldBe(new []{new Output(_startInfo.Object, false, "Some output")});
    }
    
    [Fact]
    public void ShouldProcessTestIgnored()
    {
        // Given
        var result = CreateInstance();
        var testSuiteStarted = new ServiceMessage("testSuiteStarted")
        {
            { "name", "Assembly1" },
            { "flowId", "11" }
        };
        
        var testStdout = new ServiceMessage("testStdout")
        {
            { "name", "Test1" },
            { "flowId", "11" },
            { "out", "Some output" }
        };
        
        var testIgnored = new ServiceMessage("testIgnored")
        {
            { "name", "Test1" },
            { "flowId", "11" },
            { "message", "Some message" }
        };
        
        var testSuiteFinished = new ServiceMessage("testSuiteFinished")
        {
            { "name", "Assembly1" },
            { "flowId", "11" }
        };

        // When
        result.ProcessMessage(_startInfo.Object, testSuiteStarted).ShouldBeEmpty();
        result.ProcessMessage(_startInfo.Object, testStdout).ToArray().ShouldBe(new []{new BuildMessage(BuildMessageState.Info).WithText("Some output")});
        result.ProcessMessage(_startInfo.Object, testIgnored).ShouldBeEmpty();
        result.ProcessMessage(_startInfo.Object, testSuiteFinished).ShouldBeEmpty();
        var buildResult = result.Create();

        // Then
        buildResult.Tests.Count.ShouldBe(1);
        var test = buildResult.Tests[0];
        test.AssemblyName.ShouldBe("Assembly1");
        test.DisplayName.ShouldBe("Test1");
        test.FullyQualifiedName.ShouldBe("Full Test1");
        test.Duration.ShouldBe(TimeSpan.Zero);
        test.State.ShouldBe(TestState.Ignored);
        test.Message.ShouldBe("Some message");
        test.Details.ShouldBeEmpty();
        test.Output.ShouldBe(new []{new Output(_startInfo.Object, false, "Some output")});
    }
    
    [Theory]
    [InlineData("Normal", BuildMessageState.Info)]
    [InlineData("normal", BuildMessageState.Info)]
    [InlineData("NORMAL", BuildMessageState.Info)]
    [InlineData("Warning", BuildMessageState.Warning)]
    [InlineData("Failure", BuildMessageState.Failure)]
    [InlineData("Error", BuildMessageState.Error)]
    public void ShouldProcessMessage(string status, BuildMessageState state)
    {
        // Given
        var result = CreateInstance();
        var message = new ServiceMessage("message")
        {
            { "text", "some text" },
            { "status", status },
            { "errorDetails", "error details" }
        };

        var buildMessage = new BuildMessage(state, default, "some text", "error details");

        // When
        result.ProcessMessage(_startInfo.Object, message).ShouldBe(new []{ buildMessage });
        var buildResult = result.Create();

        // Then
        buildResult.Tests.Count.ShouldBe(0);
        buildResult.Messages.ShouldBe(new []{ buildMessage });
    }
    
    [Fact]
    public void ShouldBuildProblem()
    {
        // Given
        var result = CreateInstance();
        var buildProblem = new ServiceMessage("buildProblem")
        {
            { "description", "Problem description" },
            { "identity", "ID123" }
        };

        var buildMessage = new BuildMessage(BuildMessageState.BuildProblem, default, "Problem description", "ID123");

        // When
        result.ProcessMessage(_startInfo.Object, buildProblem).ShouldBe(new []{ buildMessage });
        var buildResult = result.Create();

        // Then
        buildResult.Tests.Count.ShouldBe(0);
        buildResult.Messages.ShouldBe(new []{ buildMessage });
    }

    private BuildResult CreateInstance() =>
        new(_testDisplayNameToFullyQualifiedNameConverter.Object);
}