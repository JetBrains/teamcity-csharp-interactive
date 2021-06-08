namespace Teamcity.CSharpInteractive
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    internal class CodeCommand: ICommand
    {
        public static readonly ICommand Shared = new CodeCommand();

        private CodeCommand() { }

        public string Name => "Code";

        public override string ToString() => Name;
    }
}