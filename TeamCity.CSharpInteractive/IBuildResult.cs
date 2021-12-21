// ReSharper disable InconsistentNaming
namespace TeamCity.CSharpInteractive
{
    using System.Collections.Generic;
    using Cmd;
    using Dotnet;

    internal interface IBuildResult
    {
        IEnumerable<BuildMessage> ProcessOutput(in CommandLineOutput output);
        
        Dotnet.BuildResult CreateResult(int? exitCode);
    }
}