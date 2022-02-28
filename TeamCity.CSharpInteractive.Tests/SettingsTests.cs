namespace TeamCity.CSharpInteractive.Tests;

public class SettingsTests
{
    private readonly Mock<IEnvironment> _environment = new Mock<IEnvironment>();
    private readonly Mock<ICommandLineParser> _commandLineParser = new Mock<ICommandLineParser>();
    private readonly ICodeSource _consoleCodeSource = Mock.Of<ICodeSource>();
    private readonly Mock<Func<string, ICodeSource>> _fileCodeSourceFactory = new Mock<Func<string, ICodeSource>>();

    [Fact]
    public void ShouldProvideSettingsWhenScriptMode()
    {
        // Given
        var settings = CreateInstance(RunningMode.Tool);
        var codeSource = Mock.Of<ICodeSource>();
        _fileCodeSourceFactory.Setup(i => i("myScript")).Returns(codeSource);

        // When
        _environment.Setup(i => i.GetCommandLineArgs()).Returns(new[] {"arg0", "arg1", "arg2"});
        _commandLineParser.Setup(i => i.Parse(new[] {"arg1", "arg2"}, CommandLineArgumentType.ScriptFile)).Returns(
            new[]
            {
                new CommandLineArgument(CommandLineArgumentType.Version),
                new CommandLineArgument(CommandLineArgumentType.NuGetSource, "Src1"),
                new CommandLineArgument(CommandLineArgumentType.ScriptFile, "myScript"),
                new CommandLineArgument(CommandLineArgumentType.ScriptArgument, "Arg1"),
                new CommandLineArgument(CommandLineArgumentType.NuGetSource, "Src2"),
                new CommandLineArgument(CommandLineArgumentType.ScriptArgument, "Arg2")
            });

        // Then
        settings.VerbosityLevel.ShouldBe(VerbosityLevel.Normal);
        settings.InteractionMode.ShouldBe(InteractionMode.NonInteractive);
        settings.ShowVersionAndExit.ShouldBeTrue();
        settings.CodeSources.ToArray().ShouldBe(new[] {codeSource});
        settings.NuGetSources.ToArray().ShouldBe(new[] {"Src1", "Src2"});
        settings.ScriptArguments.ToArray().ShouldBe(new[] {"Arg1", "Arg2"});
    }

    [Fact]
    public void ShouldProvideSettingsWhenApplication()
    {
        // Given
        var settings = CreateInstance(RunningMode.Application);
        var codeSource = Mock.Of<ICodeSource>();
        _fileCodeSourceFactory.Setup(i => i("myScript")).Returns(codeSource);

        // When
        _environment.Setup(i => i.GetCommandLineArgs()).Returns(new[] {"arg0", "arg1", "arg2"});
        _commandLineParser.Setup(i => i.Parse(new[] {"arg1", "arg2"}, CommandLineArgumentType.ScriptArgument)).Returns(
            new[]
            {
                new CommandLineArgument(CommandLineArgumentType.Version),
                new CommandLineArgument(CommandLineArgumentType.NuGetSource, "Src1"),
                new CommandLineArgument(CommandLineArgumentType.ScriptArgument, "Arg1"),
                new CommandLineArgument(CommandLineArgumentType.NuGetSource, "Src2"),
                new CommandLineArgument(CommandLineArgumentType.ScriptArgument, "Arg2")
            });

        // Then
        settings.VerbosityLevel.ShouldBe(VerbosityLevel.Normal);
        settings.InteractionMode.ShouldBe(InteractionMode.NonInteractive);
        settings.ShowVersionAndExit.ShouldBeTrue();
        settings.CodeSources.ShouldBeEmpty();
        settings.NuGetSources.ToArray().ShouldBe(new[] {"Src1", "Src2"});
        settings.ScriptArguments.ToArray().ShouldBe(new[] {"Arg1", "Arg2"});
    }

    [Fact]
    public void ShouldProvideSettingsWhenInteractiveMode()
    {
        // Given
        var settings = CreateInstance(RunningMode.Tool);

        // When
        _environment.Setup(i => i.GetCommandLineArgs()).Returns(new[] {"arg0"});

        // Then
        settings.VerbosityLevel.ShouldBe(VerbosityLevel.Quiet);
        settings.InteractionMode.ShouldBe(InteractionMode.Interactive);
        settings.CodeSources.ToArray().ShouldBe(new[] {_consoleCodeSource});
    }

    private Settings CreateInstance(RunningMode runningMode) =>
        new(runningMode, _environment.Object, _commandLineParser.Object, _consoleCodeSource, _fileCodeSourceFactory.Object);
}