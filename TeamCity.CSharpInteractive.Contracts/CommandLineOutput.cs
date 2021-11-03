namespace TeamCity.CSharpInteractive.Contracts
{
    public record CommandLineOutput(CommandLine CommandLine, bool IsError, string Line);
}