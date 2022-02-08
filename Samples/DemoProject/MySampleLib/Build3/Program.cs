// Run this from the working directory where the solution or project to build is located.
using Microsoft.Extensions.DependencyInjection;
using NuGet.Versioning;

var configuration = Property.Get("configuration", "Release");
var version = NuGetVersion.Parse(Property.Get("version", "1.0.0-dev", true));

await 
    GetService<IServiceCollection>()
        .AddSingleton(_ => new Settings(configuration, version))
        .AddSingleton<IRoot, Root>()
        .AddSingleton<IBuild, Build>()
        .AddSingleton<ICreateImage, CreateImage>()
    .BuildServiceProvider()
    .GetRequiredService<IRoot>()
    .BuildAsync();