namespace TeamCity.CSharpInteractive;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
internal readonly struct CommandResult
{
    public readonly ICommand Command;
    public readonly bool? Success;

    public CommandResult(ICommand command, bool? success)
    {
        Command = command;
        Success = success;
    }

    public override string ToString()
    {
        var success = Success.HasValue ? Success.Value.ToString() : "empty";
        return $"{Command.Name}: {success}";
    }
}