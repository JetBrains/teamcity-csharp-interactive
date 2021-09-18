namespace TeamCity.CSharpInteractive
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    internal class ResetCommand: ICommand
    {
        public static readonly ICommand Shared = new ResetCommand();

        private ResetCommand() { }
        
        public string Name => "Reset";
        
        public bool Internal => false;
    }
}