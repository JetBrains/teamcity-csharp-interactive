// ReSharper disable UnusedMember.Global
namespace TeamCity.CSharpInteractive.Contracts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface ICommandLine
    {
        int? Run(CommandLine commandLine, Action<CommandLineOutput>? handler = default, TimeSpan timeout = default);
        
        Task<int?> RunAsync(CommandLine commandLine, Action<CommandLineOutput>? handler = default, CancellationToken cancellationToken = default);
    }
}