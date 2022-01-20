namespace TeamCity.CSharpInteractive.Tests;

using System.Reflection;
using Microsoft.CodeAnalysis.Scripting;

public class ReferencesScriptOptionsFactoryTests
{
    [Fact]
    public void ShouldRegisterAssembly()
    {
        // Given
        var someLocation = Assembly.GetCallingAssembly().Location;
        var factory = CreateInstance();

        // When
        factory.TryRegisterAssembly(someLocation, out var description).ShouldBeTrue();
        description.ShouldNotBeEmpty();

        // Then
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

    private static ReferencesScriptOptionsFactory CreateInstance() =>
        new(Mock.Of<ILog<ReferencesScriptOptionsFactory>>());
}