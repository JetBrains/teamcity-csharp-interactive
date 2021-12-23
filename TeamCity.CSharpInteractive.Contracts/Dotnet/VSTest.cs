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
        bool Integration = true,
        string ShortName = "")
        : IProcess
    {
        private readonly string _shortName = ShortName;

        public VSTest(params string[] testFileNames)
            : this(testFileNames, ImmutableList< string>.Empty, ImmutableList<(string, string)>.Empty, ImmutableList<(string, string)>.Empty, ImmutableList< string>.Empty)
        {
        }
        
        public string ShortName => !string.IsNullOrWhiteSpace(_shortName) ? _shortName : "dotnet vstest";

        public IStartInfo GetStartInfo(IHost host)
        {
            var cmd =  new CommandLine(ExecutablePath)
                .WithArgs("vstest")
                .AddArgs(TestFileNames.Where(i => !string.IsNullOrWhiteSpace(i)).ToArray())
                .WithWorkingDirectory(WorkingDirectory)
                .WithVars(Vars)
                .AddVSTestIntegration(Integration, Verbosity)
                .AddArgs(
                    ("--Tests", Tests),
                    ("--TestCaseFilter", TestCaseFilter),
                    ("--Framework", Framework),
                    ("--Platform", Platform?.ToString()),
                    ("--Settings", Settings),
                    ("--TestAdapterPath", TestAdapterPath),
                    ("--Diag", Diag),
                    ("--ResultsDirectory", ResultsDirectory),
                    ("--ParentProcessId", ParentProcessId?.ToString()),
                    ("--Port", Port?.ToString()),
                    ("--Collect", Collect)
                )
                .AddBooleanArgs(
                    ("--ListTests", ListTests),
                    ("--Parallel", Parallel),
                    ("--Blame", Blame),
                    ("--InIsolation", InIsolation)
                )
                .AddArgs(Loggers.ToArray())
                .AddArgs(Args.ToArray());
                
                var runSettings = RunSettings.Select(i => $"{i.name}={i.value}").ToArray();
                if(runSettings.Any())
                {
                    cmd = cmd.AddArgs("--").AddArgs(runSettings);
                }

                return cmd;
        }

        public ProcessState GetState(int exitCode) => exitCode == 0 ? ProcessState.Success : ProcessState.Fail;
    }
}