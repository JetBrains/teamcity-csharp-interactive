namespace Teamcity.CSharpInteractive
{
    internal interface ICommand
    {
        string Name { get; }
        
        CommandKind Kind { get; }
    }
}