// ReSharper disable UnusedType.Global
// ReSharper disable CheckNamespace
namespace Dotnet
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Cmd;
    using TeamCity.CSharpInteractive.Contracts;

    [Immutype.TargetAttribute]
    public record Clean(
        IEnumerable<(string name, string value)> Props,
        IEnumerable<string> Args,
        IEnumerable<(string name, string value)> Vars,
        string ExecutablePath = WellknownValues.DotnetExecutablePath,
        string WorkingDirectory = "",
        string Project = "",
        string Framework = "",
        string Runtime = "",
        string Configuration = "",
        string Output = "",
        bool NoLogo = false,
        Verbosity? Verbosity = default,
        bool Integration = true)
    {
        public Clean()
            : this(ImmutableList<(string, string)>.Empty, ImmutableList< string>.Empty, ImmutableList<(string, string)>.Empty)
        {
        }

        public static implicit operator CommandLine(Clean it) =>
            new CommandLine(it.ExecutablePath)
            .WithArgs("clean")
            .AddArgs(new []{
                it.Project}.Where(i => !string.IsNullOrWhiteSpace(i)).ToArray())
            .WithWorkingDirectory(it.WorkingDirectory)
            .WithVars(it.Vars)
            .AddMSBuildIntegration(it.Integration, it.Verbosity)
            .AddArgs(
                ("--output", it.Output),
                ("--framework", it.Framework),
                ("--runtime", it.Runtime),
                ("--configuration", it.Configuration),
                ("--verbosity", it.Verbosity?.ToString().ToLowerInvariant())
            )
            .AddBooleanArgs(
                ("--nologo", it.NoLogo)
            )
            .AddProps("/p", it.Props.ToArray())
            .AddArgs(it.Args.ToArray());
    }
}