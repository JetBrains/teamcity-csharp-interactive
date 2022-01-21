// ReSharper disable UnusedParameter.Global
namespace Script.Cmd;

using Script;

public interface ICommandLine
{
    IStartInfo GetStartInfo(IHost host);
}