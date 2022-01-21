// ReSharper disable CheckNamespace
// ReSharper disable UnusedParameter.Global
namespace Cmd;

using Script;

public interface IProcess
{
    IStartInfo GetStartInfo(IHost host);
}