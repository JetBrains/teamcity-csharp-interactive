// ReSharper disable CheckNamespace
// ReSharper disable UnusedParameter.Global
namespace Cmd;

using TeamCity.CSharpInteractive.Contracts;

public interface IProcess
{
    IStartInfo GetStartInfo(IHost host);
}