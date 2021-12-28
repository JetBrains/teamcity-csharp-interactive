// ReSharper disable UnusedType.Global
// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
namespace Dotnet
{
    using System.Collections.Generic;
    using System.Linq;
    using Cmd;
    using TeamCity.CSharpInteractive.Contracts;

    [Immutype.Target]
    public record Run(
        IEnumerable<string> Args,
        IEnumerable<(string name, string value)> Vars,
        string ExecutablePath = "",
        string WorkingDirectory = "",
        string Framework = "",
        string Configuration = "",
        string Runtime = "",
        string Project = "",
        string LaunchProfile = "",
        bool NoLaunchProfile = false,
        bool NoBuild = false,
        bool NoRestore = false,
        bool NoDependencies = false,
        bool Force = false,
        Verbosity? Verbosity = default,
        string ShortName = "")
        : IProcess
    {
        public Run()
            : this(Enumerable.Empty<string>(), Enumerable.Empty<(string, string)>())
        { }
        
        public IStartInfo GetStartInfo(IHost host) =>
            new CommandLine(string.IsNullOrWhiteSpace(ExecutablePath) ? host.GetService<IWellknownValueResolver>().Resolve(WellknownValue.DotnetExecutablePath) : ExecutablePath)
                .WithShortName(!string.IsNullOrWhiteSpace(ShortName) ? ShortName : "dotnet run")
                .WithArgs("run")
                .WithWorkingDirectory(WorkingDirectory)
                .WithVars(Vars)
                .AddArgs(
                    ("--framework", Framework),
                    ("--configuration", Configuration),
                    ("--runtime", Runtime),
                    ("--project", Project),
                    ("--launch-profile", LaunchProfile),
                    ("--verbosity", Verbosity?.ToString().ToLowerInvariant())
                )
                .AddBooleanArgs(
                    ("--no-launch-profile", NoLaunchProfile),
                    ("--no-build", NoBuild),
                    ("--no-restore", NoRestore),
                    ("--no-dependencies", NoDependencies),
                    ("--force", Force)
                )
                .AddArgs(Args.ToArray());

        public ProcessState GetState(int exitCode) => ProcessState.Unknown;
    }
}