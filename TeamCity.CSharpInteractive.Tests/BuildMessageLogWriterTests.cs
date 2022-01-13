namespace TeamCity.CSharpInteractive.Tests;

using Dotnet;
using Moq;
using Xunit;

public class BuildMessageLogWriterTests
{
    private readonly Mock<ILog<BuildMessageLogWriter>> _log = new();

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
        new(_log.Object);
}