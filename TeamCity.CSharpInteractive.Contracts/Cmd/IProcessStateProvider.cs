// ReSharper disable CheckNamespace
namespace Cmd
{
    internal interface IProcessStateProvider
    {
        ProcessState GetState(int exitCode);
    }
}