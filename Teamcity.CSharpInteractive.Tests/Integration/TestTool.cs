namespace Teamcity.CSharpInteractive.Tests.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using CommandLineArgument = CSharpInteractive.CommandLineArgument;
    using Composer = CSharpInteractive.Composer;
    using IFileSystem = CSharpInteractive.IFileSystem;

    internal static class TestTool
    {
        public static IProcessResult Run(IEnumerable<string> args, IEnumerable<string> scriptArgs, params string[] lines)
        {
            var fileSystem = Core.Composer.Resolve<Core.IFileSystem>();
            var scriptFile = fileSystem.CreateTempFilePath();
            try
            {
                fileSystem.AppendAllLines(scriptFile, lines);
                var allArgs = new List<string>(args) { scriptFile };
                allArgs.AddRange(scriptArgs);
                return Core.Composer.Resolve<IProcessRunner>().Run(allArgs.Select(i => new Core.CommandLineArgument(i)), Array.Empty<EnvironmentVariable>());
            }
            finally
            {
                fileSystem.DeleteFile(scriptFile);
            }
        }
        
        public static IProcessResult Run(params string[] lines) =>
            Run(Array.Empty<string>(), Array.Empty<string>(), lines);
    }
}