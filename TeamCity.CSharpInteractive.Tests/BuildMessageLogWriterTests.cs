namespace TeamCity.CSharpInteractive.Tests;

using CSharpInteractive;
using HostApi;

public class BuildMessageLogWriterTests
{
    private readonly Mock<ILog<BuildMessageLogWriter>> _log = new();
    private readonly Mock<IStdOut> _stdOut = new ();
    private readonly Mock<IStdErr> _stdErr = new ();

    [Fact]
    public void ShouldWriteInfo()
    {
        // Given
        var writer = CreateInstance();

        // When
        writer.Write(new BuildMessage(BuildMessageState.StdOut, default, "Abc"));

        // Then
        _stdOut.Verify(i => i.WriteLine(It.Is<Text[]>(text => text.SequenceEqual(new [] {new Text("Abc")}))));
    }
    
    [Fact]
    public void ShouldWriteStdErr()
    {
        // Given
        var writer = CreateInstance();

        // When
        writer.Write(new BuildMessage(BuildMessageState.StdError, default, "Abc"));

        // Then
        _stdErr.Verify(i => i.WriteLine(It.Is<Text[]>(text => text.SequenceEqual(new [] {new Text("Abc")}))));
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
        new(_log.Object, _stdOut.Object, _stdErr.Object);
}