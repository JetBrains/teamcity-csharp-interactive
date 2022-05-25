namespace TeamCity.CSharpInteractive.Tests;

using System.Reflection;
using Microsoft.CodeAnalysis.Scripting;

public class ReferencesScriptOptionsFactoryTests
{
    private readonly Mock<IRuntimeExplorer> _runtimeExplorer = new();
    
    [Fact]
    public void ShouldRegisterAssembly()
    {
        // Given
        var someLocation = Assembly.GetCallingAssembly().Location;
        var factory = CreateInstance();

        // When
        factory.TryRegisterAssembly(someLocation, out var description).ShouldBeTrue();
        var options = factory.Create(ScriptOptions.Default);

        // Then
        description.ShouldNotBeEmpty();
        options.MetadataReferences.Length.ShouldBe(ScriptOptions.Default.MetadataReferences.Length + 1);
    }
    
    [Fact]
    public void ShouldRegisterBothAssembliesWhenRuntime()
    {
        // Given
        var someLocation = Assembly.GetCallingAssembly().Location;
        var factory = CreateInstance();
        var runtimeLocation = typeof(object).Assembly.Location;
        _runtimeExplorer.Setup(i => i.TryFindRuntimeAssembly(someLocation, out runtimeLocation)).Returns(true);

        // When
        factory.TryRegisterAssembly(someLocation, out _).ShouldBeTrue();
        var options = factory.Create(ScriptOptions.Default);

        // Then
        options.MetadataReferences.Length.ShouldBe(ScriptOptions.Default.MetadataReferences.Length + 2);
    }

    [Fact]
    public void ShouldRegisterDuplicatedAssembly()
    {
        // Given
        var someLocation = Assembly.GetCallingAssembly().Location;
        var factory = CreateInstance();

        // When
        factory.TryRegisterAssembly(someLocation, out _);
        factory.TryRegisterAssembly(someLocation, out _).ShouldBeTrue();

        // Then
    }

    [Fact]
    public void ShouldCreateOptions()
    {
        // Given
        var someLocation = Assembly.GetCallingAssembly().Location;
        var factory = CreateInstance();
        factory.TryRegisterAssembly(someLocation, out _);

        // When
        var options = factory.Create(ScriptOptions.Default);

        // Then
        options.MetadataReferences.Length.ShouldBe(ScriptOptions.Default.MetadataReferences.Length + 1);
    }

    private ReferencesScriptOptionsFactory CreateInstance() =>
        new(Mock.Of<ILog<ReferencesScriptOptionsFactory>>(), _runtimeExplorer.Object);
}