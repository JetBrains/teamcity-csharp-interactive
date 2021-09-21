namespace TeamCity.CSharpInteractive.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Contracts;
    using Moq;
    using NuGet.Versioning;
    using Shouldly;
    using Xunit;

    public class NuGetServiceTests
    {
        private static readonly NuGetPackage NuGetPackage1 = new("Abc", new Version(1, 2, 3), "package", "AbcPath", "111222");
        private static readonly NuGetPackage NuGetPackage2 = new("Abc.Contracts", new Version(3, 2, 1), "package", "AbcContractsPath", "111233");
        private static readonly IEnumerable<string> Sources = new []{"src1", "src2"};
        private static readonly IEnumerable<string> FallBacks = new []{"fb1", "fb2"};

        private readonly Mock<ILog<NuGetService>> _log;
        private readonly Mock<IFileSystem> _fileSystem;
        private readonly Mock<IEnvironment> _environment;
        private readonly Mock<INugetEnvironment> _nugetEnvironment;
        private readonly Mock<INugetRestoreService> _nugetRestoreService;
        private readonly Mock<INugetAssetsReader> _nugetAssetsReader;
        private readonly Mock<ICleaner> _cleaner;
        private readonly Mock<IDisposable> _trackToken;

        public NuGetServiceTests()
        {
            _log = new Mock<ILog<NuGetService>>();
            _fileSystem = new Mock<IFileSystem>();
            _environment = new Mock<IEnvironment>();
            _nugetEnvironment = new Mock<INugetEnvironment>();
            _nugetRestoreService = new Mock<INugetRestoreService>();
            _nugetAssetsReader = new Mock<INugetAssetsReader>();
            _cleaner = new Mock<ICleaner>();

            _environment.Setup(i => i.GetPath(SpecialFolder.Temp)).Returns("TMP");
            _environment.Setup(i => i.GetPath(SpecialFolder.Working)).Returns("WD");
            _nugetEnvironment.SetupGet(i => i.Sources).Returns(Sources);
            _nugetEnvironment.SetupGet(i => i.FallbackFolders).Returns(FallBacks);
            _trackToken = new Mock<IDisposable>();
            _cleaner = new Mock<ICleaner>();
            _cleaner.Setup(i => i.Track("AssetsTmp")).Returns(_trackToken.Object);
        }

        public static IEnumerable<object?[]> Data => new List<object?[]>
        {
            new object?[] { "myPackages", false, Path.Combine("WD", "myPackages") },
            new object?[] { "myPackages", true, "myPackages" },
            new object?[] { default, true, "defaultPackagesPath" },
            new object?[] { default, true, "defaultPackagesPath" }
        };

        [Theory]
        [MemberData(nameof(Data))]
        public void ShouldRestore(string? packagesPath, bool isPathRooted, string expectedNuGtePackagesDir)
        {
            // Given
            string projectAssetsJson = Path.Combine("AssetsTmp", "assets.json");
            var nuGet = CreateInstance();
            _nugetEnvironment.SetupGet(i => i.PackagesPath).Returns("defaultPackagesPath");
            _fileSystem.Setup(i => i.IsPathRooted(It.IsAny<string>())).Returns(isPathRooted);
            _nugetAssetsReader.Setup(i => i.ReadPackages(expectedNuGtePackagesDir, projectAssetsJson)).Returns(new [] {NuGetPackage1, NuGetPackage2});
            _nugetRestoreService.Setup(i => i.TryRestore(It.IsAny<string>(), It.IsAny<VersionRange?>(), ".NETCoreApp,Version=v3.1", Sources, FallBacks, expectedNuGtePackagesDir, out projectAssetsJson)).Returns(true);

            // When
            var packages = nuGet.Restore("Abc", "1.2.3", ".NETCoreApp,Version=v3.1", packagesPath).ToArray();

            // Then
            _nugetRestoreService.Verify(i => i.TryRestore("Abc", VersionRange.Parse("1.2.3"), ".NETCoreApp,Version=v3.1", Sources, FallBacks, expectedNuGtePackagesDir, out projectAssetsJson));
            packages.ShouldBe(new []{NuGetPackage1, NuGetPackage2});
            _trackToken.Verify(i => i.Dispose());
        }

        private NuGetService CreateInstance() =>
            new(
                _log.Object,
                _fileSystem.Object,
                _environment.Object,
                _nugetEnvironment.Object,
                _nugetRestoreService.Object,
                _nugetAssetsReader.Object,
                _cleaner.Object);
    }
}