// ReSharper disable InconsistentNaming
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable InvertIf
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global
namespace HostApi;

using Cmd;
using Docker;
using Immutype;

[Target]
public record DockerRun(
    // Process to run in container
    ICommandLine CommandLine,
    // Docker image
    string Image,
    // Specifies the set of command line arguments to use when starting the tool.
    IEnumerable<string> Args,
    // Specifies the set of environment variables that apply to this process and its child processes.
    IEnumerable<(string name, string value)> Vars,
    // Additional docker options
    IEnumerable<string> Options,
    // Expose a port or a range of ports
    IEnumerable<string> ExposedPorts,
    // Publish a container's port(s) to the host
    IEnumerable<string> PublishedPorts,
    // Adds bind mounts or volumes using the --mount flag
    IEnumerable<string> Mounts,
    // Bind mount a volume
    IEnumerable<(string from, string to)> Volumes,
    // Overrides the tool executable path.
    string ExecutablePath = "",
    // Specifies the working directory for the tool to be started.
    string WorkingDirectory = "",
    // Number of CPUs
    int? CPUs = default,
    // Overwrite the default ENTRYPOINT of the image
    string EntryPoint = "",
    // Container host name
    string HostName = "",
    // Kernel memory limit
    int? KernelMemory = default,
    // Memory limit
    int? Memory = default,
    // Assign a name to the container
    string? Name = default,
    // Connect a container to a network
    string Network = "",
    // Set platform if server is multi-platform capable
    string Platform = "",
    // Give extended privileges to this container
    bool? Privileged = default,
    // Pull image before running ("always"|"missing"|"never")
    DockerPullType? Pull = default,
    // Mount the container's root filesystem as read only
    bool? ReadOnly = default,
    // Automatically remove the container when it exits
    bool? AutoRemove = default,
    // Username or UID (format: <name|uid>[:<group|gid>])
    string User = "",
    // Working directory inside the container
    string ContainerWorkingDirectory = "",
    // A file with environment variables inside the container
    string EnvFile = "",
    // Specifies a short name for this operation.
    string ShortName = "")
    : ICommandLine
{
    public DockerRun(string image = "") : this(new CommandLine(string.Empty), image)
    { }

    public DockerRun(ICommandLine commandLine, string image)
        : this(
            commandLine,
            image,
            Enumerable.Empty<string>(),
            Enumerable.Empty<(string, string)>(),
            Enumerable.Empty<string>(),
            Enumerable.Empty<string>(),
            Enumerable.Empty<string>(),
            Enumerable.Empty<string>(),
            Enumerable.Empty<(string, string)>())
    { }

    public IStartInfo GetStartInfo(IHost host)
    {
        var directoryMap = new Dictionary<string, string>();
        var pathResolver = new PathResolver(Platform, directoryMap);
        using var pathResolverToken = host.GetService<IPathResolverContext>().Register(pathResolver);
        var settings = host.GetService<IDockerSettings>();

        var processInfo = CommandLine.GetStartInfo(host);
        var cmd = new CommandLine(string.IsNullOrWhiteSpace(ExecutablePath) ? settings.DockerExecutablePath : ExecutablePath)
            .WithShortName(!string.IsNullOrWhiteSpace(ShortName) ? ShortName : $"{processInfo.ShortName} in the docker container {Image}")
            .WithWorkingDirectory(WorkingDirectory)
            .WithArgs("run")
            .AddBooleanArgs(
                ("-it", Name == string.Empty),
                ("--privileged", Privileged),
                ("--read-only", ReadOnly),
                ("--rm", AutoRemove))
            .AddArgs("--expose", ExposedPorts)
            .AddArgs("--publish", PublishedPorts)
            .AddArgs("--mount", Mounts)
            .AddArgs(
                ("--cpus", CPUs?.ToString() ?? ""),
                ("--entrypoint", EntryPoint),
                ("--hostname", HostName),
                ("--kernel-memory", KernelMemory?.ToString() ?? ""),
                ("--memory", Memory?.ToString() ?? ""),
                ("--name", Name ?? string.Empty),
                ("--network", Network),
                ("--platform", Platform),
                ("--platform", Pull?.ToString() ?? string.Empty),
                ("--user", User),
                ("--workdir", ContainerWorkingDirectory),
                ("--env-file", EnvFile))
            .AddArgs(Args.ToArray())
            .AddValues("-e", "=", processInfo.Vars.ToArray());

        var additionalVolums = directoryMap.Select(i => (i.Key, i.Value));
        return cmd
            .AddValues("-v", ":", additionalVolums.ToArray())
            .AddValues("-v", ":", Volumes.ToArray())
            .AddArgs(Options.ToArray())
            .AddArgs(Image)
            .AddArgs(processInfo.ExecutablePath)
            .AddArgs(processInfo.Args.ToArray())
            .WithVars(Vars.ToArray());
    }
    
    public override string ToString() => !string.IsNullOrWhiteSpace(ShortName) ? ShortName : $"{CommandLine} in the docker container {Image}";

    private class PathResolver : IPathResolver
    {
        private readonly string _platform;
        private readonly IDictionary<string, string> _directoryMap;

        public PathResolver(string platform, IDictionary<string, string> directoryMap)
        {
            _platform = platform;
            _directoryMap = directoryMap;
        }

        public string Resolve(IHost host, string path, IPathResolver nextResolver)
        {
            path = Path.GetFullPath(path);
            if (!_directoryMap.TryGetValue(path, out var toPath))
            {
                var rootDirectory = _platform.Contains("windows", StringComparison.OrdinalIgnoreCase) ? "c:" : string.Empty;
                toPath = $"{rootDirectory}/.{Guid.NewGuid().ToString()[..8]}";
                _directoryMap.Add(path, toPath);
            }

            return toPath;
        }
    }
}