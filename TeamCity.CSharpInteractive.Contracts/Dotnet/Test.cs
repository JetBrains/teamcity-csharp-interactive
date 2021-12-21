// ReSharper disable UnusedType.Global
// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
namespace Dotnet
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Cmd;
    using TeamCity.CSharpInteractive.Contracts;

    [Immutype.Target]
    public record Test(
        IEnumerable<(string name, string value)> Props,
        IEnumerable<string> Args,
        IEnumerable<(string name, string value)> Vars,
        string ExecutablePath = WellknownValues.DotnetExecutablePath,
        string WorkingDirectory = "",
        string Project = "",
        string Settings = "",
        bool ListTests = false,
        string Filter = "",
        string TestAdapterPath = "",
        string Logger = "",
        string Configuration = "",
        string Framework = "",
        string Runtime = "",
        string Output = "",
        string Diag = "",
        bool NoBuild = false,
        string ResultsDirectory = "",
        string Collect = "",
        bool Blame = false,
        bool NoLogo = false,
        bool NoRestore = false,
        Verbosity? Verbosity = default,
        bool Integration = true)
    {
        public Test()
            : this(ImmutableList<(string, string)>.Empty, ImmutableList< string>.Empty, ImmutableList<(string, string)>.Empty)
        { }

        public static implicit operator CommandLine(Test it) =>
            new CommandLine(it.ExecutablePath)
            .WithArgs("test")
            .AddArgs(new []{
                it.Project}.Where(i => !string.IsNullOrWhiteSpace(i)).ToArray())
            .WithWorkingDirectory(it.WorkingDirectory)
            .WithVars(it.Vars)
            .AddMSBuildIntegration(it.Integration, it.Verbosity)
            .AddArgs(
                ("--settings", it.Settings),
                ("--test-adapter-path", it.TestAdapterPath),
                ("--logger", it.Logger),
                ("--configuration", it.Configuration),
                ("--framework", it.Framework),
                ("--runtime", it.Runtime),
                ("--output", it.Output),
                ("--diag", it.Diag),
                ("--results-directory", it.ResultsDirectory),
                ("--collect", it.Collect),
                ("--verbosity", it.Verbosity?.ToString().ToLowerInvariant())
            )
            .AddBooleanArgs(
                ("--list-tests", it.ListTests),
                ("--no-build", it.NoBuild),
                ("--blame", it.Blame),
                ("--nologo", it.NoLogo),
                ("--no-restore", it.NoRestore)
            )
            .AddProps("/p", it.Props.ToArray())
            .AddArgs(it.Args.ToArray());
    }
}