// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System;
    using System.IO;
    using System.Linq;

    internal class AddPackageReferenceCommandRunner: ICommandRunner
    {
        private readonly ILog<AddPackageReferenceCommandRunner> _log;
        private readonly IEnvironment _environment;
        private readonly IUniqueNameGenerator _uniqueNameGenerator;
        private readonly INugetEnvironment _nugetEnvironment;
        private readonly INugetRestoreService _nugetRestoreService;
        private readonly INugetAssetsReader _nugetAssetsReader;
        private readonly Func<ICommandsRunner> _commandsRunnerFactory;
        private readonly ICleaner _cleaner;

        public AddPackageReferenceCommandRunner(
            ILog<AddPackageReferenceCommandRunner> log,
            IEnvironment environment,
            IUniqueNameGenerator uniqueNameGenerator,
            INugetEnvironment nugetEnvironment,
            INugetRestoreService nugetRestoreService,
            INugetAssetsReader nugetAssetsReader,
            Func<ICommandsRunner> commandsRunnerFactory,
            ICleaner cleaner)
        {
            _log = log;
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
            if (command is not AddPackageReferenceCommand addPackageReferenceCommand)
            {
                return new CommandResult(command, default);
            }

            using var restoreToken = _log.Block($"Restore {addPackageReferenceCommand.PackageId} {addPackageReferenceCommand.Version}");
            var tempDirectory = _environment.GetPath(SpecialFolder.Temp);
            var outputPath = Path.Combine(tempDirectory, _uniqueNameGenerator.Generate());
            var restoreResult = _nugetRestoreService.Restore(
                addPackageReferenceCommand.PackageId,
                addPackageReferenceCommand.Version,
                _nugetEnvironment.Sources,
                _nugetEnvironment.FallbackFolders,
                outputPath,
                _nugetEnvironment.PackagesPath);

            if (!restoreResult)
            {
                return new CommandResult(command, false);
            }

            using var outputPathToken = _cleaner.Track(outputPath);
            var assetsFilePath = Path.Combine(outputPath, "project.assets.json");
            var commands =
                from assembly in _nugetAssetsReader.ReadAssemblies(assetsFilePath)
                select new ScriptCommand(assembly.Name, $"#r \"{assembly.FilePath}\"");

            using var addRefsToken = _log.Block($"Add references");
            foreach (var result in _commandsRunnerFactory().Run(commands))
            {
                _log.Info(result.Command.ToString()!);
                if (result.Success.HasValue && result.Success.Value)
                {
                    continue;
                }

                return new CommandResult(command, false);
            }
            
            return new CommandResult(command, true);
        }
    }
}