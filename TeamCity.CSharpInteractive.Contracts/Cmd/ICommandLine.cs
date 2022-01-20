// ReSharper disable UnusedMember.Global
// ReSharper disable CheckNamespace
namespace Cmd;

public interface ICommandLine
{
    int? Run(IProcess process, Action<Output>? handler = default, TimeSpan timeout = default);
        
    Task<int?> RunAsync(IProcess process, Action<Output>? handler = default, CancellationToken cancellationToken = default);
}