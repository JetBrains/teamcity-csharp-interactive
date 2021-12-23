// ReSharper disable CheckNamespace
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