namespace Teamcity.CSharpInteractive
{
    internal interface IScriptCommandFactory
    {
        ICommand Create(string originName, string code);
    }
}