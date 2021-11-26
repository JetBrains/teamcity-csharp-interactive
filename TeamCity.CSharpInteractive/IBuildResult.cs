// ReSharper disable InconsistentNaming
namespace TeamCity.CSharpInteractive
{
    using System.Collections.Generic;
    using Cmd;
    using Dotnet;
    using JetBrains.TeamCity.ServiceMessages;

    internal interface IBuildResult
    {
        IEnumerable<BuildMessage> ProcessOutput(CommandLineOutput output);
        
        Dotnet.BuildResult CreateResult(int? exitCode);
    }
}