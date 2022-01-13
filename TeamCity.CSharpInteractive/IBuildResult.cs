// ReSharper disable InconsistentNaming
namespace TeamCity.CSharpInteractive
{
    using System.Collections.Generic;
    using Cmd;
    using Dotnet;
    using JetBrains.TeamCity.ServiceMessages;

    internal interface IBuildResult
    {
        IReadOnlyList<BuildMessage> ProcessMessage(IStartInfo startInfo, IServiceMessage message);
        
        Dotnet.BuildResult Create();
    }
}