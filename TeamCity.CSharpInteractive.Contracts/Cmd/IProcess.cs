// ReSharper disable CheckNamespace
// ReSharper disable UnusedParameter.Global
namespace Cmd
{
    using TeamCity.CSharpInteractive.Contracts;

    public interface IProcess: IProcessState
    {
        IStartInfo GetStartInfo(IHost host);
    }
}