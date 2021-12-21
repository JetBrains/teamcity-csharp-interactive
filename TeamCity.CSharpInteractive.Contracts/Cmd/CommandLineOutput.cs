// ReSharper disable CheckNamespace
namespace Cmd
{
    public readonly record struct CommandLineOutput(CommandLine CommandLine, bool IsError, string Line);
}