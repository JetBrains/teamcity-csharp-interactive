namespace Teamcity.CSharpInteractive
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    internal class CodeCommand: ICommand
    {
        public static readonly ICommand Shared = new CodeCommand();

        public string Name => "Code";

        public CommandKind Kind => CommandKind.Code;
        
        public override string ToString() => Name;
    }
}