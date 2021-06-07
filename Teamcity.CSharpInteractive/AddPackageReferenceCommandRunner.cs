// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System;
    using System.IO;
    using System.Linq;

    internal class AddPackageReferenceCommandRunner: ICommandRunner
    {
        private readonly IEnvironment _environment;
        private readonly IUniqueNameGenerator _uniqueNameGenerator;
        private readonly INugetEnvironment _nugetEnvironment;
        private readonly INugetRestoreService _nugetRestoreService;
        private readonly INugetAssetsReader _nugetAssetsReader;
        private readonly Func<ICommandsRunner> _commandsRunnerFactory;
        private readonly ICleaner _cleaner;

        public AddPackageReferenceCommandRunner(
            IEnvironment environment,
            IUniqueNameGenerator uniqueNameGenerator,
            INugetEnvironment nugetEnvironment,
            INugetRestoreService nugetRestoreService,
            INugetAssetsReader nugetAssetsReader,
            Func<ICommandsRunner> commandsRunnerFactory,
            ICleaner cleaner)
        {
            _environment = environment;
            _uniqueNameGenerator = uniqueNameGenerator;
            _nugetEnvironment = nugetEnvironment;
            _nugetRestoreService = nugetRestoreService;
            _nugetAssetsReader = nugetAssetsReader;
            _commandsRunnerFactory = commandsRunnerFactory;
            _cleaner = cleaner;
        }

        public CommandResult TryRun(ICommand command)
        {
            if (command.Kind != CommandKind.AddPackageReference || command is not AddPackageReferenceCommand addPackageReferenceCommand)
            {
                return new CommandResult(command, default);
            }

            var tempDirectory = _environment.GetPath(SpecialFolder.Temp);
            var outputPath = Path.Combine(tempDirectory, _uniqueNameGenerator.Generate());
            var packagesPath = Path.Combine(tempDirectory, ".nuget");
            var restoreResult = _nugetRestoreService.Restore(
                addPackageReferenceCommand.PackageId,
                addPackageReferenceCommand.Version,
                _nugetEnvironment.Sources,
                _nugetEnvironment.FallbackFolders,
                outputPath,
                packagesPath);

            if (!restoreResult)
            {
                return new CommandResult(command, false);
            }

            using var outputPathToken = _cleaner.Track(outputPath);
            var assetsFilePath = Path.Combine(outputPath, "project.assets.json");
            var commands =
                from assembly in _nugetAssetsReader.ReadAssemblies(assetsFilePath)
                select new ScriptCommand(string.Empty, $"#r \"{assembly.FilePath}\"");

            foreach (var result in _commandsRunnerFactory().Run(commands))
            {
                if (result.Success.HasValue && !result.Success.Value)
                {
                    break;
                }
            }
            
            return new CommandResult(command, true);
        }
    }
}