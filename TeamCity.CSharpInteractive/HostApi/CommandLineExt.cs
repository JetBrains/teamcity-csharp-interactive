// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
namespace HostApi;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public static class CommandLineExt
{
    public static int? Run(this ICommandLine commandLine, Action<Output>? handler = default, TimeSpan timeout = default) => 
        Host.GetService<ICommandLineRunner>().Run(commandLine, handler, timeout);

    public static Task<int?> RunAsync(this ICommandLine commandLine, Action<Output>? handler = default, CancellationToken cancellationToken = default) =>
        Host.GetService<ICommandLineRunner>().RunAsync(commandLine, handler, cancellationToken);
    
    public static IBuildResult Build(this ICommandLine commandLine, Action<BuildMessage>? handler = default, TimeSpan timeout = default) => 
        Host.GetService<IBuildRunner>().Run(commandLine, handler, timeout);

    public static Task<IBuildResult> BuildAsync(this ICommandLine commandLine, Action<BuildMessage>? handler = default, CancellationToken cancellationToken = default) =>
        Host.GetService<IBuildRunner>().RunAsync(commandLine, handler, cancellationToken);
}