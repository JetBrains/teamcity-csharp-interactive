// ReSharper disable CheckNamespace
namespace Cmd
{
    public interface IProcessState
    {
        ProcessState GetState(int exitCode);
    }
}