namespace TeamCity.CSharpInteractive.Tests;

using HostApi;

public class CommandLineRunnerTests
{
    private readonly Mock<IHost> _host = new();
    private readonly Mock<IProcessRunner> _processRunner = new();

    public CommandLineRunnerTests()
    {
        _host.SetupGet(i => i.Host).Returns(_host.Object);
    }

    [Fact]
    public void ShouldRunProcess()
    {
        // Given
        var startInfo = new Mock<IStartInfo>();
        var process = new Mock<ICommandLine>();
        process.Setup(i => i.GetStartInfo(_host.Object)).Returns(startInfo.Object);
        var cmdService = CreateInstance();
        _processRunner.Setup(i => i.Run(It.IsAny<ProcessInfo>(), TimeSpan.FromSeconds(1))).Returns(new ProcessResult(ProcessState.Finished, 33));

        // When
        var exitCode = cmdService.Run(process.Object, Handler, TimeSpan.FromSeconds(1));

        // Then
        exitCode.ShouldBe(33);
    }

    [Fact]
    public async Task ShouldRunProcessAsync()
    {
        // Given
        using var cancellationTokenSource = new CancellationTokenSource();
        var token = cancellationTokenSource.Token;
        var startInfo = new Mock<IStartInfo>();
        var process = new Mock<ICommandLine>();
        process.Setup(i => i.GetStartInfo(_host.Object)).Returns(startInfo.Object);
        var cmdService = CreateInstance();
        _processRunner.Setup(i => i.RunAsync(It.IsAny<ProcessInfo>(), token)).Returns(Task.FromResult(new ProcessResult(ProcessState.Finished, 33)));

        // When
        var exitCode = await cmdService.RunAsync(process.Object, Handler, token);

        // Then
        exitCode.ShouldBe(33);
    }

    private static void Handler(Output obj) { }

    private CommandLineRunner CreateInstance() =>
        new(_host.Object, _processRunner.Object, Mock.Of<IProcessMonitor>);
}