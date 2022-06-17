// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
namespace HostApi;

using DotNet;
using Immutype;

[Target]
public partial record DotNetNuGetPush(
    // Specifies the set of command line arguments to use when starting the tool.
    IEnumerable<string> Args,
    // Specifies the set of environment variables that apply to this process and its child processes.
    IEnumerable<(string name, string value)> Vars,
    // Specifies the server URL. NuGet identifies a UNC or local folder source and simply copies the file there instead of pushing it using HTTP.
    IEnumerable<string> Sources,
    // Specifies the symbol server URL.
    IEnumerable<string> SymbolSources,
    // Specifies the file path to the package to be pushed.
    string Package = "",
    // Overrides the tool executable path.
    string ExecutablePath = "",
    // Specifies the working directory for the tool to be started.
    string WorkingDirectory = "",
    // Forces the application to run using an invariant, English-based culture.
    bool? ForceEnglishOutput = default,
    // Specifies the timeout for pushing to a server in seconds. Defaults to 300 seconds (5 minutes). Specifying 0 applies the default value.
    int? Timeout = default,
    // The API key for the server.
    string ApiKey = "",
    // The API key for the symbol server.
    string SymbolApiKey = "",
    // Disables buffering when pushing to an HTTP(S) server to reduce memory usage.
    bool? DisableBuffering = default,
    // Doesn't push symbols (even if present).
    bool? NoSymbols = default,
    // Doesn't append "api/v2/package" to the source URL.
    bool? NoServiceEndpoint = default,
    // When pushing multiple packages to an HTTP(S) server, treats any 409 Conflict response as a warning so that the push can continue.
    bool? SkipDuplicate = default,
    // Specifies a short name for this operation.
    string ShortName = "")
{
    public DotNetNuGetPush(params string[] args)
        : this(args, Enumerable.Empty<(string, string)>(), Enumerable.Empty<string>(), Enumerable.Empty<string>())
    { }

    public IStartInfo GetStartInfo(IHost host) =>
        host.CreateCommandLine(ExecutablePath)
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