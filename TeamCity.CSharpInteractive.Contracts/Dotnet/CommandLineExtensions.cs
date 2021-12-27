// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable ArrangeRedundantParentheses
namespace Dotnet
{
    using System.Linq;
    using Cmd;
    using TeamCity.CSharpInteractive.Contracts;

    internal static class CommandLineExtensions
    {
        public static CommandLine AddMSBuildIntegration(this CommandLine cmd, bool integration, Verbosity? verbosity) =>
            integration
                ? cmd
                    .AddArgs("/noconsolelogger")
                    .AddMSBuildArgs(("/l", $"TeamCity.MSBuild.Logger.TeamCityMSBuildLogger,{WellknownValues.DotnetLoggerDirectory}/TeamCity.MSBuild.Logger.dll;TeamCity;plain"))
                    .AddProps("/p", 
                        ("VSTestLogger", "logger://teamcity"),
                        ("VSTestTestAdapterPath", $"\".;{WellknownValues.DotnetLoggerDirectory}\""),
                        ("VSTestVerbosity", (verbosity.HasValue ? (verbosity.Value >= Verbosity.Normal ? verbosity.Value : Verbosity.Normal) : Verbosity.Normal).ToString().ToLowerInvariant()))
                    .AddVars(("TEAMCITY_SERVICE_MESSAGES_PATH", WellknownValues.TeamCityMessagesPath))
                : cmd;
        
        public static CommandLine AddVSTestIntegration(this CommandLine cmd, bool integration, Verbosity? verbosity) =>
            integration
                ? cmd
                    .AddMSBuildArgs( 
                        ("--Logger", "logger://teamcity"),
                        ("--Logger", $"console;verbosity={(verbosity.HasValue ? (verbosity.Value >= Verbosity.Normal ? verbosity.Value : Verbosity.Normal) : Verbosity.Normal).ToString().ToLowerInvariant()}"),
                        ("--TestAdapterPath", $"\"{WellknownValues.DotnetLoggerDirectory}\""))
                    .AddVars(("TEAMCITY_SERVICE_MESSAGES_PATH", WellknownValues.TeamCityMessagesPath))
                : cmd;

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
}