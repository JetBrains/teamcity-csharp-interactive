// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
// ReSharper disable CheckNamespace
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedMemberInSuper.Global
namespace DotNet
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Cmd;

    public interface IBuild
    {
        BuildResult Run(IProcess process, Action<BuildMessage>? handler = default, TimeSpan timeout = default);
        
        Task<BuildResult> RunAsync(IProcess process, Action<BuildMessage>? handler = default, CancellationToken cancellationToken = default);
    }
}