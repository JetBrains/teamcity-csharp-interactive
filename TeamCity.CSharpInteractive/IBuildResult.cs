// ReSharper disable InconsistentNaming
namespace TeamCity.CSharpInteractive
{
    using System.Collections.Generic;
    using Cmd;
    using Dotnet;
    using JetBrains.TeamCity.ServiceMessages;

    internal interface IBuildResult
    {
        IReadOnlyList<BuildMessage> ProcessMessage(IStartInfo startInfo, int processId, IServiceMessage message);
        
        Dotnet.BuildResult Create(IStartInfo startInfo, ProcessState state, int? exitCode);
    }
}