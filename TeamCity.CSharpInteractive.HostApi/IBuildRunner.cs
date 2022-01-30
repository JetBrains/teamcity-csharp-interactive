// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedMemberInSuper.Global
namespace HostApi;

public interface IBuildRunner
{
    IBuildResult Run(ICommandLine commandLine, Action<BuildMessage>? handler = default, TimeSpan timeout = default);

    Task<IBuildResult> RunAsync(ICommandLine commandLine, Action<BuildMessage>? handler = default, CancellationToken cancellationToken = default);
}