
// ReSharper disable InconsistentNaming
// ReSharper disable ArrangeRedundantParentheses
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
namespace Script.DotNet;

using System.Linq;
using Cmd;
using Script;

internal static class CommandLineExtensions
{
    public static CommandLine AddMSBuildIntegration(this CommandLine cmd, IHost host, Verbosity? verbosity)
    {
        var virtualContext = host.GetService<IVirtualContext>();
        var settings = host.GetService<ISettings>();
        return settings.LoggersAreRequired
            ? cmd
                .AddArgs("/noconsolelogger")
                .AddMSBuildArgs(("/l", $"TeamCity.MSBuild.Logger.TeamCityMSBuildLogger,{virtualContext.Resolve(settings.DotNetLoggerDirectory)}/TeamCity.MSBuild.Logger.dll;TeamCity;plain"))
                .AddProps("/p",
                    ("VSTestLogger", "logger://teamcity"),
                    ("VSTestTestAdapterPath", $"\".;{virtualContext.Resolve(settings.DotNetLoggerDirectory)}\""),
                    ("VSTestVerbosity", (verbosity.HasValue ? (verbosity.Value >= Verbosity.Normal ? verbosity.Value : Verbosity.Normal) : Verbosity.Normal).ToString().ToLowerInvariant()))
                .AddVars(("TEAMCITY_SERVICE_MESSAGES_PATH", virtualContext.Resolve(settings.TeamCityMessagesPath)))
            : cmd;
    }

    public static CommandLine AddVSTestIntegration(this CommandLine cmd, IHost host, Verbosity? verbosity)
    {
        var virtualContext = host.GetService<IVirtualContext>();
        var settings = host.GetService<ISettings>();
        return settings.LoggersAreRequired
            ? cmd
                .AddMSBuildArgs(
                    ("--Logger", "logger://teamcity"),
                    ("--Logger", $"console;verbosity={(verbosity.HasValue ? (verbosity.Value >= Verbosity.Normal ? verbosity.Value : Verbosity.Normal) : Verbosity.Normal).ToString().ToLowerInvariant()}"),
                    ("--TestAdapterPath", $"\"{virtualContext.Resolve(settings.DotNetLoggerDirectory)}\""))
                .AddVars(("TEAMCITY_SERVICE_MESSAGES_PATH", virtualContext.Resolve(settings.TeamCityMessagesPath)))
            : cmd;
    }

    public static CommandLine AddArgs(this CommandLine cmd, params (string name, string? value)[] args) =>
        cmd.AddArgs((
                from arg in args
                where !string.IsNullOrWhiteSpace(arg.value)
                select new [] {arg.name, arg.value})
            .SelectMany(i => i)
            .ToArray());
        
    public static CommandLine AddMSBuildArgs(this CommandLine cmd, params (string name, string? value)[] args) =>
        cmd.AddArgs((
                from arg in args
                where !string.IsNullOrWhiteSpace(arg.value)
                select $"{arg.name}:{arg.value}")
            .ToArray());
        
    public static CommandLine AddBooleanArgs(this CommandLine cmd, params (string name, bool? value)[] args) =>
        cmd.AddArgs((
                from arg in args
                where arg.value ?? false
                select arg.name)
            .ToArray());
        
    public static CommandLine AddProps(this CommandLine cmd, string propertyName, params (string name, string value)[] props) =>
        cmd.AddArgs(props.Select(i => $"{propertyName}:{i.name}={i.value}")
            .ToArray());
}