// ReSharper disable UnusedMember.Global
namespace HostApi;

public interface ICommandLineRunner
{
    int? Run(ICommandLine commandLine, Action<Output>? handler = default, TimeSpan timeout = default);
        
    Task<int?> RunAsync(ICommandLine commandLine, Action<Output>? handler = default, CancellationToken cancellationToken = default);
}