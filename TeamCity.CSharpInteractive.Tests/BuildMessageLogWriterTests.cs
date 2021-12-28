namespace TeamCity.CSharpInteractive.Tests;

using System;
using Dotnet;
using JetBrains.TeamCity.ServiceMessages.Write;
using Moq;
using Xunit;

public class BuildMessageLogWriterTests
{
    private readonly Mock<ILog<BuildMessageLogWriter>> _log;
    private readonly Mock<IServiceMessageFormatter> _serviceMessageFormatter;

    public BuildMessageLogWriterTests()
    {
        _log = new Mock<ILog<BuildMessageLogWriter>>();
        _serviceMessageFormatter = new Mock<IServiceMessageFormatter>();
    }

    [Fact]
    public void ShouldWriteServiceMessagesToTrace()
    {
        // Given
        var writer = CreateInstance();

        // When
        writer.Write(new BuildMessage(BuildMessageState.ServiceMessage, new ServiceMessage("Abc")));

        // Then
        _log.Verify(i => i.Trace(It.IsAny<Func<Text[]>>(), string.Empty));
    }
    
    [Fact]
    public void ShouldWriteInfo()
    {
        // Given
        var writer = CreateInstance();

        // When
        writer.Write(new BuildMessage(BuildMessageState.Info, default, "Abc"));

        // Then
        _log.Verify(i => i.Info(It.IsAny<Text[]>()));
    }
    
    [Fact]
    public void ShouldWriteWarning()
    {
        // Given
        var writer = CreateInstance();

        // When
        writer.Write(new BuildMessage(BuildMessageState.Warning, default, "Abc"));

        // Then
        _log.Verify(i => i.Warning(It.IsAny<Text[]>()));
    }
    
    [Theory]
    [InlineData(BuildMessageState.Error)]
    [InlineData(BuildMessageState.Failure)]
    [InlineData(BuildMessageState.BuildProblem)]
    public void ShouldWriteError(BuildMessageState state)
    {
        // Given
        var writer = CreateInstance();

        // When
        writer.Write(new BuildMessage(state, default, "Abc"));

        // Then
        _log.Verify(i => i.Error(ErrorId.Build, It.IsAny<Text[]>()));
    }

    private BuildMessageLogWriter CreateInstance() =>
        new(_log.Object, _serviceMessageFormatter.Object);
}