namespace TeamCity.CSharpInteractive.Tests.Integration.Core
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Cmd;

    internal record DotNetScript: CommandLine
    {
        public static readonly CommandLine Shared = new DotNetScript();
        
        private DotNetScript()
            : base(
                TeamCity.CSharpInteractive.Composer.Resolve<IDotnetEnvironment>().Path,
                Path.Combine(Path.GetDirectoryName(typeof(DotNetScript).Assembly.Location)!, "dotnet-csi.dll"))
        {
        }

        public static CommandLine Create(IEnumerable<string> args, params string[] lines) =>
            Create("script.csx", default, args, lines);

        public static CommandLine Create(params string[] lines) =>
            Create(Enumerable.Empty<string>(), lines);
        
        public static CommandLine Create(string scriptName, string? workingDirectory, IEnumerable<string> args, params string[] lines)
        {
            var fileSystem = Composer.ResolveIFileSystem();
            workingDirectory = workingDirectory ?? GetWorkingDirectory();
            var scriptFile = Path.Combine(workingDirectory, scriptName);
            fileSystem.AppendAllLines(scriptFile, lines);
            return Shared.AddArgs(args.ToArray()).AddArgs(scriptFile).WithWorkingDirectory(workingDirectory);
        }

        public static string GetWorkingDirectory()
        {
            var uniqueNameGenerator = TeamCity.CSharpInteractive.Composer.Resolve<IUniqueNameGenerator>();
            var environment = TeamCity.CSharpInteractive.Composer.Resolve<IEnvironment>();
            var tempDirectory = environment.GetPath(SpecialFolder.Temp);
            return Path.Combine(tempDirectory, uniqueNameGenerator.Generate());
        }
    }
}