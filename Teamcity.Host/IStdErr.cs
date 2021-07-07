namespace Teamcity.Host
{
    internal interface IStdErr
    {
        void WriteLine(params Text[] errorLine);
    }
}