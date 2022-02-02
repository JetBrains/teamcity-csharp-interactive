using HostApi;
using JetBrains.TeamCity.ServiceMessages.Write.Special;
using Microsoft.CodeAnalysis;

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

    public async Task<Optional<string>> RunAsync()
    {
        var result = await _build.RunAsync();
        if (!result.HasValue)
        {
            return result;
        }

        var dockerfile = Path.Combine("BlazorServerApp", "Dockerfile");
        var build = new DockerCustom("build", "-f", dockerfile, "-t", "blazorapp", result.Value);
        var exitCode = await _runner.RunAsync(build, output => {WriteLine("    " + output.Line, Color.Details);});
        if (exitCode != 0)
        {
            Error("Failed to build a docker image.");
            return new Optional<string>();
        }
        
        var imageFile = Path.Combine("BlazorServerApp", "bin", _settings.Configuration, "BlazorServerApp.tar");
        var save = new DockerCustom("image", "save", "-o", imageFile, "blazorapp");
        exitCode = await _runner.RunAsync(save);
        if (exitCode != 0)
        {
            Error("Failed to save docker image.");
            return new Optional<string>();
        }

        _teamCityWriter.PublishArtifact($"{Path.GetFullPath(imageFile)} -> .");

        WriteLine("To load an image from a tar archive:", Color.Highlighted);
        WriteLine($"    docker load -i \"{imageFile}\"", Color.Highlighted);
        WriteLine("To run web application in container:", Color.Highlighted);
        WriteLine("    docker run -it --rm -p 5000:80 blazorapp", Color.Highlighted);
        WriteLine("Open url: http://localhost:5000", Color.Highlighted);
        return imageFile;
    }
}