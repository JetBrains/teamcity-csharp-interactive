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
    public record Clean(
        IEnumerable<(string name, string value)> Props,
        IEnumerable<string> Args,
        IEnumerable<(string name, string value)> Vars,
        string ExecutablePath = "",
        string WorkingDirectory = "",
        string Project = "",
        string Framework = "",
        string Runtime = "",
        string Configuration = "",
        string Output = "",
        bool NoLogo = false,
        Verbosity? Verbosity = default,
        string ShortName = "")
        : IProcess
    {
        public Clean(params string[] args)
            : this(Enumerable.Empty<(string, string)>(), args, Enumerable.Empty<(string, string)>())
        { }
        
        public IStartInfo GetStartInfo(IHost host) =>
            new CommandLine(string.IsNullOrWhiteSpace(ExecutablePath) ? host.GetService<ISettings>().DotnetExecutablePath : ExecutablePath)
                .WithShortName(!string.IsNullOrWhiteSpace(ShortName) ? ShortName : "dotnet clean")
                .WithArgs("clean")
                .AddArgs(new []{ Project }.Where(i => !string.IsNullOrWhiteSpace(i)).ToArray())
                .WithWorkingDirectory(WorkingDirectory)
                .WithVars(Vars.ToArray())
                .AddMSBuildIntegration(host, Verbosity)
                .AddArgs(
                    ("--output", Output),
                    ("--framework", Framework),
                    ("--runtime", Runtime),
                    ("--configuration", Configuration),
                    ("--verbosity", Verbosity?.ToString().ToLowerInvariant())
                )
                .AddBooleanArgs(
                    ("--nologo", NoLogo)
                )
                .AddProps("/p", Props.ToArray())
                .AddArgs(Args.ToArray());
    }
}