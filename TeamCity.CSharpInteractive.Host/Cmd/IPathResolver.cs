// ReSharper disable UnusedParameter.Global
namespace Script.Cmd;

using Script;

internal interface IPathResolver
{
    string Resolve(IHost host, string path, IPathResolver nextResolver);
}