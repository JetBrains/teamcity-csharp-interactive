namespace Teamcity.CSharpInteractive
{
    internal readonly struct CommandResult
    {
        public readonly ICommand Command;
        public readonly bool? Success;

        public CommandResult(ICommand command, bool? success)
        {
            Command = command;
            Success = success;
        }
    }
}