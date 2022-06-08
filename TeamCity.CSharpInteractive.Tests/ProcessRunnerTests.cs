namespace TeamCity.CSharpInteractive.Tests;

using HostApi;

public class ProcessRunnerTests
{
    private readonly Mock<IProcessManager> _processManager = new();
    private readonly Mock<IStartInfo> _startInfo = new();
    private readonly Mock<IProcessMonitor> _monitor = new();
    private readonly List<Output> _output = new();
    private readonly ProcessResult _processResult;

    public ProcessRunnerTests()
    {
        _processResult = new ProcessResult(_startInfo.Object, ProcessState.Finished, 12, new []{new Text("Abc")});
    }

    [Fact]
    public void ShouldKillWhenTimeoutIsExpired()
    {
        // Given
        var timeout = TimeSpan.FromSeconds(5);
        Exception? exception;
        _processManager.Setup(i => i.Start(_startInfo.Object, out exception)).Returns(true);
        _processManager.Setup(i => i.WaitForExit(timeout)).Returns(false);
        _processManager.SetupGet(i => i.Id).Returns(99);
        _monitor.Setup(i => i.Finished(_startInfo.Object, It.IsAny<long>(), ProcessState.Canceled, default, default)).Returns(_processResult);
        var instance = CreateInstance();

        // When
        instance.Run(new ProcessInfo(_startInfo.Object, _monitor.Object, Handler), timeout).ShouldBe(_processResult);

        // Then
        _processManager.Verify(i => i.WaitForExit(timeout));
        _processManager.Verify(i => i.Kill());
        _monitor.Verify(i => i.Started(_startInfo.Object, 99));
        _monitor.Verify(i => i.Finished(_startInfo.Object, It.IsAny<long>(), ProcessState.Canceled, default, default));
    }

    [Fact]
    public void ShouldRunWhenTimeoutIsSpecified()
    {
        // Given
        var timeout = TimeSpan.FromSeconds(5);
        var processRun = new ProcessInfo(_startInfo.Object, _monitor.Object, Handler);
        Exception? exception;
        _processManager.Setup(i => i.Start(_startInfo.Object, out exception)).Returns(true);
        _processManager.Setup(i => i.WaitForExit(timeout)).Returns(true);
        _processManager.SetupGet(i => i.ExitCode).Returns(1);
        _processManager.SetupGet(i => i.Id).Returns(99);
        _monitor.Setup(i => i.Finished(_startInfo.Object, It.IsAny<long>(), ProcessState.Finished, 1, default)).Returns(_processResult);
        var instance = CreateInstance();

        // When
        instance.Run(processRun, timeout).ShouldBe(_processResult);

        // Then
        _processManager.Verify(i => i.WaitForExit(timeout));
        _processManager.Verify(i => i.Kill(), Times.Never);
        _monitor.Verify(i => i.Started(_startInfo.Object, 99));
        _monitor.Verify(i => i.Finished(_startInfo.Object, It.IsAny<long>(), ProcessState.Finished, 1, default));
    }

    [Fact]
    public void ShouldRunWhenTimeoutIsNotSpecified()
    {
        // Given
        var timeout = TimeSpan.Zero;
        var processRun = new ProcessInfo(_startInfo.Object, _monitor.Object, Handler);
        Exception? exception;
        _processManager.Setup(i => i.Start(_startInfo.Object, out exception)).Returns(true);
        _processManager.Setup(i => i.WaitForExit(TimeSpan.Zero)).Returns(true);
        _processManager.SetupGet(i => i.ExitCode).Returns(1);
        _processManager.SetupGet(i => i.Id).Returns(99);
        _monitor.Setup(i => i.Finished(_startInfo.Object, It.IsAny<long>(), ProcessState.Finished, 1, default)).Returns(_processResult);
        var instance = CreateInstance();

        // When
        instance.Run(processRun, timeout).ShouldBe(_processResult);

        // Then
        _processManager.Verify(i => i.WaitForExit(TimeSpan.Zero));
        _processManager.Verify(i => i.Kill(), Times.Never);
        _monitor.Verify(i => i.Started(_startInfo.Object, 99));
        _monitor.Verify(i => i.Finished(_startInfo.Object, It.IsAny<long>(), ProcessState.Finished, 1, default));
    }

    [Fact]
    public void ShouldNotWaitWhenCannotStart()
    {
        // Given
        var timeout = TimeSpan.Zero;
        var processRun = new ProcessInfo(_startInfo.Object, _monitor.Object, Handler);
        var cannotStart = new Exception("Cannot start");
        var exception = cannotStart;
        _processManager.Setup(i => i.Start(_startInfo.Object, out exception)).Returns(false);
        _processManager.SetupGet(i => i.Id).Returns(99);
        _monitor.Setup(i => i.Finished(_startInfo.Object, It.IsAny<long>(), ProcessState.Failed, default, exception)).Returns(_processResult);
        var instance = CreateInstance();

        // When
        instance.Run(processRun, timeout).ShouldBe(_processResult);

        // Then
        _processManager.Verify(i => i.WaitForExit(TimeSpan.Zero), Times.Never);
        _monitor.Verify(i => i.Finished(_startInfo.Object, It.IsAny<long>(), ProcessState.Failed, default, exception));
    }

    [Fact]
    public void ShouldProvideOutput()
    {
        // Given
        var timeout = TimeSpan.Zero;
        var processRun = new ProcessInfo(_startInfo.Object, _monitor.Object, Handler);
        Exception? exception;
        _processManager.Setup(i => i.Start(_startInfo.Object, out exception)).Returns(true);
        _processManager.SetupGet(i => i.ExitCode).Returns(1);
        _processManager.SetupAdd(i => i.OnOutput += Handler).Callback<Action<Output>>(i => i(new Output(_startInfo.Object, false, "out", 99)));
        _processManager.SetupGet(i => i.Id).Returns(99);
        var instance = CreateInstance();

        // When
        instance.Run(processRun, timeout);

        // Then
        _output.Count.ShouldBe(1);
    }

    [Fact]
    public void ShouldRun()
    {
        // Given
        var processRun = new ProcessInfo(_startInfo.Object, _monitor.Object, Handler);
        Exception? exception;
        _processManager.Setup(i => i.Start(_startInfo.Object, out exception)).Returns(true);
        _processManager.Setup(i => i.WaitForExit(TimeSpan.FromDays(1))).Returns(true);
        _processManager.SetupGet(i => i.ExitCode).Returns(1);
        _processManager.SetupGet(i => i.Id).Returns(99);
        _monitor.Setup(i => i.Finished(_startInfo.Object, It.IsAny<long>(), ProcessState.Finished, 1, default)).Returns(_processResult);
        var instance = CreateInstance();

        // When
        instance.Run(processRun, TimeSpan.FromDays(1)).ShouldBe(_processResult);

        // Then
        _processManager.Verify(i => i.Kill(), Times.Never);
        _monitor.Verify(i => i.Started(_startInfo.Object, 99));
        _monitor.Verify(i => i.Finished(_startInfo.Object, It.IsAny<long>(), ProcessState.Finished, 1, default));
    }

    [Fact]
    public void ShouldRunWhenStateProviderIsNotDefined()
    {
        // Given
        var processRun = new ProcessInfo(_startInfo.Object, _monitor.Object, Handler);
        Exception? exception;
        _processManager.Setup(i => i.Start(_startInfo.Object, out exception)).Returns(true);
        _processManager.Setup(i => i.WaitForExit(TimeSpan.FromDays(1))).Returns(true);
        _processManager.SetupGet(i => i.ExitCode).Returns(1);
        _processManager.SetupGet(i => i.Id).Returns(99);
        _monitor.Setup(i => i.Finished(_startInfo.Object, It.IsAny<long>(), ProcessState.Finished, 1, default)).Returns(_processResult);
        var instance = CreateInstance();

        // When
        instance.Run(processRun, TimeSpan.FromDays(1)).ShouldBe(_processResult);

        // Then
        _processManager.Verify(i => i.Kill(), Times.Never);
        _monitor.Verify(i => i.Started(_startInfo.Object, 99));
    }

    [Fact]
    public async Task ShouldRunAsync()
    {
        // Given
        var processRun = new ProcessInfo(_startInfo.Object, _monitor.Object, Handler);
        Exception? exception;
        _processManager.Setup(i => i.Start(_startInfo.Object, out exception)).Returns(true);
        _processManager.SetupGet(i => i.ExitCode).Returns(2);
        _processManager.SetupAdd(i => i.OnExit += It.IsAny<Action>()).Callback<Action>(i => i());
        _processManager.SetupGet(i => i.Id).Returns(99);
        _monitor.Setup(i => i.Finished(_startInfo.Object, It.IsAny<long>(), ProcessState.Finished, 2, default)).Returns(_processResult);
        var cancellationTokenSource = new CancellationTokenSource();
        var instance = CreateInstance();

        // When
        var result = await instance.RunAsync(processRun, cancellationTokenSource.Token);
        result.ShouldBe(_processResult);

        // Then
        _processManager.Verify(i => i.Kill(), Times.Never);
        _monitor.Verify(i => i.Started(_startInfo.Object, 99));
        _monitor.Verify(i => i.Finished(_startInfo.Object, It.IsAny<long>(), ProcessState.Finished, 2, default));
    }

    private void Handler(Output output) => _output.Add(output);

    private ProcessRunner CreateInstance() =>
        new(() => _processManager.Object);
}