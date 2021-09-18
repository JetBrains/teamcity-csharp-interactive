namespace TeamCity.CSharpInteractive.Tests.Integration.Core
{
    using System.Collections.Generic;
    using System.Linq;

    internal readonly struct ProcessParameters
    {
        public readonly string Executable;
        public readonly string WorkingDirectory;
        public readonly IReadOnlyCollection<CommandLineArgument> Arguments;
        public readonly IReadOnlyCollection<EnvironmentVariable> Variables;

        public ProcessParameters(
            string executable,
            string workingDirectory,
            IEnumerable<CommandLineArgument> arguments,
            IEnumerable<EnvironmentVariable> variables)
        {
            Executable = executable;
            WorkingDirectory = workingDirectory;
            Arguments = arguments.ToList().AsReadOnly();
            Variables = variables.ToList().AsReadOnly();
        }
    }
}
