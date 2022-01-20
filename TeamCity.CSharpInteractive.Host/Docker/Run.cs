// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable InvertIf
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global
namespace Docker;

using Cmd;
using Host;

[Immutype.Target]
public record Run(
    // Process to run in container
    IProcess Process,
    // Docker image
    string Image,
    IEnumerable<string> Args,
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
    string ExecutablePath = "",
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
    bool Privileged = false,
    // Pull image before running ("always"|"missing"|"never")
    PullType? Pull = default,
    // Mount the container's root filesystem as read only
    bool ReadOnly = false,
    // Automatically remove the container when it exits
    bool AutoRemove = true,
    // Username or UID (format: <name|uid>[:<group|gid>])
    string User = "",
    // Working directory inside the container
    string ContainerWorkingDirectory = "",
    // A file with environment variables inside the container
    string EnvFile = "",
    string ShortName = "")
    : IProcess
{
    public Run(): this(new CommandLine(string.Empty), string.Empty) 
    { }
        
    public Run(IProcess process, string image)
        : this(
            process,
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
        var settings = host.GetService<ISettings>();

        var processInfo = Process.GetStartInfo(host);
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

    private class PathResolver: IPathResolver
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