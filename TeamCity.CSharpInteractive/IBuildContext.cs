// ReSharper disable InconsistentNaming
namespace TeamCity.CSharpInteractive;

using JetBrains.TeamCity.ServiceMessages;
using Script.Cmd;
using Script.DotNet;

internal interface IBuildContext
{
    IReadOnlyList<BuildMessage> ProcessMessage(in Output output, IServiceMessage message);
        
    IReadOnlyList<BuildMessage> ProcessOutput(in Output output);

    IBuildResult Create(IStartInfo startInfo, int? exitCode);
}