using Microsoft.Extensions.DependencyInjection;
using NuGet.Versioning;

var configuration = Tools.GetProperty("configuration", "Release");
var version = NuGetVersion.Parse(Tools.GetProperty("version", "1.0.0-dev", true));

var result = await
    GetService<IServiceCollection>()
        .AddSingleton(_ => new Settings(configuration, version))
        .AddSingleton<Root>()
        .AddSingleton<Build>()
        .AddSingleton<CreateImage>()
    .BuildServiceProvider()
    .GetRequiredService<Root>()
    .RunAsync();

return result.HasValue ? 0 : 1;