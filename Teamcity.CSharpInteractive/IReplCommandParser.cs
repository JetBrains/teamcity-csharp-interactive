namespace Teamcity.CSharpInteractive
{
    internal interface IReplCommandParser
    {
        bool TryParse(string replCommand, out ICommand? command);
    }
}