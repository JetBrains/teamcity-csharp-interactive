namespace Teamcity.CSharpInteractive.Tests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Host;
    using Moq;
    using Shouldly;
    using Xunit;

    public class HostIntegrationCodeSourceTests
    {
        private readonly Mock<IEnvironment> _environment;
        private readonly Mock<ISession> _session;

        public HostIntegrationCodeSourceTests()
        {
            _environment = new Mock<IEnvironment>();
            _session = new Mock<ISession>();
        }

        [Fact]
        public void ShouldProvideSourceCode()
        {
            // Given
            _environment.Setup(i => i.GetPath(SpecialFolder.Bin)).Returns("Bin");
            _session.Setup(i => i.Port).Returns(123);
            var instance = CreateInstance();

            // When
            var actualCode = instance.ToList();

            // Then
            actualCode.ShouldBe(
                new List<string>
                {
                    $"#r \"{Path.Combine("Bin", "Teamcity.Host.dll")}\"",
                    HostIntegrationCodeSource.UsingSystem + HostIntegrationCodeSource.UsingStaticHost + HostIntegrationCodeSource.UsingStaticColor + $"{nameof(Host.ScriptInternal_SetPort)}(123);"
                });
        }

        private HostIntegrationCodeSource CreateInstance() =>
            new(_environment.Object, _session.Object);
    }
}