using HostApi;
using JetBrains.TeamCity.ServiceMessages.Write.Special;

class CreateImage
{
    private readonly Settings _settings;
    private readonly Build _build;
    private readonly ICommandLineRunner _runner;
    private readonly ITeamCityWriter _teamCityWriter;

    public CreateImage(
        Settings settings,
        Build build,
        ICommandLineRunner runner,
        ITeamCityWriter teamCityWriter)
    {
        _settings = settings;
        _build = build;
        _runner = runner;
        _teamCityWriter = teamCityWriter;
    }

    public async Task<BuildResult> RunAsync()
    {
        var result = await _build.RunAsync();
        if (!result.Success)
        {
            return result;
        }

        var dockerfile = Path.Combine("BlazorServerApp", "Dockerfile");
        var build = new DockerCustom("build", "-f", dockerfile, "-t", "blazorapp", result.Output);
        var exitCode = await _runner.RunAsync(build, output => {WriteLine("    " + output.Line, Color.Details);});
        if (exitCode != 0)
        {
            Error("Failed to build a docker image.");
            return new BuildResult(false);
        }
        
        var imageFile = Path.Combine("BlazorServerApp", "bin", _settings.Configuration, "BlazorServerApp.tar");
        var save = new DockerCustom("image", "save", "-o", imageFile, "blazorapp");
        exitCode = await _runner.RunAsync(save);
        if (exitCode != 0)
        {
            Error("Failed to save docker image.");
            return new BuildResult(false);
        }

        _teamCityWriter.PublishArtifact($"{Path.GetFullPath(imageFile)} -> .");
        return new BuildResult(true, imageFile);
    }
}