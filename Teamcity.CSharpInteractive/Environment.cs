// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.DotNet.PlatformAbstractions;

    [ExcludeFromCodeCoverage]
    internal class Environment : IEnvironment
    {
        public Platform OperatingSystemPlatform => RuntimeEnvironment.OperatingSystemPlatform;

        public string ProcessArchitecture => RuntimeEnvironment.RuntimeArchitecture;

        public IEnumerable<string> GetCommandLineArgs() => System.Environment.GetCommandLineArgs();

        public string TempDirectory => @"C:\Projects\a\temp";
    }
}