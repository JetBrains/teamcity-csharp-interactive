namespace Teamcity.CSharpInteractive
{
    internal interface IStdOut
    {
        void Write(params Text[] text);
    }
}