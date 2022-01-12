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
        private readonly string _moduleFile;
        private readonly IEnvironment _environment;
        private readonly IFileExplorer _fileExplorer;
        public DotnetEnvironment(
            [Tag("TargetFrameworkMoniker")] string targetFrameworkMoniker,
            [Tag("ModuleFile")] string moduleFile,
            IEnvironment environment,
            IFileExplorer fileExplorer)
        {
            _moduleFile = moduleFile;
            _environment = environment;
            _fileExplorer = fileExplorer;
            TargetFrameworkMoniker = targetFrameworkMoniker;
        }

        public string Path
        {
            get
            {
                var executable = _environment.OperatingSystemPlatform == Platform.Windows ? "dotnet.exe" : "dotnet";
                try
                {
                    if (System.IO.Path.GetFileName(_moduleFile).Equals(executable, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return _moduleFile;
                    }

                    return _fileExplorer.FindFiles(executable, "DOTNET_HOME").FirstOrDefault() ?? executable;
                }
                catch
                {
                    // ignored
                }

                return executable;
            }
        }

        public string TargetFrameworkMoniker { get; }

        [ExcludeFromCodeCoverage]
        public IEnumerable<Text> Trace
        {
            get
            {
                yield return new Text($"FrameworkDescription: {RuntimeInformation.FrameworkDescription}");
                yield return new Text($"Default C# version: {ScriptCommandFactory.ParseOptions.LanguageVersion}");
                yield return new Text($"DotnetPath: {Path}");
                yield return new Text($"TargetFrameworkMoniker: {TargetFrameworkMoniker}");
            }
        }
    }
}