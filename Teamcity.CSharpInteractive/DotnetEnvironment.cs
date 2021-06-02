// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global
namespace Teamcity.CSharpInteractive
{
    using System;
    using System.Reflection;
    using System.Runtime.Versioning;
    using Microsoft.DotNet.PlatformAbstractions;

    internal class DotnetEnvironment : IDotnetEnvironment
    {
        private const string VersionPrefix = ",Version=v";
        private static readonly string FrameworkName = Assembly.GetEntryAssembly()?.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName ?? string.Empty;
        private static readonly Version FrameworkVersion = Version.Parse(FrameworkName[(FrameworkName.IndexOf(VersionPrefix, StringComparison.Ordinal) + VersionPrefix.Length)..]);
        private readonly IEnvironment _environment;

        public DotnetEnvironment(IEnvironment environment) =>
            _environment = environment;

        public string Path =>
            System.IO.Path.Combine(
                _environment.OperatingSystemPlatform == Platform.Windows
                    ? System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles)
                    : "usr/local/share",
                "dotnet");

        public string TargetFrameworkMoniker => FrameworkName;

        public string Tfm => Version.Major >= 5 ? $"net{Version}" : $"netcoreapp{Version}";

        public Version Version => FrameworkVersion;

        public string RuntimeIdentifier =>
            _environment.OperatingSystemPlatform switch
            {
                Platform.Windows => RuntimeEnvironment.GetRuntimeIdentifier(),
                Platform.Darwin => $"osx-{_environment.ProcessArchitecture}",
                _ => $"linux-{_environment.ProcessArchitecture}"
            };
    }
}