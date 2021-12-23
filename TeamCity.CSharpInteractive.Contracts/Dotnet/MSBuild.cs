// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable CheckNamespace
// ReSharper disable MemberCanBePrivate.Global
namespace Dotnet
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.IO;
    using System.Linq;
    using Cmd;
    using TeamCity.CSharpInteractive.Contracts;

    [Immutype.Target]
    public record MSBuild(
            IEnumerable<string> Args,
            IEnumerable<(string name, string value)> Props,
            IEnumerable<(string name, string value)> Vars,
            IEnumerable<(string name, string value)> RestoreProps,
            string ExecutablePath = WellknownValues.DotnetExecutablePath,
            string WorkingDirectory = "",
            string Project = "",
            string Target = "",
            int? MaxCpuCount = default,
            string ToolsVersion = "",
            string WarnAsError = "",
            string WarnAsMessage = "",
            string IgnoreProjectExtensions = "",
            bool? NodeReuse = default,
            string Preprocess = "",
            bool DetailedSummary = false,
            bool? Restore = default,
            string ProfileEvaluation = "",
            bool? IsolateProjects = default,
            string InputResultsCaches = "",
            string OutputResultsCache = "",
            bool? GraphBuild = default,
            bool? LowPriority = default,
            bool NoAutoResponse = false,
            bool NoLogo = false,
            bool Version = false,
            Verbosity? Verbosity = default,
            bool Integration = true,
            string ShortName = "")
        : IProcess
    {
        private readonly string _shortName = ShortName;

        public MSBuild()
            : this(WellknownValues.DotnetExecutablePath)
        {
        }

        public MSBuild(string ExecutablePath)
            : this(ImmutableList<string>.Empty, ImmutableList<(string, string)>.Empty, ImmutableList<(string, string)>.Empty, ImmutableList<(string, string)>.Empty, ExecutablePath)
        {
        }
        
        public string ShortName => !string.IsNullOrWhiteSpace(_shortName) ? _shortName : ExecutablePath == WellknownValues.DotnetExecutablePath ? "dotnet msbuild" : Path.GetFileNameWithoutExtension(ExecutablePath);

        public IStartInfo GetStartInfo(IHost host) =>
            new CommandLine(ExecutablePath)
                .WithArgs(ExecutablePath == WellknownValues.DotnetExecutablePath ? new [] {"msbuild"} : Array.Empty<string>())
                .AddArgs(new []{ Project }.Where(i => !string.IsNullOrWhiteSpace(i)).ToArray())
                .WithWorkingDirectory(WorkingDirectory)
                .WithVars(Vars)
                .AddMSBuildIntegration(Integration, Verbosity)
                .AddMSBuildArgs(
                    ("-target", Target),
                    ("-maxCpuCount", MaxCpuCount?.ToString()),
                    ("-toolsVersion", ToolsVersion),
                    ("-verbosity", Verbosity?.ToString().ToLower()),
                    ("-warnAsError", WarnAsError),
                    ("-warnAsMessage", WarnAsMessage),
                    ("-ignoreProjectExtensions", IgnoreProjectExtensions),
                    ("-nodeReuse", NodeReuse?.ToString()),
                    ("-preprocess", Preprocess),
                    ("-restore", Restore?.ToString()),
                    ("-profileEvaluation", ProfileEvaluation),
                    ("-isolateProjects", IsolateProjects?.ToString()),
                    ("-inputResultsCaches", InputResultsCaches),
                    ("-outputResultsCache", OutputResultsCache),
                    ("-graphBuild", GraphBuild?.ToString()),
                    ("-lowPriority", LowPriority?.ToString())
                )
                .AddBooleanArgs(
                    ("-detailedSummary", DetailedSummary),
                    ("-noAutoResponse", NoAutoResponse),
                    ("-noLogo", NoLogo),
                    ("-version", Version)
                )
                .AddProps("-restoreProperty", RestoreProps.ToArray())
                .AddProps("/p", Props.ToArray())
                .AddArgs(Args.ToArray());

        public ProcessState GetState(int exitCode) => exitCode == 0 ? ProcessState.Success : ProcessState.Fail;
    }
}