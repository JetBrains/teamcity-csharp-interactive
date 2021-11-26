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

    internal class DockerEnvironment : ITraceSource, IDockerEnvironment
    {
        public DockerEnvironment(
            IEnvironment environment,
            IFileExplorer fileExplorer)
        {
            Path = environment.OperatingSystemPlatform == Platform.Windows ? "docker.exe" : "docker";
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

        [ExcludeFromCodeCoverage]
        public IEnumerable<Text> GetTrace()
        {
            yield return new Text($"DockerPath: {Path}");
        }
    }
}