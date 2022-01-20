namespace TeamCity.CSharpInteractive.Tests;

public class DockerSettingsTests
{
    private readonly Mock<IDockerEnvironment> _dockerEnvironment = new();

    [Fact]
    public void ShouldGetDockerExecutablePath()
    {
        // Given
        var settings = CreateInstance();

        // When
        _dockerEnvironment.SetupGet(i => i.Path).Returns("Bin");

        // Then
        settings.DockerExecutablePath.ShouldBe("Bin");
    }

    private DockerSettings CreateInstance() =>
        new(_dockerEnvironment.Object);
}