namespace TeamCity.CSharpInteractive.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using Moq;
    using NuGet.Versioning;
    using Shouldly;
    using Xunit;

    public class AddNuGetReferenceCommandRunnerTests
    {
        private readonly Mock<ILog<AddNuGetReferenceCommandRunner>> _log;
        private readonly Mock<INugetEnvironment> _nugetEnv;
        private readonly Mock<INugetRestoreService> _nugetRestoreService;
        private readonly Mock<INugetAssetsReader> _nugetAssetsReader;
        private readonly Mock<ICleaner> _cleaner;
        private readonly Mock<IReferenceRegistry> _referenceRegistry;
        private readonly AddNuGetReferenceCommand _command;
        private readonly Mock<IDisposable> _trackToken;
        private static readonly string[] Sources = {"src"};
        private static readonly string[] FallbackFolders = {"fallback"};
        private const string PackagesPath = "packages";
        
        public AddNuGetReferenceCommandRunnerTests()
        {
            _command = new AddNuGetReferenceCommand("Abc", new VersionRange(new NuGetVersion(1, 2, 3)));
            
            _log = new Mock<ILog<AddNuGetReferenceCommandRunner>>();
            _log.Setup(i => i.Block(It.IsAny<Text[]>())).Returns(Disposable.Empty);
            
            _nugetEnv = new Mock<INugetEnvironment>();
            _nugetEnv.SetupGet(i => i.Sources).Returns(Sources);
            _nugetEnv.SetupGet(i => i.FallbackFolders).Returns(FallbackFolders);
            _nugetEnv.SetupGet(i => i.PackagesPath).Returns(PackagesPath);
            
            _nugetRestoreService = new Mock<INugetRestoreService>();
            string projectAssetsJson = Path.Combine("TMP", "project.assets.json");
            _nugetRestoreService.Setup(i => i.TryRestore(_command.PackageId, _command.VersionRange, Sources, FallbackFolders, PackagesPath, out projectAssetsJson)).Returns(true);

            ReferencingAssembly referencingAssembly1 = new("Abc1", "Abc1.dll");
            ReferencingAssembly referencingAssembly2 = new("Abc2", "Abc2.dll");
            
            _nugetAssetsReader = new Mock<INugetAssetsReader>();
            _nugetAssetsReader.Setup(i => i.ReadReferencingAssemblies(projectAssetsJson)).Returns(new [] {referencingAssembly1, referencingAssembly2});
            
            _trackToken = new Mock<IDisposable>();
            _cleaner = new Mock<ICleaner>();
            _cleaner.Setup(i => i.Track("TMP")).Returns(_trackToken.Object);

            _referenceRegistry = new Mock<IReferenceRegistry>();
            var referencingAssembly1Description = referencingAssembly1.Name;
            _referenceRegistry.Setup(i => i.TryRegisterAssembly(referencingAssembly1.FilePath, out referencingAssembly1Description)).Returns(true);
            
            var referencingAssembly2Description = referencingAssembly2.Name;
            _referenceRegistry.Setup(i => i.TryRegisterAssembly(referencingAssembly2.FilePath, out referencingAssembly2Description)).Returns(true);
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
            string projectAssetsJson = Path.Combine("TMP", "project.assets.json");
            _nugetRestoreService.Setup(i => i.TryRestore(_command.PackageId, _command.VersionRange, Sources, FallbackFolders, PackagesPath, out projectAssetsJson)).Returns(false);
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
            string projectAssetsJson = Path.Combine("TMP", "project.assets.json");
            _nugetAssetsReader.Setup(i => i.ReadReferencingAssemblies(projectAssetsJson)).Returns(Enumerable.Empty<ReferencingAssembly>());
            var result = runner.TryRun(_command);

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
            var referencingAssembly2Description = "Error";
            _referenceRegistry.Setup(i => i.TryRegisterAssembly("Abc2.dll", out referencingAssembly2Description)).Returns(false);
            
            var result = runner.TryRun(_command);

            // Then
            result.Command.ShouldBe(_command);
            result.Success.ShouldBe(false);
            _trackToken.Verify(i => i.Dispose());
        }

        private AddNuGetReferenceCommandRunner CreateInstance() =>
            new(
                _log.Object,
                _nugetEnv.Object,
                _nugetRestoreService.Object,
                _nugetAssetsReader.Object,
                _cleaner.Object,
                _referenceRegistry.Object);
    }
}