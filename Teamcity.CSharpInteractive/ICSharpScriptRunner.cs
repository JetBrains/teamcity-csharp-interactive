namespace Teamcity.CSharpInteractive
{
    internal interface ICSharpScriptRunner
    {
        bool Run(ICommand sourceCommand, string script);

        void Reset();
    }
}