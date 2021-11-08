namespace TeamCity.CSharpInteractive.Tests.Integration.Core
{
    using System.Collections.Generic;
    using System.Linq;
    using Contracts;

    internal record DotNetScript: CommandLine
    {
        public static readonly CommandLine Shared = new DotNetScript();
        
        private DotNetScript()
            : base("dotnet", "dotnet-csi.dll")
        {
        }

        public static CommandLine Create(IEnumerable<string> args, params string[] lines)
        {
            var fileSystem = Composer.ResolveIFileSystem();
            var scriptFile = fileSystem.CreateTempFilePath();
            fileSystem.AppendAllLines(scriptFile, lines);
            return Shared.AddArgs(args.ToArray()).AddArgs(scriptFile);
        }
        
        public static CommandLine Create(params string[] lines) =>
            Create(Enumerable.Empty<string>(), lines);
    }
}