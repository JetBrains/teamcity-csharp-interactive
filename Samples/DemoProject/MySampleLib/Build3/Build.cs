using HostApi;

interface IBuild
{
    Task<string> BuildAsync();
}

class Build : IBuild
{
    private readonly Settings _settings;
    private readonly IBuildRunner _runner;

    public Build(
        Settings settings,
        IBuildRunner runner)
    {
        _settings = settings;
        _runner = runner;
    }

    public async Task<string> BuildAsync()
    {
        var build = new DotNetBuild()
            .WithConfiguration(_settings.Configuration)
            .AddProps(("version", _settings.Version.ToString()));

        await Assertion.Succeed(_runner.RunAsync(build));

        var test = new DotNetTest()
            .WithConfiguration(_settings.Configuration)
            .WithNoBuild(true);

        var testInContainer = new DockerRun(
                test.WithExecutablePath("dotnet"),
                $"mcr.microsoft.com/dotnet/sdk:6.0")
            .WithPlatform("linux")
            .AddVolumes((Environment.CurrentDirectory, "/project"))
            .WithContainerWorkingDirectory("/project");

        await Assertion.Succeed(
            Task.WhenAll(
                _runner.RunAsync(test),
                _runner.RunAsync(testInContainer)
            )
        );

        var output = Path.Combine("bin", _settings.Configuration, "output");

        var publish = new DotNetPublish()
            .WithWorkingDirectory("BlazorServerApp")
            .WithConfiguration(_settings.Configuration)
            .WithNoBuild(true)
            .WithOutput(output)
            .AddProps(("version", _settings.Version.ToString()));

        await Assertion.Succeed(_runner.RunAsync(publish));
        return Path.Combine("BlazorServerApp", output);
    }
}