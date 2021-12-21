// ReSharper disable UnusedType.Global
// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
namespace Dotnet
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Cmd;
    using TeamCity.CSharpInteractive.Contracts;

    [Immutype.Target]
    public record VSTest(
        IEnumerable<string> TestFileNames,
        IEnumerable<string> Args,
        IEnumerable<(string name, string value)> RunSettings,
        IEnumerable<(string name, string value)> Vars,
        IEnumerable<string> Loggers,
        string ExecutablePath = WellknownValues.DotnetExecutablePath,
        string WorkingDirectory = "",
        string Tests = "",
        string TestCaseFilter = "",
        string Framework = "",
        Platform? Platform = default,
        string Settings = "",
        bool ListTests = false,
        bool Parallel = false,
        string TestAdapterPath = "",
        bool Blame = false,
        string Diag = "",
        string ResultsDirectory = "",
        int? ParentProcessId = default,
        int? Port = default,
        string Collect = "",
        bool InIsolation = false,
        Verbosity? Verbosity = default,
        bool Integration = true)
    {
        public VSTest(params string[] testFileNames)
            : this(testFileNames, ImmutableList< string>.Empty, ImmutableList<(string, string)>.Empty, ImmutableList<(string, string)>.Empty, ImmutableList< string>.Empty)
        {
        }

        public static implicit operator CommandLine(VSTest it) =>
            new CommandLine(it.ExecutablePath)
            .WithArgs("vstest")
            .AddArgs(it.TestFileNames.Where(i => !string.IsNullOrWhiteSpace(i)).ToArray())
            .WithWorkingDirectory(it.WorkingDirectory)
            .WithVars(it.Vars)
            .AddVSTestIntegration(it.Integration, it.Verbosity)
            .AddArgs(
                ("--TestCaseFilter", it.TestCaseFilter),
                ("--Framework", it.Framework),
                ("--Platform", it.Platform?.ToString()),
                ("--Settings", it.Settings),
                ("--TestAdapterPath", it.TestAdapterPath),
                ("--Diag", it.Diag),
                ("--ResultsDirectory", it.ResultsDirectory),
                ("--ParentProcessId", it.ParentProcessId?.ToString()),
                ("--Port", it.Port?.ToString()),
                ("--Collect", it.Collect)
            )
            .AddBooleanArgs(
                ("--ListTests", it.ListTests),
                ("--Parallel", it.Parallel),
                ("--Blame", it.Blame),
                ("--InIsolation", it.InIsolation)
            )
            .AddArgs(it.Loggers.ToArray())
            .AddArgs(it.Args.ToArray());
    }
}