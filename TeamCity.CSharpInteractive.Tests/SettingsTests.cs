namespace TeamCity.CSharpInteractive.Tests
{
    using System.Linq;
    using Moq;
    using Shouldly;
    using Xunit;

    public class SettingsTests
    {
        private readonly Mock<IEnvironment> _environment;
        private readonly Mock<ICommandLineParser> _commandLineParser;
        private readonly ICodeSource _hostCodeSource;
        private readonly ICodeSource _consoleCodeSource;
        private readonly Mock<IFileCodeSourceFactory> _fileCodeSourceFactory;

        public SettingsTests()
        {
            _environment = new Mock<IEnvironment>();
            _commandLineParser = new Mock<ICommandLineParser>();
            _hostCodeSource = Mock.Of<ICodeSource>();
            _consoleCodeSource = Mock.Of<ICodeSource>();
            _fileCodeSourceFactory = new Mock<IFileCodeSourceFactory>();
        }

        [Fact]
        public void ShouldProvideSettingsWhenScriptMode()
        {
            // Given
            var settings = CreateInstance();
            var codeSource = Mock.Of<ICodeSource>();
            _fileCodeSourceFactory.Setup(i => i.Create("myScript")).Returns(codeSource);

            // When
            _environment.Setup(i => i.GetCommandLineArgs()).Returns(new[] { "arg0", "arg1", "arg2"});
            _commandLineParser.Setup(i => i.Parse(new[] { "arg1", "arg2"})).Returns(
                new []
                {
                    new CommandLineArgument(CommandLineArgumentType.Version),
                    new CommandLineArgument(CommandLineArgumentType.NuGetSource, "Src1"),
                    new CommandLineArgument(CommandLineArgumentType.ScriptFile, "myScript"),
                    new CommandLineArgument(CommandLineArgumentType.ScriptArgument, "Arg1"),
                    new CommandLineArgument(CommandLineArgumentType.NuGetSource, "Src2"),
                    new CommandLineArgument(CommandLineArgumentType.ScriptArgument, "Arg2")
                });
            settings.Load();

            // Then
            settings.VerbosityLevel.ShouldBe(VerbosityLevel.Normal);
            settings.InteractionMode.ShouldBe(InteractionMode.Script);
            settings.ShowVersionAndExit.ShouldBeTrue();
            settings.CodeSources.ToArray().ShouldBe(new []{_hostCodeSource, codeSource});
            settings.NuGetSources.ToArray().ShouldBe(new []{"Src1", "Src2"});
            settings.ScriptArguments.ToArray().ShouldBe(new []{"Arg1", "Arg2"});
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
            settings.VerbosityLevel.ShouldBe(VerbosityLevel.Quiet);
            settings.InteractionMode.ShouldBe(InteractionMode.Interactive);
            settings.CodeSources.ToArray().ShouldBe(new []{_hostCodeSource, _consoleCodeSource});
        }

        private Settings CreateInstance() =>
            new(_environment.Object, _commandLineParser.Object, _hostCodeSource, _consoleCodeSource, _fileCodeSourceFactory.Object);
    }
}