namespace TeamCity.CSharpInteractive.Tests;

using NuGet.Versioning;
using Xunit;

public class AddNuGetReferenceCommandRunnerTests
{
    private readonly Mock<ILog<AddNuGetReferenceCommandRunner>> _log = new();
    private readonly Mock<INuGetReferenceResolver> _nuGetReferenceResolver = new();
    private readonly Mock<IReferenceRegistry> _referenceRegistry = new();
    private readonly ReferencingAssembly _referencingAssembly1 = new("Abc1", "Abc1.dll");
    private readonly ReferencingAssembly _referencingAssembly2 = new("Abc2", "Abc2.dll");
    private readonly AddNuGetReferenceCommand _command = new("Abc", new VersionRange(new NuGetVersion(1, 2, 3)));

    public AddNuGetReferenceCommandRunnerTests()
    {
        _referenceRegistry = new Mock<IReferenceRegistry>();
        var referencingAssembly1Description = _referencingAssembly1.Name;
        _referenceRegistry.Setup(i => i.TryRegisterAssembly(_referencingAssembly1.FilePath, out referencingAssembly1Description)).Returns(true);

        var referencingAssembly2Description = _referencingAssembly2.Name;
        _referenceRegistry.Setup(i => i.TryRegisterAssembly(_referencingAssembly2.FilePath, out referencingAssembly2Description)).Returns(true);
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
    public void ShouldReturnFailWhenCannotResolveAssemblies()
    {
        // Given
        var runner = CreateInstance();
        IReadOnlyCollection<ReferencingAssembly> assemblies = Array.Empty<ReferencingAssembly>();
        _nuGetReferenceResolver.Setup(i => i.TryResolveAssemblies(_command.PackageId, _command.VersionRange, out assemblies)).Returns(false);

        // When
        var result = runner.TryRun(_command);

        // Then
        result.ShouldBe(new CommandResult(_command, false));
    }
    
    [Fact]
    public void ShouldRegisterAssemblies()
    {
        // Given
        var runner = CreateInstance();
        IReadOnlyCollection<ReferencingAssembly> assemblies = new []{_referencingAssembly1, _referencingAssembly2};
        _nuGetReferenceResolver.Setup(i => i.TryResolveAssemblies(_command.PackageId, _command.VersionRange, out assemblies)).Returns(true);
        var description = string.Empty;
        _referenceRegistry.Setup(i => i.TryRegisterAssembly(_referencingAssembly1.FilePath, out description)).Returns(true);
        _referenceRegistry.Setup(i => i.TryRegisterAssembly(_referencingAssembly2.FilePath, out description)).Returns(true);

        // When
        var result = runner.TryRun(_command);

        // Then
        result.ShouldBe(new CommandResult(_command, true));
    }
    
    [Fact]
    public void ShouldReturnFailWhenSomeAssemblyWasNotRegistered()
    {
        // Given
        var runner = CreateInstance();
        IReadOnlyCollection<ReferencingAssembly> assemblies = new []{_referencingAssembly1, _referencingAssembly2};
        _nuGetReferenceResolver.Setup(i => i.TryResolveAssemblies(_command.PackageId, _command.VersionRange, out assemblies)).Returns(true);
        var description = string.Empty;
        _referenceRegistry.Setup(i => i.TryRegisterAssembly(_referencingAssembly1.FilePath, out description)).Returns(true);
        _referenceRegistry.Setup(i => i.TryRegisterAssembly(_referencingAssembly2.FilePath, out description)).Returns(false);

        // When
        var result = runner.TryRun(_command);

        // Then
        result.ShouldBe(new CommandResult(_command, false));
    }

    private AddNuGetReferenceCommandRunner CreateInstance() =>
        new(_log.Object, _nuGetReferenceResolver.Object, _referenceRegistry.Object);
}