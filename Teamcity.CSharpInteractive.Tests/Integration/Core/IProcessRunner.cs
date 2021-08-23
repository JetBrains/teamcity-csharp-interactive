namespace Teamcity.CSharpInteractive.Tests.Integration.Core
{
    using System.Collections.Generic;

    internal interface IProcessRunner
    {
        IProcessResult Run(IEnumerable<CommandLineArgument> args, IEnumerable<EnvironmentVariable> vars);
    }
}