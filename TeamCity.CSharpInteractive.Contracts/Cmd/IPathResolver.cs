// ReSharper disable CheckNamespace
// ReSharper disable UnusedParameter.Global
namespace Cmd;

using TeamCity.CSharpInteractive.Contracts;

internal interface IPathResolver
{
    string Resolve(IHost host, string path, IPathResolver nextResolver);
}