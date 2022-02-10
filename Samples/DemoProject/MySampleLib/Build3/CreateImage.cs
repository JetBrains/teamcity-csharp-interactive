using HostApi;
using JetBrains.TeamCity.ServiceMessages.Write.Special;

interface ICreateImage
{
    Task<string> BuildAsync();
}

class CreateImage : ICreateImage
{
    private readonly Settings _settings;
    private readonly IBuild _build;
    private readonly ICommandLineRunner _runner;
    private readonly ITeamCityWriter _teamCityWriter;
    private readonly IProperties _properties;

    public CreateImage(
        IProperties properties,
        Settings settings,
        IBuild build,
        ICommandLineRunner runner,
        ITeamCityWriter teamCityWriter)
    {
        _properties = properties;
        _settings = settings;
        _build = build;
        _runner = runner;
        _teamCityWriter = teamCityWriter;
    }

    public async Task<string> BuildAsync()
    {
        var buildOutputPath = await _build.BuildAsync();

        var dockerfile = Path.Combine("BlazorServerApp", "Dockerfile");
        var build = new DockerCustom("build", "-f", dockerfile, "-t", "blazorapp", buildOutputPath);
        await Assertion.Succeed(
            _runner.RunAsync(build, output => { WriteLine("    " + output.Line, Color.Details); }),
            "build a docker image"
        );

        var imageFile = Path.Combine("BlazorServerApp", "bin", _settings.Configuration, "BlazorServerApp.tar");
        var save = new DockerCustom("image", "save", "-o", imageFile, "blazorapp");
        await Assertion.Succeed(
            _runner.RunAsync(save),
            "Saving a docker image"
        );

        if (_properties.TryGetValue("teamcity.version", out _))
        {
            _teamCityWriter.PublishArtifact($"{Path.GetFullPath(imageFile)} -> .");
        }

        WriteLine("To load an image from a tar archive:", Color.Highlighted);
        WriteLine($"    docker load -i \"{imageFile}\"", Color.Highlighted);
        WriteLine("To run web application in container:", Color.Highlighted);
        WriteLine("    docker run -it --rm -p 5000:80 blazorapp", Color.Highlighted);
        WriteLine("Open url: http://localhost:5000", Color.Highlighted);
        return imageFile;
    }
}