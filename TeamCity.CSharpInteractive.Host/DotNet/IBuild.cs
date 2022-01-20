// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
// ReSharper disable CheckNamespace
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedMemberInSuper.Global
namespace DotNet;

using Cmd;

public interface IBuild
{
    IResult Run(IProcess process, Action<BuildMessage>? handler = default, TimeSpan timeout = default);
        
    Task<IResult> RunAsync(IProcess process, Action<BuildMessage>? handler = default, CancellationToken cancellationToken = default);
}