// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global
namespace TeamCity.CSharpInteractive
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using Pure.DI;

    internal class DotnetEnvironment : IDotnetEnvironment, ITraceSource
    {
        private readonly IEnvironment _environment;

        public DotnetEnvironment(
            [Tag("TargetFrameworkMoniker")] string targetFrameworkMoniker,
            IEnvironment environment)
        {
            TargetFrameworkMoniker = targetFrameworkMoniker;
            _environment = environment;
        }

        public string Path => System.IO.Path.Combine(_environment.GetPath(SpecialFolder.ProgramFiles), "dotnet");

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