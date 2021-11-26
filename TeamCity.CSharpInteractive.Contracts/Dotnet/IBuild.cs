// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
// ReSharper disable CheckNamespace
namespace Dotnet
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Cmd;

    public interface IBuild
    {
        BuildResult Run(CommandLine commandLine, Action<CommandLineOutput>? handler = default, TimeSpan timeout = default);
        
        Task<BuildResult> RunAsync(CommandLine commandLine, Action<CommandLineOutput>? handler = default, CancellationToken cancellationToken = default);
    }
}