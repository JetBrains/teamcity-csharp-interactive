// ReSharper disable UnusedParameter.Global
namespace HostApi;

public interface ICommandLine
{
    IStartInfo GetStartInfo(IHost host);
}