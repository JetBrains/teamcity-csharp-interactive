// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable InvertIf
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
namespace Docker
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Cmd;
    using TeamCity.CSharpInteractive.Contracts;

    [Immutype.Target]
    public record Run(
        // Command to run in container
        CommandLine CommandLine,
        // Docker image
        string Image,
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
        string EnvFile = "")
    {
        public Run(): this(new CommandLine(string.Empty), string.Empty) 
        { }

        public Run(CommandLine commandLine, string image)
            : this(
                commandLine,
                image,
                ImmutableList<(string, string)>.Empty,
                ImmutableList<string>.Empty,
                ImmutableList<string>.Empty,
                ImmutableList<string>.Empty,
                ImmutableList<string>.Empty,
                ImmutableList<(string, string)>.Empty)
        { }

        public static implicit operator CommandLine(Run it)
        {
            var cmd = new CommandLine(WellknownValues.DockerExecutablePath)
                .WithWorkingDirectory(it.WorkingDirectory)
                .WithArgs("run")
                .AddBooleanArgs(
                    ("-it", it.Name == string.Empty),
                    ("--privileged", it.Privileged),
                    ("--read-only", it.ReadOnly),
                    ("--rm", it.AutoRemove))
                .AddArgs("--expose", it.ExposedPorts)
                .AddArgs("--publish", it.PublishedPorts)
                .AddArgs("--mount", it.Mounts)
                .AddArgs(
                    ("--cpus", it.CPUs?.ToString() ?? ""),
                    ("--entrypoint", it.EntryPoint),
                    ("--hostname", it.HostName),
                    ("--kernel-memory", it.KernelMemory?.ToString() ?? ""),
                    ("--memory", it.Memory?.ToString() ?? ""),
                    ("--name", it.Name ?? string.Empty),
                    ("--network", it.Network),
                    ("--platform", it.Platform),
                    ("--platform", it.Pull?.ToString() ?? string.Empty),
                    ("--user", it.User),
                    ("--workdir", it.ContainerWorkingDirectory),
                    ("--env-file", it.EnvFile))
                .AddValues("-e", "=", it.CommandLine.Vars.ToArray());

            var additionalVolums = new HashSet<(string, string)>();
            var args = it.CommandLine.Args.ToArray();

            var rootDirectory = it.Platform.Contains("windows", StringComparison.OrdinalIgnoreCase) ? "c:" : string.Empty;
            var integrationDirectory = $"{rootDirectory}/.{Guid.NewGuid().ToString()[..8]}";
            (string fromDir, string toDir)[] directoryMap = {
                (WellknownValues.DotnetLoggerDirectory, $"{integrationDirectory}")
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
                .AddValues("-v", ":", it.Volumes.ToArray())
                .AddArgs(it.Options.ToArray())
                .AddArgs(it.Image)
                .AddArgs(it.CommandLine.ExecutablePath)
                .WithVars(it.Vars)
                .AddArgs(args);
        }
    }
}