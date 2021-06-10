namespace Teamcity.CSharpInteractive
{
    internal interface IFlushableRegistry
    {
        void Register(IFlushable flushable);
    }
}