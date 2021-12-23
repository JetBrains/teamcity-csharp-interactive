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
        BuildResult Run(IProcess process, Action<Output>? handler = default, TimeSpan timeout = default);
        
        Task<BuildResult> RunAsync(IProcess process, Action<Output>? handler = default, CancellationToken cancellationToken = default);
    }
}