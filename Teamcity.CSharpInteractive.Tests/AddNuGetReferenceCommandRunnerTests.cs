namespace Teamcity.CSharpInteractive.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Moq;
    using NuGet.Versioning;
    using Shouldly;
    using Xunit;

    public class AddNuGetReferenceCommandRunnerTests
    {
        private readonly Mock<ILog<AddNuGetReferenceCommandRunner>> _log;
        private readonly Mock<IEnvironment> _env;
        private readonly Mock<IUniqueNameGenerator> _uniqueNameGenerator;
        private readonly Mock<INugetEnvironment> _nugetEnv;
        private readonly Mock<INugetRestoreService> _nugetRestoreService;
        private readonly Mock<INugetAssetsReader> _nugetAssetsReader;
        private readonly Mock<ICommandsRunner> _commandsRunner;
        private readonly Mock<ICleaner> _cleaner;
        private readonly AddNuGetReferenceCommand _command;
        private readonly Mock<IDisposable> _trackToken;
        private const string TempDir = "Tmp";
        private const string UniqName = "123456";
        private static readonly string[] Sources = {"src"};
        private static readonly string[] FallbackFolders = {"fallback"};
        private static readonly string OutputPath = Path.Combine(TempDir, UniqName);
        private const string PackagesPath = "packages";
        private static readonly string AssetsFilePath = Path.Combine(OutputPath, "project.assets.json");
        private readonly ScriptCommand[] _commands;

        public AddNuGetReferenceCommandRunnerTests()
        {
            _command = new AddNuGetReferenceCommand("Abc", new NuGetVersion(1, 2, 3));
            
            _log = new Mock<ILog<AddNuGetReferenceCommandRunner>>();
            _log.Setup(i => i.Block(It.IsAny<Text[]>())).Returns(Disposable.Empty);
            
            _env = new Mock<IEnvironment>();
            _env.Setup(i => i.GetPath(SpecialFolder.Temp)).Returns(TempDir);
                
            _uniqueNameGenerator = new Mock<IUniqueNameGenerator>();
            _uniqueNameGenerator.Setup(i => i.Generate()).Returns(UniqName);
            
            _nugetEnv = new Mock<INugetEnvironment>();
            _nugetEnv.SetupGet(i => i.Sources).Returns(Sources);
            _nugetEnv.SetupGet(i => i.FallbackFolders).Returns(FallbackFolders);
            _nugetEnv.SetupGet(i => i.PackagesPath).Returns(PackagesPath);
            
            _nugetRestoreService = new Mock<INugetRestoreService>();
            _nugetRestoreService.Setup(i => i.Restore(_command.PackageId, _command.Version, Sources, FallbackFolders, OutputPath, PackagesPath)).Returns(true);

            ReferencingAssembly referencingAssembly1 = new("Abc1", "Abc1.dll");
            ReferencingAssembly referencingAssembly2 = new("Abc2", "Abc2.dll");
            
            _nugetAssetsReader = new Mock<INugetAssetsReader>();
            _nugetAssetsReader.Setup(i => i.ReadAssemblies(AssetsFilePath)).Returns(new [] {referencingAssembly1, referencingAssembly2});

            _commands = new[]
            {
                new ScriptCommand(referencingAssembly1.Name, $"#r \"{referencingAssembly1.FilePath}\""),
                new ScriptCommand(referencingAssembly2.Name, $"#r \"{referencingAssembly2.FilePath}\"")
            };
            
            CommandResult[] results = {
                new(_commands[0], true),
                new(_commands[0], true)
            };
            
            _commandsRunner = new Mock<ICommandsRunner>();
            _commandsRunner.Setup(i => i.Run(It.Is<IEnumerable<ICommand>>(j => j.SequenceEqual(_commands)))).Returns(results);

            _trackToken = new Mock<IDisposable>();
            _cleaner = new Mock<ICleaner>();
            _cleaner.Setup(i => i.Track(OutputPath)).Returns(_trackToken.Object);
        }
        
        [Fact]
        public void ShouldSkipWhenNotPassAddPackageReferenceCommand()
        {
            // Given
            var runner = CreateInstance();
            var command = HelpCommand.Shared;

            // When
            var result = runner.TryRun(command);

            // Then
            result.ShouldBe(new CommandResult(command, default));
        }

        [Fact]
        public void ShouldRestore()
        {
            // Given
            var runner = CreateInstance();

            // When
            var result = runner.TryRun(_command);

            // Then
            result.Command.ShouldBe(_command);
            result.Success.ShouldBe(true);
            _trackToken.Verify(i => i.Dispose());
        }
        
        [Fact]
        public void ShouldFailWhenRestoreFail()
        {
            // Given
            var runner = CreateInstance();

            // When
            _nugetRestoreService.Setup(i => i.Restore(_command.PackageId, _command.Version, Sources, FallbackFolders, OutputPath, PackagesPath)).Returns(false);
            var result = runner.TryRun(_command);

            // Then
            result.Command.ShouldBe(_command);
            result.Success.ShouldBe(false);
        }
        
        [Fact]
        public void ShouldRestoreWhenHasNoAssemblies()
        {
            // Given
            var runner = CreateInstance();

            // When
            var result = runner.TryRun(_command);
            _nugetAssetsReader.Setup(i => i.ReadAssemblies(AssetsFilePath)).Returns(Enumerable.Empty<ReferencingAssembly>());

            // Then
            result.Command.ShouldBe(_command);
            result.Success.ShouldBe(true);
            _trackToken.Verify(i => i.Dispose());
        }
        
        [Fact]
        public void ShouldFailWhenCannotAddRef()
        {
            // Given
            var runner = CreateInstance();

            // When
            var results = new[]
            {
                new CommandResult(_commands[0], true),
                new CommandResult(_commands[0], false)
            };
            
            _commandsRunner.Setup(i => i.Run(It.Is<IEnumerable<ICommand>>(j => j.SequenceEqual(_commands)))).Returns(results);
            var result = runner.TryRun(_command);

            // Then
            result.Command.ShouldBe(_command);
            result.Success.ShouldBe(false);
            _trackToken.Verify(i => i.Dispose());
        }
        
        [Fact]
        public void ShouldFailWhenCannotRunAddRef()
        {
            // Given
            var runner = CreateInstance();

            // When
            var results = new[]
            {
                new CommandResult(_commands[0], true),
                new CommandResult(_commands[0], null)
            };
            
            _commandsRunner.Setup(i => i.Run(It.Is<IEnumerable<ICommand>>(j => j.SequenceEqual(_commands)))).Returns(results);
            var result = runner.TryRun(_command);

            // Then
            result.Command.ShouldBe(_command);
            result.Success.ShouldBe(false);
            _trackToken.Verify(i => i.Dispose());
        }
        
        private AddNuGetReferenceCommandRunner CreateInstance() =>
            new(
                _log.Object,
                _env.Object,
                _uniqueNameGenerator.Object,
                _nugetEnv.Object,
                _nugetRestoreService.Object,
                _nugetAssetsReader.Object,
                () => _commandsRunner.Object,
                _cleaner.Object);
    }
}