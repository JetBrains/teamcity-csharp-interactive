namespace TeamCity.CSharpInteractive.Tests;

using HostApi;

public class ProcessMonitorTests
{
    private readonly Mock<ILog<ProcessMonitor>> _log = new();
    private readonly Mock<IEnvironment> _environment = new();
    private readonly Mock<IStartInfo> _startInfo = new();

    public ProcessMonitorTests()
    {
        _startInfo.SetupGet(i => i.ExecutablePath).Returns("Cm d");
        _startInfo.SetupGet(i => i.WorkingDirectory).Returns("W d");
        _startInfo.SetupGet(i => i.Args).Returns(new[] {"Arg1", "Arg 2"});
        _startInfo.SetupGet(i => i.Vars).Returns(new[] {("Var1", "Val 1"), ("Var2", "Val 2")});
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
        _log.Verify(i => i.Info(It.Is<Text[]>(text => text.SequenceEqual(new[] {new("99 \"Abc xyz\" process started ", Color.Highlighted), new("\"Cm d\""), Text.Space, new("Arg1"), Text.Space, new("\"Arg 2\"")}))));
        _log.Verify(i => i.Info(It.Is<Text[]>(text => text.SequenceEqual(new Text[] {new("in directory: "), new("\"W d\"")}))));
        _log.Verify(i => i.Trace(It.IsAny<Func<Text[]>>(), It.IsAny<string>()), Times.Never);
        _log.Verify(i => i.Warning(It.IsAny<Text[]>()), Times.Never);
        _log.Verify(i => i.Error(It.IsAny<ErrorId>(), It.IsAny<Text[]>()), Times.Never);
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
        _log.Verify(i => i.Info(It.Is<Text[]>(text => text.SequenceEqual(new[] {new("99 \"Abc xyz\" process started ", Color.Highlighted), new("\"Cm d\""), Text.Space, new("Arg1"), Text.Space, new("\"Arg 2\"")}))));
        _log.Verify(i => i.Info(It.Is<Text[]>(text => text.SequenceEqual(new Text[] {new("in directory: "), new("\"Cur Wd\"")}))));
        _log.Verify(i => i.Trace(It.IsAny<Func<Text[]>>(), It.IsAny<string>()), Times.Never);
        _log.Verify(i => i.Warning(It.IsAny<Text[]>()), Times.Never);
        _log.Verify(i => i.Error(It.IsAny<ErrorId>(), It.IsAny<Text[]>()), Times.Never);
    }

    private ProcessMonitor CreateInstance() =>
        new(_log.Object, _environment.Object);
}
