// ReSharper disable InconsistentNaming
namespace TeamCity.CSharpInteractive
{
    using System.Collections.Generic;
    using Cmd;
    using Dotnet;
    using JetBrains.TeamCity.ServiceMessages;

    internal interface IBuildResult
    {
        IEnumerable<BuildMessage> ProcessOutput(IStartInfo startInfo, IReadOnlyList<IServiceMessage> messages);
        
        Dotnet.BuildResult CreateResult(int? exitCode);
    }
}