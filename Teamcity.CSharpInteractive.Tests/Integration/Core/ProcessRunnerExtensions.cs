namespace Teamcity.CSharpInteractive.Tests.Integration.Core
{
    using System;
    using System.Linq;

    internal static class ProcessRunnerExtensions
    {
        public static IProcessResult Run(this IProcessRunner runner, params string[] args)
            => runner.Run(args.Select(i => new CommandLineArgument(i)), Array.Empty<EnvironmentVariable>());
    }
}