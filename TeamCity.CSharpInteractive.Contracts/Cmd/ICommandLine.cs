// ReSharper disable UnusedMember.Global
// ReSharper disable CheckNamespace
namespace Cmd
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface ICommandLine
    {
        int? Run(in CommandLine commandLine, Action<CommandLineOutput>? handler = default, TimeSpan timeout = default);
        
        Task<int?> RunAsync(CommandLine commandLine, Action<CommandLineOutput>? handler = default, CancellationToken cancellationToken = default);
    }
}