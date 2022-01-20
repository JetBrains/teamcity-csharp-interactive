// ReSharper disable CheckNamespace
namespace Cmd;

internal interface IPathResolverContext
{
    IDisposable Register(IPathResolver resolver);
}