// ReSharper disable InconsistentNaming
namespace TeamCity.CSharpInteractive
{
    using System.Collections.Generic;
    using Cmd;
    using Dotnet;
    using JetBrains.TeamCity.ServiceMessages;

    internal interface IBuildResult
    {
        IReadOnlyList<BuildMessage> ProcessMessage(in Output output, IServiceMessage message);
        
        IReadOnlyList<BuildMessage> ProcessOutput(in Output output);

        Dotnet.BuildResult Create(IStartInfo startInfo, int? exitCode);
    }
}