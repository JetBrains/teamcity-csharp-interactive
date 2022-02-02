using HostApi;

class Build
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
    
    public async Task<BuildResult> RunAsync()
    {
        var build = new DotNetBuild()
            .WithConfiguration(_settings.Configuration)
            .AddProps(("version", _settings.Version.ToString()));

        var result = await _runner.RunAsync(build);
        if (result.ExitCode != 0)
        {
            Error("Build failed.");
            return new BuildResult(false);
        }

        var test = new DotNetTest()
            .WithConfiguration(_settings.Configuration)
            .WithNoBuild(true);

        var testInContainer = new DockerRun(
                test.WithExecutablePath("dotnet"),
                $"mcr.microsoft.com/dotnet/sdk:6.0")
            .WithPlatform("linux")
            .AddVolumes((Environment.CurrentDirectory, "/project"))
            .WithContainerWorkingDirectory("/project");

        var results = await Task.WhenAll(
            _runner.RunAsync(test),
            _runner.RunAsync(testInContainer)
        );

        if (results.Any(result => result.ExitCode != 0))
        {
            var failedTests =
                from buildResult in results
                from testResult in buildResult.Tests
                where testResult.State == TestState.Failed
                select testResult.ToString();

            foreach (var failedTest in failedTests.Distinct())
            {
                Error(failedTest);
            }

            Error("Tests failed.");
            return new BuildResult(false);
        };

        var output = Path.Combine("bin", _settings.Configuration, "output");

        var publish = new DotNetPublish()
            .WithWorkingDirectory("BlazorServerApp")
            .WithConfiguration(_settings.Configuration)
            .WithNoBuild(true)
            .WithOutput(output)
            .AddProps(("version", _settings.Version.ToString()));

        result = await _runner.RunAsync(publish);
        if (result.ExitCode != 0)
        {
            Error("Publishing failed.");
            return new BuildResult(false);
        }

        return new BuildResult(true, Path.Combine("BlazorServerApp", output));
    }
}