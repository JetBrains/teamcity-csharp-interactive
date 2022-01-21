namespace Script.Cmd;

internal interface IPathResolverContext
{
    IDisposable Register(IPathResolver resolver);
}