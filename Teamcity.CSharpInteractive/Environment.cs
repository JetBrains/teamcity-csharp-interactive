// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.DotNet.PlatformAbstractions;

    [ExcludeFromCodeCoverage]
    internal class Environment : IEnvironment, ITraceSource
    {
        public Platform OperatingSystemPlatform => RuntimeEnvironment.OperatingSystemPlatform;

        public string ProcessArchitecture => RuntimeEnvironment.RuntimeArchitecture;

        public IEnumerable<string> GetCommandLineArgs() => System.Environment.GetCommandLineArgs();

        public string? GetEnvironmentVariable(string name) => System.Environment.GetEnvironmentVariable(name);

        public string GetPath(SpecialFolder specialFolder)
        {
            switch (OperatingSystemPlatform)
            {
                case Platform.Windows:
                    return specialFolder switch
                    {
                        SpecialFolder.Temp => System.Environment.GetEnvironmentVariable("TMP") ?? ".",
                        SpecialFolder.ProgramFiles => System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles),
                        _ => throw new ArgumentOutOfRangeException(nameof(specialFolder), specialFolder, null)
                    };

                case Platform.Unknown:
                case Platform.Linux:
                case Platform.Darwin:
                case Platform.FreeBSD:
                    return specialFolder switch
                    {
                        SpecialFolder.Temp => System.Environment.GetEnvironmentVariable("TMP") ?? ".",
                        SpecialFolder.ProgramFiles => "usr/local/share",
                        _ => throw new ArgumentOutOfRangeException(nameof(specialFolder), specialFolder, null)
                    };
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Exit(ExitCode exitCode) => System.Environment.Exit((int)exitCode);

        public IEnumerable<Text> GetTrace()
        {
            yield return new Text($"OperatingSystemPlatform: {OperatingSystemPlatform}");
            yield return new Text($"ProcessArchitecture: {ProcessArchitecture}");
            foreach (var specialFolder in Enum.GetValues(typeof(SpecialFolder)).OfType<SpecialFolder>())
            {
                yield return new Text($"Path({specialFolder}): {GetPath(specialFolder)}");
            }
        }
    }
}