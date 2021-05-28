namespace Teamcity.CSharpInteractive
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    internal class HelpCommand: ICommand
    {
        public static readonly ICommand Shared = new HelpCommand();
        
        public string Name => $"Help";

        public CommandKind Kind => CommandKind.Help;
    }
}