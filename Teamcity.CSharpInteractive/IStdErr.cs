namespace Teamcity.CSharpInteractive
{
    internal interface IStdErr
    {
        void Write(params Text[] error);
    }
}