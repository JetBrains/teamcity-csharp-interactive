// ReSharper disable InconsistentNaming
namespace TeamCity.CSharpInteractive;

using HostApi;
using JetBrains.TeamCity.ServiceMessages;

internal interface IBuildContext
{
    IReadOnlyList<BuildMessage> ProcessMessage(in Output output, IServiceMessage message);

    IReadOnlyList<BuildMessage> ProcessMessage(IStartInfo startInfo, int processId, IServiceMessage message);

    IReadOnlyList<BuildMessage> ProcessOutput(in Output output);

    IBuildResult Create(IStartInfo startInfo, int? exitCode);
}
