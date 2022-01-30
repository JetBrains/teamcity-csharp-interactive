// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using HostApi;
using HostApi.Cmd;

internal class PathResolverContext : IPathResolverContext, IVirtualContext
{
    private readonly IHost _host;
    private IPathResolver _currentResolver = EmptyResolver.Shared;
    private IPathResolver _prevResolver = EmptyResolver.Shared;

    public PathResolverContext(IHost host) => _host = host;

    public IDisposable Register(IPathResolver resolver)
    {
        var prevResolver = _prevResolver;
        _prevResolver = _currentResolver;
        _currentResolver = resolver;
        return Disposable.Create(() =>
        {
            _currentResolver = _prevResolver;
            _prevResolver = prevResolver;
        });
    }

    public string Resolve(string path) => _currentResolver.Resolve(_host, path, _prevResolver);

    private class EmptyResolver : IPathResolver
    {
        public static readonly IPathResolver Shared = new EmptyResolver();

        public string Resolve(IHost host, string path, IPathResolver nextResolver) => path;
    }
}