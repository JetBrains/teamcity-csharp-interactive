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
    public record Pack(
        IEnumerable<(string name, string value)> Props,
        IEnumerable<string> Args,
        IEnumerable<(string name, string value)> Vars,
        string ExecutablePath = "",
        string WorkingDirectory = "",
        string Project = "",
        string Output = "",
        bool NoBuild = false,
        bool IncludeSymbols = false,
        bool IncludeSource = false,
        bool Serviceable = false,
        bool NoLogo = false,
        bool NoRestore = false,
        string VersionSuffix = "",
        string Configuration = "",
        bool UseCurrentRuntime = false,
        Verbosity? Verbosity = default,
        string ShortName = "")
        : IProcess
    {
        public Pack()
            : this(Enumerable.Empty<(string, string)>(), Enumerable.Empty<string>(), Enumerable.Empty<(string, string)>())
        { }
        
        public IStartInfo GetStartInfo(IHost host) =>
            new CommandLine(string.IsNullOrWhiteSpace(ExecutablePath) ? host.GetService<IWellknownValueResolver>().Resolve(WellknownValue.DotnetExecutablePath) : ExecutablePath)
                .WithShortName(!string.IsNullOrWhiteSpace(ShortName) ? ShortName : "dotnet pack")
                .WithArgs("pack")
                .AddArgs(new []{ Project }.Where(i => !string.IsNullOrWhiteSpace(i)).ToArray())
                .WithWorkingDirectory(WorkingDirectory)
                .WithVars(Vars)
                .AddMSBuildIntegration(host, Verbosity)
                .AddArgs(
                    ("--output", Output),
                    ("--version-suffix", VersionSuffix),
                    ("--configuration", Configuration)
                )
                .AddBooleanArgs(
                    ("--no-build", NoBuild),
                    ("--include-symbols", IncludeSymbols),
                    ("--include-source", IncludeSource),
                    ("--serviceable", Serviceable),
                    ("--nologo", NoLogo),
                    ("--no-restore", NoRestore),
                    ("--use-current-runtime", UseCurrentRuntime)
                )
                .AddProps("/p", Props.ToArray())
                .AddArgs(Args.ToArray());

        public ProcessState GetState(int exitCode) => exitCode == 0 ? ProcessState.Success : ProcessState.Fail;
    }
}