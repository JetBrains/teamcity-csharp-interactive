namespace TeamCity.CSharpInteractive;

internal interface ICommandLineParser
{
    IEnumerable<CommandLineArgument> Parse(IEnumerable<string> arguments, CommandLineArgumentType defaultArgType);
}