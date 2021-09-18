namespace TeamCity.CSharpInteractive.Tests.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using JetBrains.TeamCity.ServiceMessages;
    using JetBrains.TeamCity.ServiceMessages.Read;
    using Shouldly;

    internal static class TestTool
    {
        public static readonly EnvironmentVariable[] DefaultVars = {
            new("TEAMCITY_VERSION", string.Empty),
            new("TEAMCITY_PROJECT_NAME", string.Empty)
        };

        private static readonly EnvironmentVariable[] TeamCityVars = {
            new("TEAMCITY_VERSION", "2021.2"),
            new("TEAMCITY_PROJECT_NAME", "Test")
        };

        public static IProcessResult Run(IEnumerable<string> args, IEnumerable<string> scriptArgs, IEnumerable<EnvironmentVariable> vars, params string[] lines)
        {
            var fileSystem = Composer.Resolve<IFileSystem>();
            var scriptFile = fileSystem.CreateTempFilePath();
            try
            {
                fileSystem.AppendAllLines(scriptFile, lines);
                var allArgs = new List<string>(args) { scriptFile };
                allArgs.AddRange(scriptArgs);
                return Composer.Resolve<IProcessRunner>().Run(allArgs.Select(i => new CommandLineArgument(i)), vars);
            }
            finally
            {
                fileSystem.DeleteFile(scriptFile);
            }
        }
        
        public static IProcessResult Run(params string[] lines) =>
            Run(Array.Empty<string>(), Array.Empty<string>(), DefaultVars, lines);
        
        public static IProcessResult RunUnderTeamCity(params string[] lines) =>
            Run(Array.Empty<string>(), Array.Empty<string>(), TeamCityVars, lines);

        public static void ShouldContainNormalTextMessage(this IEnumerable<IServiceMessage> messages, Predicate<string> textMatcher) =>
            messages.Count(i => 
                    i.Name == "message"
                    && i.GetValue("status") == "NORMAL"
                    && textMatcher(i.GetValue("text")))
                .ShouldBe(1);
        
        public static void ShouldContainWarningTextMessage(this IEnumerable<IServiceMessage> messages, Predicate<string> textMatcher) =>
            messages.Count(i => 
                    i.Name == "message"
                    && i.GetValue("status") == "WARNING"
                    && textMatcher(i.GetValue("text")))
                .ShouldBe(1);

        public static void ShouldContainBuildProblem(this IEnumerable<IServiceMessage> messages, Predicate<string> errorMatcher, Predicate<string> errorIdMatcher) =>
            messages.Count(i => 
                    i.Name == "buildProblem"
                    && errorIdMatcher(i.GetValue("identity"))
                    && errorMatcher(i.GetValue("description")))
                .ShouldBe(1);
        
        public static IReadOnlyCollection<IServiceMessage> ParseMessages(this IEnumerable<string> lines) =>
            lines.ParseMessagesInternal().ToList().AsReadOnly();

        private static IEnumerable<IServiceMessage> ParseMessagesInternal(this IEnumerable<string> lines)
        {
            var parser = Composer.Resolve<IServiceMessageParser>();
            foreach (var line in lines)
            {
                foreach (var message in parser.ParseServiceMessages(line))
                {
                    yield return message;
                }
            }
        }
    }
}