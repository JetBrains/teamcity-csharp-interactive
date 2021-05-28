namespace Teamcity.CSharpInteractive
{
    internal interface IScriptCommandParser
    {
        bool HasCode { get; }

        ICommand Parse(string originName, string code);
    }
}