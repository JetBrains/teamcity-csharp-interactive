namespace TeamCity.CSharpInteractive.Tests;

using HostApi;
using NuGet.Versioning;

public class NuGetReferenceResolverTests
{
    private readonly Mock<ILog<NuGetReferenceResolver>> _log;
    private readonly Mock<INuGetEnvironment> _nugetEnv;
    private readonly Mock<INuGetRestoreService> _nugetRestoreService;
    private readonly Mock<INuGetAssetsReader> _nugetAssetsReader;
    private readonly Mock<ICleaner> _cleaner;
    private readonly AddNuGetReferenceCommand _command;
    private readonly Mock<IDisposable> _trackToken;
    private static readonly string[] Sources = {"src"};
    private static readonly string[] FallbackFolders = {"fallback"};
    private const string PackagesPath = "packages";
    private readonly ReferencingAssembly _referencingAssembly1 = new("Abc1", "Abc1.dll");
    private readonly ReferencingAssembly _referencingAssembly2 = new("Abc2", "Abc2.dll");

    public NuGetReferenceResolverTests()
    {
        _command = new AddNuGetReferenceCommand("Abc", new VersionRange(new NuGetVersion(1, 2, 3)));

        _log = new Mock<ILog<NuGetReferenceResolver>>();
        _log.Setup(i => i.Info(It.IsAny<Text[]>()));

        _nugetEnv = new Mock<INuGetEnvironment>();
        _nugetEnv.SetupGet(i => i.Sources).Returns(Sources);
        _nugetEnv.SetupGet(i => i.FallbackFolders).Returns(FallbackFolders);
        _nugetEnv.SetupGet(i => i.PackagesPath).Returns(PackagesPath);

        _nugetRestoreService = new Mock<INuGetRestoreService>();
        var projectAssetsJson = Path.Combine("TMP", "project.assets.json");
        var settings = new NuGetRestoreSettings(
            _command.PackageId,
            Sources,
            FallbackFolders,
            _command.VersionRange,
            default,
            PackagesPath);
        _nugetRestoreService.Setup(i => i.TryRestore(settings, out projectAssetsJson)).Returns(true);

        _nugetAssetsReader = new Mock<INuGetAssetsReader>();
        _nugetAssetsReader.Setup(i => i.ReadReferencingAssemblies(projectAssetsJson)).Returns(new[] {_referencingAssembly1, _referencingAssembly2});

        _trackToken = new Mock<IDisposable>();
        _cleaner = new Mock<ICleaner>();
        _cleaner.Setup(i => i.Track("TMP")).Returns(_trackToken.Object);
    }

    [Fact]
    public void ShouldRestore()
    {
        // Given
        var resolver = CreateInstance();

        // When
        var result = resolver.TryResolveAssemblies(_command.PackageId, _command.VersionRange, out var assemblies);

        // Then
        result.ShouldBeTrue();
        assemblies.ShouldBe(new []{_referencingAssembly1, _referencingAssembly2});
        _trackToken.Verify(i => i.Dispose());
    }

    [Fact]
    public void ShouldFailWhenRestoreFail()
    {
        // Given
        var resolver = CreateInstance();

        // When
        var projectAssetsJson = Path.Combine("TMP", "project.assets.json");
        var settings = new NuGetRestoreSettings(
            _command.PackageId,
            Sources,
            FallbackFolders,
            _command.VersionRange,
            default,
            PackagesPath);
        _nugetRestoreService.Setup(i => i.TryRestore(settings, out projectAssetsJson)).Returns(false);
        var result = resolver.TryResolveAssemblies(_command.PackageId, _command.VersionRange, out _);

        // Then
        result.ShouldBeFalse();
    }

    [Fact]
    public void ShouldRestoreWhenHasNoAssemblies()
    {
        // Given
        var resolver = CreateInstance();

        // When
        var projectAssetsJson = Path.Combine("TMP", "project.assets.json");
        _nugetAssetsReader.Setup(i => i.ReadReferencingAssemblies(projectAssetsJson)).Returns(Enumerable.Empty<ReferencingAssembly>());
        var result = resolver.TryResolveAssemblies(_command.PackageId, _command.VersionRange, out var assemblies);

        // Then
        result.ShouldBeTrue();
        assemblies.ShouldBeEmpty();
        _trackToken.Verify(i => i.Dispose());
    }

    private NuGetReferenceResolver CreateInstance() =>
        new(
            _log.Object,
            _nugetEnv.Object,
            _nugetRestoreService.Object,
            _nugetAssetsReader.Object,
            _cleaner.Object);
}