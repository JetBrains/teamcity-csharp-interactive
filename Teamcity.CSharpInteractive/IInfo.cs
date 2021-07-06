namespace Teamcity.CSharpInteractive
{
    internal interface IInfo
    {
        void ShowHeader();

        void ShowReplHelp();
        
        void ShowHelp();
        
        void ShowVersion();

        void ShowFooter();
    }
}