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
    public record Build(
        IEnumerable<(string name, string value)> Props,
        IEnumerable<string> Args,
        IEnumerable<(string name, string value)> Vars,
        string ExecutablePath = "",
        string WorkingDirectory = "",
        string Project = "",
        string Output = "",
        string Framework = "",
        string Configuration = "",
        string Runtime = "",
        string VersionSuffix = "",
        bool NoIncremental = false,
        bool NoDependencies = false,
        bool NoLogo = false,
        bool NoRestore = false,
        bool Force = false,
        Verbosity? Verbosity = default,
        string ShortName = "")
        : IProcess
    {
        public Build()
            : this(Enumerable.Empty<(string, string)>(), Enumerable.Empty<string>(), Enumerable.Empty<(string, string)>())
        { }

        public IStartInfo GetStartInfo(IHost host) =>
            new CommandLine(string.IsNullOrWhiteSpace(ExecutablePath) ? host.GetService<IWellknownValueResolver>().Resolve(WellknownValue.DotnetExecutablePath) : ExecutablePath)
                .WithShortName(!string.IsNullOrWhiteSpace(ShortName) ? ShortName : "dotnet build")
                .WithArgs("build")
                .AddArgs(new []{ Project }.Where(i => !string.IsNullOrWhiteSpace(i)).ToArray())
                .WithWorkingDirectory(WorkingDirectory)
                .WithVars(Vars.ToArray())
                .AddMSBuildIntegration(host, Verbosity)
                .AddArgs(
                    ("--output", Output),
                    ("--framework", Framework),
                    ("--configuration", Configuration),
                    ("--runtime", Runtime),
                    ("--version-suffix", VersionSuffix),
                    ("--verbosity", Verbosity?.ToString().ToLowerInvariant())
                )
                .AddBooleanArgs(
                    ("--no-incremental", NoIncremental),
                    ("--no-dependencies", NoDependencies),
                    ("--nologo", NoLogo),
                    ("--no-restore", NoRestore),
                    ("--force", Force)
                )
                .AddProps("/p", Props.ToArray())
                .AddArgs(Args.ToArray());

        public ProcessState GetState(int exitCode) => exitCode == 0 ? ProcessState.Success : ProcessState.Fail;
    }
}