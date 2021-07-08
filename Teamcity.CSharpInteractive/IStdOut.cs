namespace Teamcity.CSharpInteractive
{
    internal interface IStdOut
    {
        void Write(params Text[] text);
        
        void WriteLine(params Text[] line);
    }
}