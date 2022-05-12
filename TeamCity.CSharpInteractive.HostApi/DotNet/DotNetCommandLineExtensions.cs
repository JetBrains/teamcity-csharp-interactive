// ReSharper disable InconsistentNaming
// ReSharper disable ArrangeRedundantParentheses
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
namespace HostApi.DotNet;

using Cmd;

internal static class DotNetCommandLineExtensions
{
    public static string GetShortName(this string baseName, string shortName, string path)
    {
        if (!string.IsNullOrWhiteSpace(shortName))
        {
            return shortName;
        }

        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (string.IsNullOrWhiteSpace(path))
        {
            return baseName;
        }

        return $"{baseName} {Path.GetFileName(path)}";
    }

    public static CommandLine AddMSBuildIntegration(this CommandLine cmd, IHost host, DotNetVerbosity? verbosity)
    {
        var virtualContext = host.GetService<IVirtualContext>();
        var settings = host.GetService<IDotNetSettings>();
        return settings.LoggersAreRequired
            ? cmd
                .AddArgs("/noconsolelogger")
                .AddMSBuildArgs(("/l", $"TeamCity.MSBuild.Logger.TeamCityMSBuildLogger,{virtualContext.Resolve(settings.DotNetMSBuildLoggerDirectory)}/TeamCity.MSBuild.Logger.dll;TeamCity;plain"))
                .AddProps("-p",
                    ("VSTestLogger", "logger://teamcity"),
                    ("VSTestTestAdapterPath", $"\".;{virtualContext.Resolve(settings.DotNetVSTestLoggerDirectory)}\""),
                    ("VSTestVerbosity", (verbosity.HasValue ? (verbosity.Value >= DotNetVerbosity.Normal ? verbosity.Value : DotNetVerbosity.Normal) : DotNetVerbosity.Normal).ToString().ToLowerInvariant()))
                .AddVars(("TEAMCITY_SERVICE_MESSAGES_PATH", virtualContext.Resolve(settings.TeamCityMessagesPath)))
            : cmd;
    }

    public static CommandLine AddVSTestIntegration(this CommandLine cmd, IHost host, DotNetVerbosity? verbosity)
    {
        var virtualContext = host.GetService<IVirtualContext>();
        var settings = host.GetService<IDotNetSettings>();
        return settings.LoggersAreRequired
            ? cmd
                .AddMSBuildArgs(
                    ("--Logger", "logger://teamcity"),
                    ("--Logger", $"console;verbosity={(verbosity.HasValue ? (verbosity.Value >= DotNetVerbosity.Normal ? verbosity.Value : DotNetVerbosity.Normal) : DotNetVerbosity.Normal).ToString().ToLowerInvariant()}"),
                    ("--TestAdapterPath", $"\"{virtualContext.Resolve(settings.DotNetVSTestLoggerDirectory)}\""))
                .AddVars(("TEAMCITY_SERVICE_MESSAGES_PATH", virtualContext.Resolve(settings.TeamCityMessagesPath)))
            : cmd;
    }

    public static CommandLine AddNotEmptyArgs(this CommandLine cmd, params string[] args) =>
        cmd.AddArgs(args.Where(i => !string.IsNullOrWhiteSpace(i)).ToArray());

    public static CommandLine AddArgs(this CommandLine cmd, params (string name, string? value)[] args) =>
        cmd.AddArgs((
                from arg in args
                where !string.IsNullOrWhiteSpace(arg.value)
                select new[] {arg.name, arg.value})
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