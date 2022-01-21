namespace TeamCity.CSharpInteractive.Tests;

using CSharpInteractive;
using JetBrains.TeamCity.ServiceMessages.Write;
using Script.Cmd;
using Script.DotNet;
using Shouldly;

public class BuildContextTests
{
    private readonly Mock<ITestDisplayNameToFullyQualifiedNameConverter> _testDisplayNameToFullyQualifiedNameConverter = new();
    private readonly Mock<IStartInfo> _startInfo = new();

    public BuildContextTests()
    {
        _testDisplayNameToFullyQualifiedNameConverter.Setup(i => i.Convert(It.IsAny<string>())).Returns<string>(displayName => $"Full {displayName}");
    }

    [Fact]
    public void ShouldProcessStdOutput()
    {
        // Given
        var result = CreateInstance();
        var msg = new BuildMessage(BuildMessageState.StdOut, default, "Abc");
        
        // When
        var messages = result.ProcessOutput(new Output(Mock.Of<IStartInfo>(), false, "Abc", 33));
        var buildResult = result.Create(Mock.Of<IStartInfo>(), 22);
        
        // Then
        messages.ShouldBe(new []{msg});
        buildResult.Errors.ShouldNotContain(msg);
    }
    
    [Fact]
    public void ShouldProcessErrOutput()
    {
        // Given
        var result = CreateInstance();
        var msg = new BuildMessage(BuildMessageState.StdError, default, "Abc");
        
        // When
        var messages = result.ProcessOutput(new Output(Mock.Of<IStartInfo>(), true, "Abc", 33));
        var buildResult = result.Create(Mock.Of<IStartInfo>(), 22);
        
        // Then
        messages.ShouldBe(new []{msg});
        buildResult.Errors.ShouldContain(msg);
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

        var output = new Output(_startInfo.Object, false, string.Empty, 11);
        
        // When
        result.ProcessMessage(output, testSuiteStarted).ShouldBeEmpty();
        result.ProcessMessage(output, testStdout).ToArray().ShouldBe(new []{new BuildMessage(BuildMessageState.StdOut).WithText("Some output")});
        result.ProcessMessage(output, testStderr).ToArray().ShouldBe(new []{new BuildMessage(BuildMessageState.StdError).WithText("Some error")});
        result.ProcessMessage(output, testFinished).ShouldBeEmpty();
        result.ProcessMessage(output, testSuiteFinished).ShouldBeEmpty();
        var buildResult = result.Create(_startInfo.Object, 33);

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
            new Output(_startInfo.Object, false, "Some output", 11),
            new Output(_startInfo.Object, true, "Some error", 11)
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
        
        var output = new Output(_startInfo.Object, false, string.Empty, 11);

        // When
        result.ProcessMessage(output, testSuiteStarted).ShouldBeEmpty();
        result.ProcessMessage(output, testStdout).ToArray().ShouldBe(new []{new BuildMessage(BuildMessageState.StdOut).WithText("Some output")});
        result.ProcessMessage(output, testFailed).ShouldBeEmpty();
        result.ProcessMessage(output, testSuiteFinished).ShouldBeEmpty();
        var buildResult = result.Create(_startInfo.Object, 33);

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
        test.Output.ShouldBe(new []{new Output(_startInfo.Object, false, "Some output", 11)});
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
        
        var output = new Output(_startInfo.Object, false, string.Empty, 11);

        // When
        result.ProcessMessage(output, testSuiteStarted).ShouldBeEmpty();
        result.ProcessMessage(output, testStdout).ToArray().ShouldBe(new []{new BuildMessage(BuildMessageState.StdOut).WithText("Some output")});
        result.ProcessMessage(output, testIgnored).ShouldBeEmpty();
        result.ProcessMessage(output, testSuiteFinished).ShouldBeEmpty();
        var buildResult = result.Create(_startInfo.Object, 33);

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
        test.Output.ShouldBe(new []{new Output(_startInfo.Object, false, "Some output", 11)});
    }
    
    [Theory]
    [InlineData("Normal", BuildMessageState.StdOut)]
    [InlineData("normal", BuildMessageState.StdOut)]
    [InlineData("NORMAL", BuildMessageState.StdOut)]
    [InlineData("Warning", BuildMessageState.Warning)]
    [InlineData("warning", BuildMessageState.Warning)]
    [InlineData("WARNING", BuildMessageState.Warning)]
    [InlineData("Failure", BuildMessageState.Failure)]
    [InlineData("failure", BuildMessageState.Failure)]
    [InlineData("FAILURE", BuildMessageState.Failure)]
    [InlineData("Error", BuildMessageState.StdError)]
    [InlineData("error", BuildMessageState.StdError)]
    [InlineData("ERROR", BuildMessageState.StdError)]
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

        var output = new Output(_startInfo.Object, false, string.Empty, 11);

        // When
        result.ProcessMessage(output, message).ShouldBe(new []{ buildMessage });
        var buildResult = result.Create(_startInfo.Object, 33);

        // Then
        buildResult.Tests.Count.ShouldBe(0);
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (state)
        {
            case BuildMessageState.ServiceMessage:
            case BuildMessageState.StdOut:
                buildResult.Errors.ShouldBe(Array.Empty<BuildMessage>());
                break;

            case BuildMessageState.Warning:
                buildResult.Warnings.ShouldBe(new[] { buildMessage });
                break;

            case BuildMessageState.Failure:
            case BuildMessageState.StdError:
            case BuildMessageState.BuildProblem:
                buildResult.Errors.ShouldBe(new[] { buildMessage });
                break;
        }
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
        var output = new Output(_startInfo.Object, false, string.Empty, 11);

        // When
        result.ProcessMessage(output, buildProblem).ShouldBe(new []{ buildMessage });
        var buildResult = result.Create(_startInfo.Object, 33);

        // Then
        buildResult.Tests.Count.ShouldBe(0);
        buildResult.Errors.ShouldBe(new []{ buildMessage });
    }

    private BuildContext CreateInstance() =>
        new(_testDisplayNameToFullyQualifiedNameConverter.Object);
}