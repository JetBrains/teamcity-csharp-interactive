// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable InvertIf
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global
namespace Docker
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cmd;
    using TeamCity.CSharpInteractive.Contracts;

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
            var processInfo = Process.GetStartInfo(host);
            var valueResolver = host.GetService<IWellknownValueResolver>();
            var cmd = new CommandLine(string.IsNullOrWhiteSpace(ExecutablePath) ? valueResolver.Resolve(WellknownValue.DockerExecutablePath) : ExecutablePath)
                .WithShortName(!string.IsNullOrWhiteSpace(ShortName) ? ShortName : $"docker run {Image} {processInfo.ShortName}")
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

            var additionalVolums = new HashSet<(string, string)>();
            var args = processInfo.Args.ToArray();

            var rootDirectory = Platform.Contains("windows", StringComparison.OrdinalIgnoreCase) ? "c:" : string.Empty;
            var integrationDirectory = $"{rootDirectory}/.{Guid.NewGuid().ToString()[..8]}";
            (string fromDir, string toDir)[] directoryMap = {
                (valueResolver.Resolve(WellknownValue.DotnetLoggerDirectory), $"{integrationDirectory}")
            };

            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                foreach (var (fromDir, toDir) in directoryMap)
                {
                    if (arg.Contains(fromDir))
                    {
                        args[i] = arg.Replace(fromDir, toDir);
                        additionalVolums.Add((fromDir, toDir));
                    }
                }
            }

            return cmd
                .AddValues("-v", ":", additionalVolums.ToArray())
                .AddValues("-v", ":", Volumes.ToArray())
                .AddArgs(Options.ToArray())
                .AddArgs(Image)
                .AddArgs(processInfo.ExecutablePath)
                .WithVars(Vars)
                .AddArgs(args);
        }

        public ProcessState GetState(int exitCode) => exitCode == 0 ? ProcessState.Success : ProcessState.Fail;
    }
}