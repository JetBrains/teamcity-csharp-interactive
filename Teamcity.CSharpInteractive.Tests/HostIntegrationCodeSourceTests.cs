namespace Teamcity.CSharpInteractive.Tests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Moq;
    using Shouldly;
    using Xunit;

    public class HostIntegrationCodeSourceTests
    {
        private readonly Mock<IEnvironment> _environment;

        public HostIntegrationCodeSourceTests()
        {
            _environment = new Mock<IEnvironment>();
        }

        [Fact]
        public void ShouldProvideSourceCode()
        {
            // Given
            _environment.Setup(i => i.GetPath(SpecialFolder.Bin)).Returns("Bin");
            var instance = CreateInstance();

            // When
            var actualCode = instance.ToList();

            // Then
            actualCode.ShouldBe(
                new List<string?>
                {
                    $"#r \"{Path.Combine("Bin", "Teamcity.CSharpInteractive.Contracts.dll")}\"",
                    HostIntegrationCodeSource.UsingStatic
                });
        }

        private HostIntegrationCodeSource CreateInstance() =>
            new(_environment.Object);
    }
}