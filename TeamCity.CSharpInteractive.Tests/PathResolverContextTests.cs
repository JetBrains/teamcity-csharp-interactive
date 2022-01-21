namespace TeamCity.CSharpInteractive.Tests;

using Script;
using Script.Cmd;

public class PathResolverContextTests
{
    private readonly Mock<IHost> _host = new();

    [Fact]
    public void ShouldUseDefaultContext()
    {
        // Given
        var context = CreateInstance();

        // When
        var resolvedPath = context.Resolve("Abc");

        // Then
        resolvedPath.ShouldBe("Abc");
    }
    
    [Fact]
    public void ShouldUseRegisteredResolver()
    {
        // Given
        var context = CreateInstance();
        var pathResolver = new Mock<IPathResolver>();
        pathResolver.Setup(i => i.Resolve(_host.Object, "Abc", It.IsAny<IPathResolver>())).Returns<IHost, string, IPathResolver>((host, path, next) => "Xyz" + next.Resolve(host, path, Mock.Of<IPathResolver>()));

        // When
        using var token = context.Register(pathResolver.Object);
        var resolvedPath = context.Resolve("Abc");

        // Then
        resolvedPath.ShouldBe("XyzAbc");
    }
    
    [Fact]
    public void ShouldReleaseRegisteredResolver()
    {
        // Given
        var context = CreateInstance();
        var pathResolver = new Mock<IPathResolver>();
        pathResolver.Setup(i => i.Resolve(_host.Object, "Abc", It.IsAny<IPathResolver>())).Returns("Xyz");
        var token = context.Register(pathResolver.Object);

        // When
        token.Dispose();
        var resolvedPath = context.Resolve("Abc");

        // Then
        resolvedPath.ShouldBe("Abc");
    }
    
    [Fact]
    public void ShouldPassNextResolver()
    {
        // Given
        var context = CreateInstance();
        var pathResolver1 = new Mock<IPathResolver>();
        var pathResolver2 = new Mock<IPathResolver>();
        pathResolver2.Setup(i => i.Resolve(_host.Object, "Abc", pathResolver1.Object)).Returns("Xyz");

        // When
        using var token1 = context.Register(pathResolver1.Object);
        using var token2 = context.Register(pathResolver2.Object);
        var resolvedPath = context.Resolve("Abc");

        // Then
        resolvedPath.ShouldBe("Xyz");
    }

    private PathResolverContext CreateInstance() =>
        new(_host.Object);
}