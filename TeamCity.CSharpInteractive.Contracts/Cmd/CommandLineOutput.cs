// ReSharper disable CheckNamespace
namespace Cmd
{
    public record CommandLineOutput(CommandLine CommandLine, bool IsError, string Line);
}