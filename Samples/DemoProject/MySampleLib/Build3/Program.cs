using Microsoft.Extensions.DependencyInjection;
using NuGet.Versioning;

var target = Enum.Parse<Target>(GetProperty("target", Target.Build.ToString()), true);
var configuration = GetProperty("configuration", "Release");
var version = NuGetVersion.Parse(GetProperty("version", "1.0.0-dev", true));

var result = await
    GetService<IServiceCollection>()
        .AddSingleton(_ => new Settings(target, configuration, version))
        .AddSingleton<Root>()
        .AddSingleton<Build>()
        .AddSingleton<CreateImage>()
    .BuildServiceProvider()
    .GetRequiredService<Root>()
    .RunAsync();

return result.HasValue ? 0 : 1;

string GetProperty(string name, string defaultProp, bool showWarning = false)
{
    var prop = Props[name];
    if (!string.IsNullOrWhiteSpace(prop))
    {
        return prop;
    }

    var message = $"The property \"{name}\" was not defined, the default value \"{defaultProp}\" was used.";
    if (showWarning)
    {
        Warning(message);
    }
    else
    {
        Info(message);
    }

    return defaultProp;
}