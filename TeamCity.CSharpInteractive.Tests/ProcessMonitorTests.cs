namespace TeamCity.CSharpInteractive.Tests;

using Cmd;
using Contracts;

public class ProcessMonitorTests
{
    private readonly Mock<ILog<ProcessMonitor>> _log = new();
    private readonly Mock<IEnvironment> _environment = new();
    private readonly Mock<IStartInfo> _startInfo = new();

    public ProcessMonitorTests()
    {
        _startInfo.SetupGet(i => i.ExecutablePath).Returns("Cm d");
        _startInfo.SetupGet(i => i.WorkingDirectory).Returns("W d");
        _startInfo.SetupGet(i => i.Args).Returns(new []{ "Arg1", "Arg 2" });
        _startInfo.SetupGet(i => i.Vars).Returns(new []{ ("Var1", "Val 1"), ("Var2", "Val 2") });
    }

    [Fact]
    public void ShouldLogHeaderOnStart()
    {
        // Given
        _startInfo.SetupGet(i => i.ShortName).Returns("Abc xyz");
        var monitor = CreateInstance();

        // When
        monitor.Started(_startInfo.Object, 99);

        // Then
        _log.Verify(i => i.Info(It.Is<Text[]>(text => text.SequenceEqual(new [] {new("99 \"Abc xyz\" process started ", Color.Highlighted), new("\"Cm d\""), Text.Space, new("Arg1"), Text.Space, new("\"Arg 2\"")}))));
        _log.Verify(i => i.Info(It.Is<Text[]>(text => text.SequenceEqual(new Text[] {new("in directory: "), new("\"W d\"")}))));
        _log.Verify(i => i.Trace(It.IsAny<Func<Text[]>>(), It.IsAny<string>()), Times.Never);
        _log.Verify(i => i.Warning(It.IsAny<Text[]>()), Times.Never);
        _log.Verify(i => i.Error(It.IsAny<ErrorId>(),It.IsAny<Text[]>()), Times.Never);
    }
    
    [Fact]
    public void ShouldLogCurrentWorkingDirectoryWhenWasNotSpecified()
    {
        // Given
        _startInfo.SetupGet(i => i.WorkingDirectory).Returns(string.Empty);
        _startInfo.SetupGet(i => i.ShortName).Returns("Abc xyz");
        _environment.Setup(i => i.GetPath(SpecialFolder.Working)).Returns("Cur Wd");
        var monitor = CreateInstance();

        // When
        monitor.Started(_startInfo.Object, 99);

        // Then
        _log.Verify(i => i.Info(It.Is<Text[]>(text => text.SequenceEqual(new [] {new("99 \"Abc xyz\" process started ", Color.Highlighted), new("\"Cm d\""), Text.Space, new("Arg1"), Text.Space, new("\"Arg 2\"")}))));
        _log.Verify(i => i.Info(It.Is<Text[]>(text => text.SequenceEqual(new Text[] {new("in directory: "), new("\"Cur Wd\"")}))));
        _log.Verify(i => i.Trace(It.IsAny<Func<Text[]>>(), It.IsAny<string>()), Times.Never);
        _log.Verify(i => i.Warning(It.IsAny<Text[]>()), Times.Never);
        _log.Verify(i => i.Error(It.IsAny<ErrorId>(),It.IsAny<Text[]>()), Times.Never);
    }

    [Theory]
    [InlineData(ProcessState.Finished, "finished", Color.Highlighted)]
    internal void ShouldLogWhenFinishedWithSuccess(ProcessState state, string stateDescription, Color color)
    {
        // Given
        _startInfo.SetupGet(i => i.ShortName).Returns("Abc xyz");
        var monitor = CreateInstance();
        monitor.Started(_startInfo.Object, 99);

        // When
        monitor.Finished(_startInfo.Object, 22, state, 33);
        
        // Then
        _log.Verify(i => i.Info(It.Is<Text[]>(text => text.SequenceEqual(new Text[] {new("99 \"Abc xyz\" process ", color), new(stateDescription, color), new(" (in 22 ms)"), new(" with exit code 33"), new(".")}))));
        _log.Verify(i => i.Trace(It.IsAny<Func<Text[]>>(), It.IsAny<string>()), Times.Never);
        _log.Verify(i => i.Warning(It.IsAny<Text[]>()), Times.Never);
        _log.Verify(i => i.Error(It.IsAny<ErrorId>(),It.IsAny<Text[]>()), Times.Never);
    }

    [Fact]
    public void ShouldLogErrorWhenFailed()
    {
        // Given
        var monitor = CreateInstance();
        _startInfo.SetupGet(i => i.ShortName).Returns("Abc xyz");
        monitor.Started(_startInfo.Object, 99);

        // When
        monitor.Finished(_startInfo.Object, 22, ProcessState.Failed, 33);

        // Then
        _log.Verify(i => i.Error(ErrorId.Process, It.Is<Text[]>(text => text.SequenceEqual(new Text[] {new("99 \"Abc xyz\" process "), new("failed"), new(" (in 22 ms)"), new(" with exit code 33"), new(".")}))));
        _log.Verify(i => i.Trace(It.IsAny<Func<Text[]>>(), It.IsAny<string>()), Times.Never);
        _log.Verify(i => i.Warning(It.IsAny<Text[]>()), Times.Never);
    }
    
    [Fact]
    public void ShouldLogErrorWhenFailedToStart()
    {
        // Given
        _startInfo.SetupGet(i => i.ShortName).Returns("Abc xyz");
        var monitor = CreateInstance();

        // When
        monitor.Finished(_startInfo.Object, 22, ProcessState.Failed);

        // Then
        _log.Verify(i => i.Error(ErrorId.Process, It.Is<Text[]>(text => text.SequenceEqual(new Text[] {new("\"Abc xyz\" process "), new("failed"), new(" (in 22 ms)"), new(".")}))));
        _log.Verify(i => i.Trace(It.IsAny<Func<Text[]>>(), It.IsAny<string>()), Times.Never);
        _log.Verify(i => i.Warning(It.IsAny<Text[]>()), Times.Never);
    }
    
    [Fact]
    public void ShouldLogWarningWhenCanceled()
    {
        // Given
        _startInfo.SetupGet(i => i.ShortName).Returns("Abc xyz");
        var monitor = CreateInstance();
        monitor.Started(_startInfo.Object, 99);

        // When
        monitor.Finished(_startInfo.Object, 22, ProcessState.Canceled);

        // Then
        _log.Verify(i => i.Warning(It.Is<Text[]>(text => text.SequenceEqual(new Text[] {new("99 \"Abc xyz\" process "), new("canceled"), new(" (in 22 ms)"), new(".")}))));
        _log.Verify(i => i.Trace(It.IsAny<Func<Text[]>>(), It.IsAny<string>()), Times.Never);
        _log.Verify(i => i.Error(It.IsAny<ErrorId>(),It.IsAny<Text[]>()), Times.Never);
    }

    private ProcessMonitor CreateInstance() =>
        new(_log.Object, _environment.Object);
}