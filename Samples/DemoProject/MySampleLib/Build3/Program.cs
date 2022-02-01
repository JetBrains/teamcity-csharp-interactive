using Microsoft.Extensions.DependencyInjection;
using NuGet.Versioning;

var action = Enum.Parse<BuildAction>(GetProperty("action", BuildAction.Build.ToString()));
var configuration = GetProperty("configuration", "Release");
var version = NuGetVersion.Parse(GetProperty("version", "1.0.0-dev"));

var result = await
    GetService<IServiceCollection>()
        .AddSingleton(_ => new Settings(action, configuration, version))
        .AddSingleton<Root>()
        .AddSingleton<Build>()
        .AddSingleton<CreateImage>()
    .BuildServiceProvider()
    .GetRequiredService<Root>()
    .RunAsync();

Info(result.ToString());
return result.Success ? 0 : 1;

string GetProperty(string name, string defaultProp)
{
    var prop = Props[name];
    if (!string.IsNullOrWhiteSpace(prop))
    {
        return prop;
    }

    Warning($"The property \"{name}\" was not defined, the default value \"{defaultProp}\" was used.");
    return defaultProp;
}