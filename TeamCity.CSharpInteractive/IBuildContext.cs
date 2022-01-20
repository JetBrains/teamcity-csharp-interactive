// ReSharper disable InconsistentNaming
namespace TeamCity.CSharpInteractive;

using Cmd;
using DotNet;
using JetBrains.TeamCity.ServiceMessages;

internal interface IBuildContext
{
    IReadOnlyList<BuildMessage> ProcessMessage(in Output output, IServiceMessage message);
        
    IReadOnlyList<BuildMessage> ProcessOutput(in Output output);

    IResult Create(IStartInfo startInfo, int? exitCode);
}