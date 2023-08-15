// ReSharper disable InconsistentNaming
// ReSharper disable ArrangeRedundantParentheses
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
namespace HostApi.DotNet;

using Cmd;

internal static class DotNetCommandLineExtensions
{
    internal static CommandLine CreateCommandLine(this IHost host, string executablePath) => new(host.GetExecutablePath(executablePath));

    private static string GetExecutablePath(this IHost host, string executablePath)
    {
        if (!string.IsNullOrWhiteSpace(executablePath))
        {
            return executablePath;
        }

        executablePath = host.GetService<IDotNetSettings>().DotNetExecutablePath;
        return host.GetService<IVirtualContext>().IsActive
            ? Path.GetFileNameWithoutExtension(executablePath)
            : executablePath;
    }
    
    public static string GetShortName(this string baseName, string shortName, string path = "")
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

    public static CommandLine AddMSBuildLoggers(this CommandLine cmd, IHost host, DotNetVerbosity? verbosity = default)
    {
        var virtualContext = host.GetService<IVirtualContext>();
        var settings = host.GetService<IDotNetSettings>();

        if (settings.LoggersAreRequired == false)
        {
            return cmd;
        }

        return cmd
            .AddArgs("/noconsolelogger")
            .AddMSBuildArgs(("/l", $"TeamCity.MSBuild.Logger.TeamCityMSBuildLogger,{virtualContext.Resolve(settings.DotNetMSBuildLoggerDirectory)}/TeamCity.MSBuild.Logger.dll;TeamCity;plain"))
            .AddProps("-p",
                ("VSTestLogger", "logger://teamcity"),
                ("VSTestTestAdapterPath", virtualContext.Resolve(settings.DotNetVSTestLoggerDirectory)),
                ("VSTestVerbosity", (verbosity.HasValue ? (verbosity.Value >= DotNetVerbosity.Normal ? verbosity.Value : DotNetVerbosity.Normal) : DotNetVerbosity.Normal).ToString().ToLowerInvariant()));
    }

    public static CommandLine AddTeamCityEnvironmentVariables(this CommandLine cmd, IHost host)
    {
        var virtualContext = host.GetService<IVirtualContext>();
        var settings = host.GetService<IDotNetSettings>();
        string? ResolvePath(string? s) => s == null ? null : virtualContext.Resolve(s);

        return cmd
            .AddNonEmptyVars(
                ("TEAMCITY_SERVICE_MESSAGES_PATH", ResolvePath(settings.TeamCityServiceMessagesBackupPathEnvValue)),
                ("TEAMCITY_TEST_REPORT_FILES_PATH", ResolvePath(settings.TeamCityTestReportFilesPathEnvValue)),
                ("TEAMCITY_FALLBACK_TO_STDOUT_TEST_REPORTING", settings.TeamCityFallbackToStdOutTestReportingEnvValue)
            );
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

    public static CommandLine AddNonEmptyVars(this CommandLine cmd, params (string name, string? value)[] vars)
    {
        var nonEmptyVars = vars
            .Where(x => !string.IsNullOrWhiteSpace(x.value))
            .Select(x => (x.name, x.value!))
            .ToArray();

        return cmd.AddVars(nonEmptyVars);
    }
}
