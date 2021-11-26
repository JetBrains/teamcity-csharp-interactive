// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global
namespace TeamCity.CSharpInteractive
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Runtime.InteropServices;
    using Microsoft.DotNet.PlatformAbstractions;
    using Pure.DI;

    internal class DotnetEnvironment : IDotnetEnvironment, ITraceSource
    {
        public DotnetEnvironment(
            [Tag("TargetFrameworkMoniker")] string targetFrameworkMoniker,
            IEnvironment environment,
            IFileExplorer fileExplorer)
        {
            TargetFrameworkMoniker = targetFrameworkMoniker;
            Path = environment.OperatingSystemPlatform == Platform.Windows ? "dotnet.exe" : "dotnet";
            try
            {
                var processFileName = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName ?? string.Empty;
                if (System.IO.Path.GetFileName(processFileName).Equals(Path, StringComparison.InvariantCultureIgnoreCase))
                {
                    Path = processFileName;
                }

                Path = fileExplorer.FindFiles(Path).FirstOrDefault() ?? Path;
            }
            catch
            {
                // ignored
            }
        }
        
        public string Path { get; }

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