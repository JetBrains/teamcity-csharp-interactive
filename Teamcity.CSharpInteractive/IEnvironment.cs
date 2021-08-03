// ReSharper disable UnusedMemberInSuper.Global
namespace Teamcity.CSharpInteractive
{
    using System.Collections.Generic;
    using Microsoft.DotNet.PlatformAbstractions;

    internal interface IEnvironment
    {
        Platform OperatingSystemPlatform { get; }
        
        string ProcessArchitecture { get; }
        
        IEnumerable<string> GetCommandLineArgs();
        
        string GetPath(SpecialFolder specialFolder);

        void Exit(ExitCode exitCode);
    }
}