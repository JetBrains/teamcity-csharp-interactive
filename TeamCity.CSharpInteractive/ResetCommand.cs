namespace TeamCity.CSharpInteractive;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
internal class ResetCommand: ICommand
{
    // ReSharper disable once UnusedMember.Global
    public static readonly ICommand Shared = new ResetCommand();

    private ResetCommand() { }
        
    public string Name => "Reset";
        
    public bool Internal => false;
}