// ReSharper disable CheckNamespace
// ReSharper disable UnusedParameter.Global
namespace Cmd
{
    using TeamCity.CSharpInteractive.Contracts;

    public interface IProcess
    {
        string ShortName { get; }
        
        IStartInfo GetStartInfo(IHost host);

        ProcessState GetState(int exitCode);
    }
}