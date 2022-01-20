// ReSharper disable CheckNamespace
// ReSharper disable UnusedParameter.Global
namespace Cmd;

using Host;

public interface IProcess
{
    IStartInfo GetStartInfo(IHost host);
}