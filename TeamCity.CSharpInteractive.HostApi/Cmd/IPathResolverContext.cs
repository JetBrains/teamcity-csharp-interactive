namespace HostApi.Cmd;

internal interface IPathResolverContext
{
    IDisposable Register(IPathResolver resolver);
}