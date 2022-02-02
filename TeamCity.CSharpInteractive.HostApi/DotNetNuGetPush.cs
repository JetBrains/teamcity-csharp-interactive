// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
namespace HostApi;

using DotNet;
using Immutype;

[Target]
public record DotNetNuGetPush(
    IEnumerable<string> Args,
    IEnumerable<(string name, string value)> Vars,
    IEnumerable<string> Sources,
    IEnumerable<string> SymbolSources,
    string Package = "",
    string ExecutablePath = "",
    string WorkingDirectory = "",
    bool? ForceEnglishOutput = default,
    int? Timeout = default,
    string ApiKey = "",
    string SymbolApiKey = "",
    bool? DisableBuffering = default,
    bool? NoSymbols = default,
    bool? NoServiceEndpoint = default,
    bool? SkipDuplicate = default,
    string ShortName = "")
    : ICommandLine
{
    public DotNetNuGetPush(params string[] args)
        : this(args, Enumerable.Empty<(string, string)>(), Enumerable.Empty<string>(), Enumerable.Empty<string>())
    { }

    public IStartInfo GetStartInfo(IHost host) =>
        new CommandLine(string.IsNullOrWhiteSpace(ExecutablePath) ? host.GetService<IDotNetSettings>().DotNetExecutablePath : ExecutablePath)
            .WithShortName(ToString())
            .WithArgs("nuget", "push")
            .AddNotEmptyArgs(Package)
            .WithWorkingDirectory(WorkingDirectory)
            .WithVars(Vars.ToArray())
            .AddArgs(Sources.Select(i => ("--source", (string?)i)).ToArray())
            .AddArgs(SymbolSources.Select(i => ("--symbol-source", (string?)i)).ToArray())
            .AddArgs(
                ("--timeout", Timeout?.ToString()),
                ("--api-key", ApiKey),
                ("--symbol-api-key", SymbolApiKey)
            )
            .AddBooleanArgs(
                ("--force-english-output", ForceEnglishOutput),
                ("--disable-buffering", DisableBuffering),
                ("--no-symbols", NoSymbols),
                ("--no-service-endpoint", NoServiceEndpoint),
                ("--skip-duplicate", SkipDuplicate)
            )
            .AddArgs(Args.ToArray());

    public override string ToString() => "dotnet nuget push".GetShortName(ShortName, Package);
}