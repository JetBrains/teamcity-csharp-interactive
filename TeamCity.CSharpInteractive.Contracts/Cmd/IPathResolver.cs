// ReSharper disable CheckNamespace
namespace Cmd
{
    using TeamCity.CSharpInteractive.Contracts;

    internal interface IPathResolver
    {
        string Resolve(IHost host, string path, IPathResolver nextResolver);
    }
}