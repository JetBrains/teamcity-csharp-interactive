namespace Teamcity.CSharpInteractive
{
    internal interface ICSharpScriptRunner
    {
        bool Run(string script);

        void Reset();
    }
}