// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global
namespace TeamCity.CSharpInteractive
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using Pure.DI;

    internal class DotnetEnvironment : IDotnetEnvironment, ITraceSource
    {
        private readonly Lazy<string> _path = new(() => System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName ?? string.Empty);
        
        public DotnetEnvironment([Tag("TargetFrameworkMoniker")] string targetFrameworkMoniker) =>
            TargetFrameworkMoniker = targetFrameworkMoniker;

        public string Path => _path.Value;

        public string TargetFrameworkMoniker { get; }
        
        [ExcludeFromCodeCoverage]
        public IEnumerable<Text> GetTrace()
        {
            yield return new Text($"FrameworkDescription: {RuntimeInformation.FrameworkDescription}");
            yield return new Text($"Default C# version: {ScriptCommandFactory.ParseOptions.LanguageVersion}");
            yield return new Text($"DotnetPath: {Path}");
            yield return new Text($"TargetFrameworkMoniker: {TargetFrameworkMoniker}");
        }
    }
}