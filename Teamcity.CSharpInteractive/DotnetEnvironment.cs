// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global
namespace Teamcity.CSharpInteractive
{
    using System;
    using Microsoft.DotNet.PlatformAbstractions;
    using Pure.DI;

    internal class DotnetEnvironment : IDotnetEnvironment
    {
        private const string VersionPrefix = ",Version=v";
        private readonly string _frameworkName;
        private readonly IEnvironment _environment;

        public DotnetEnvironment(
            [Tag("FrameworkName")] string frameworkName,
            IEnvironment environment)
        {
            _frameworkName = frameworkName;
            _environment = environment;
        }

        public string Path => System.IO.Path.Combine(_environment.GetPath(SpecialFolder.ProgramFiles), "dotnet");

        public string TargetFrameworkMoniker => _frameworkName;

        public string Tfm => Version.Major >= 5 ? $"net{Version}" : $"netcoreapp{Version}";

        public Version Version => Version.Parse(_frameworkName[(_frameworkName.IndexOf(VersionPrefix, StringComparison.Ordinal) + VersionPrefix.Length)..]);

        public string RuntimeIdentifier =>
            _environment.OperatingSystemPlatform switch
            {
                Platform.Windows => RuntimeEnvironment.GetRuntimeIdentifier(),
                Platform.Darwin => $"osx-{_environment.ProcessArchitecture}",
                _ => $"linux-{_environment.ProcessArchitecture}"
            };
    }
}