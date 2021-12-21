// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable CheckNamespace
// ReSharper disable MemberCanBePrivate.Global
namespace Dotnet
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
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
        bool Integration = true)
    {

    public MSBuild()
        : this(WellknownValues.DotnetExecutablePath)
    { }

    public MSBuild(string ExecutablePath)
        : this(ImmutableList<string>.Empty, ImmutableList<(string, string)>.Empty, ImmutableList<(string, string)>.Empty, ImmutableList<(string, string)>.Empty, ExecutablePath)
    { }

    public static implicit operator CommandLine(MSBuild it) =>
        new CommandLine(it.ExecutablePath)
        .WithArgs(it.ExecutablePath == WellknownValues.DotnetExecutablePath ? new [] {"msbuild"} : Array.Empty<string>())
        .AddArgs(new []{
            it.Project}.Where(i => !string.IsNullOrWhiteSpace(i)).ToArray())
        .WithWorkingDirectory(it.WorkingDirectory)
        .WithVars(it.Vars)
        .AddMSBuildIntegration(it.Integration, it.Verbosity)
        .AddMSBuildArgs(
            ("-target", it.Target),
            ("-maxCpuCount", it.MaxCpuCount?.ToString()),
            ("-toolsVersion", it.ToolsVersion),
            ("-verbosity", it.Verbosity?.ToString().ToLower()),
            ("-warnAsError", it.WarnAsError),
            ("-warnAsMessage", it.WarnAsMessage),
            ("-ignoreProjectExtensions", it.IgnoreProjectExtensions),
            ("-nodeReuse", it.NodeReuse?.ToString()),
            ("-preprocess", it.Preprocess),
            ("-restore", it.Restore?.ToString()),
            ("-profileEvaluation", it.ProfileEvaluation),
            ("-isolateProjects", it.IsolateProjects?.ToString()),
            ("-inputResultsCaches", it.InputResultsCaches),
            ("-outputResultsCache", it.OutputResultsCache),
            ("-graphBuild", it.GraphBuild?.ToString()),
            ("-lowPriority", it.LowPriority?.ToString())
        )
        .AddBooleanArgs(
            ("-detailedSummary", it.DetailedSummary),
            ("-noAutoResponse", it.NoAutoResponse),
            ("-noLogo", it.NoLogo),
            ("-version", it.Version)
        )
        .AddProps("-restoreProperty", it.RestoreProps.ToArray())
        .AddProps("/p", it.Props.ToArray())
        .AddArgs(it.Args.ToArray());
    }
}