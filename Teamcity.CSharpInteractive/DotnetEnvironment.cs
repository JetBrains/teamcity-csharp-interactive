// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global
namespace Teamcity.CSharpInteractive
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using Microsoft.DotNet.PlatformAbstractions;
    using Pure.DI;
    using RuntimeEnvironment = Microsoft.DotNet.PlatformAbstractions.RuntimeEnvironment;

    internal class DotnetEnvironment : IDotnetEnvironment, ITraceSource
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

        public IEnumerable<Text> GetTrace()
        {
            yield return Text.NewLine;
            yield return new Text($"FrameworkDescription: {RuntimeInformation.FrameworkDescription}");
            yield return Text.NewLine;
            yield return new Text($"Default C# version: {ScriptCommandFactory.ParseOptions.LanguageVersion}");
            yield return Text.NewLine;
            yield return new Text($"DotnetPath: {Path}");
            yield return Text.NewLine;
            yield return new Text($"TargetFrameworkMoniker: {TargetFrameworkMoniker}");
            yield return Text.NewLine;
            yield return new Text($"Tfm: {Tfm}");
            yield return Text.NewLine;
            yield return new Text($"DotnetVersion: {Version}");
            yield return Text.NewLine;
            yield return new Text($"DotnetRuntimeIdentifier: {RuntimeIdentifier}");
        }
    }
}