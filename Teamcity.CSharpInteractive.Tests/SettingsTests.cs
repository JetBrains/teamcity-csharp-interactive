namespace Teamcity.CSharpInteractive.Tests
{
    using System.Linq;
    using Moq;
    using Shouldly;
    using Xunit;

    public class SettingsTests
    {
        private readonly Mock<IEnvironment> _environment;
        private readonly ICodeSource _consoleCodeSource;
        private readonly Mock<IFileCodeSourceFactory> _fileCodeSourceFactory;

        public SettingsTests()
        {
            _environment = new Mock<IEnvironment>();
            _consoleCodeSource = Mock.Of<ICodeSource>();
            _fileCodeSourceFactory = new Mock<IFileCodeSourceFactory>();
        }

        [Fact]
        public void ShouldProvideSettingsWhenScriptMode()
        {
            // Given
            var settings = CreateInstance();
            var codeSource1 = Mock.Of<ICodeSource>();
            var codeSource2 = Mock.Of<ICodeSource>();
            _fileCodeSourceFactory.Setup(i => i.Create("arg1", true)).Returns(codeSource1);
            _fileCodeSourceFactory.Setup(i => i.Create("arg2", true)).Returns(codeSource2);

            // When
            _environment.Setup(i => i.GetCommandLineArgs()).Returns(new[] { "arg0", "arg1", "arg2"});
            settings.Load();

            // Then
            settings.VerbosityLevel.ShouldBe(VerbosityLevel.Normal);
            settings.InteractionMode.ShouldBe(InteractionMode.Script);
            settings.Sources.ToArray().ShouldBe(new []{codeSource1, codeSource2});
        }
        
        [Fact]
        public void ShouldProvideSettingsWhenInteractiveMode()
        {
            // Given
            var settings = CreateInstance();
            
            // When
            _environment.Setup(i => i.GetCommandLineArgs()).Returns(new[] { "arg0" });
            settings.Load();

            // Then
            settings.VerbosityLevel.ShouldBe(VerbosityLevel.Quit);
            settings.InteractionMode.ShouldBe(InteractionMode.Interactive);
            settings.Sources.ToArray().ShouldBe(new []{_consoleCodeSource});
        }

        private Settings CreateInstance() =>
            new(_environment.Object, _consoleCodeSource, _fileCodeSourceFactory.Object);
    }
}