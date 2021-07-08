namespace Teamcity.CSharpInteractive
{
    internal interface IStdErr
    {
        void WriteLine(params Text[] errorLine);
    }
}