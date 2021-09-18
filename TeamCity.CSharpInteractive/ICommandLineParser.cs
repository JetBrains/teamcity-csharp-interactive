namespace TeamCity.CSharpInteractive
{
    using System.Collections.Generic;

    internal interface ICommandLineParser
    {
        IEnumerable<CommandLineArgument> Parse(IEnumerable<string> arguments);
    }
}