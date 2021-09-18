namespace TeamCity.CSharpInteractive
{
    internal interface ICSharpScriptRunner
    {
        bool Run(ICommand sourceCommand, string script);

        void Reset();
    }
}