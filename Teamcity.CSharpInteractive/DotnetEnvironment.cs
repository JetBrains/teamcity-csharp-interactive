// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global
namespace Teamcity.CSharpInteractive
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using Pure.DI;

    internal class DotnetEnvironment : IDotnetEnvironment, ITraceSource
    {
        private const string VersionPrefix = ",Version=v";
        private readonly string _targetFrameworkMoniker;
        private readonly IEnvironment _environment;

        public DotnetEnvironment(
            [Tag("TargetFrameworkMoniker")] string targetFrameworkMoniker,
            IEnvironment environment)
        {
            _targetFrameworkMoniker = targetFrameworkMoniker;
            _environment = environment;
        }

        public string Path => System.IO.Path.Combine(_environment.GetPath(SpecialFolder.ProgramFiles), "dotnet");

        public string TargetFrameworkMoniker => _targetFrameworkMoniker;

        public string Tfm => Version.Major >= 5 ? $"net{Version}" : $"netcoreapp{Version}";

        public Version Version => Version.Parse(_targetFrameworkMoniker[(_targetFrameworkMoniker.IndexOf(VersionPrefix, StringComparison.Ordinal) + VersionPrefix.Length)..]);

        [ExcludeFromCodeCoverage]
        public IEnumerable<Text> GetTrace()
        {
            yield return new Text($"FrameworkDescription: {RuntimeInformation.FrameworkDescription}");
            yield return new Text($"Default C# version: {ScriptCommandFactory.ParseOptions.LanguageVersion}");
            yield return new Text($"DotnetPath: {Path}");
            yield return new Text($"TargetFrameworkMoniker: {TargetFrameworkMoniker}");
            yield return new Text($"Tfm: {Tfm}");
            yield return new Text($"DotnetVersion: {Version}");
        }
    }
}