namespace TeamCity.CSharpInteractive.Tests;

using NuGet.Versioning;
using Xunit;

public class ScriptContentReplacerTests
{
    private readonly Mock<INuGetReferenceResolver> _nuGetReferenceResolver = new();
    private readonly Mock<ICommandFactory<ICodeSource>> _commandFactory = new();
    private readonly Mock<IRuntimeExplorer> _runtimeExplorer = new();
    private readonly Mock<ICommandsRunner> _commandsRunner = new();
    private readonly Mock<IFileSystem> _fileSystem = new();
    private readonly Mock<IUniqueNameGenerator> _uniqueNameGenerator = new();
    private readonly Mock<IEnvironment> _environment = new();
    private readonly Mock<Func<string, ICodeSource>> _codeSourceFactory = new();
    private readonly ReferencingAssembly _referencingAssembly1 = new("Abc1", "Abc1.dll");
    private readonly ReferencingAssembly _referencingAssembly2 = new("Abc2", "Abc2.dll");

    [Fact]
    public void ShouldReplace()
    {
        // Given
        var replacer = CreateInstance();

        _environment.Setup(i => i.GetPath(SpecialFolder.Temp)).Returns("Tmp");
        _uniqueNameGenerator.Setup(i => i.Generate()).Returns("TempName");
        var scriptFile = Path.Combine("Tmp", "TempName");
        
        var code1Source = Mock.Of<ICodeSource>();
        _codeSourceFactory.Setup(i => i("code1")).Returns(code1Source);
        _commandFactory.Setup(i => i.Create(code1Source)).Returns(Enumerable.Empty<ICommand>());
        
        var code2Source = Mock.Of<ICodeSource>();
        _codeSourceFactory.Setup(i => i("code2")).Returns(code2Source);
        _commandFactory.Setup(i => i.Create(code2Source)).Returns(Enumerable.Empty<ICommand>());
        
        var addNuGetReferenceCommand = new AddNuGetReferenceCommand("PackId", VersionRange.Parse("1.2.3"));
        var refSource = Mock.Of<ICodeSource>();
        _codeSourceFactory.Setup(i => i("ref")).Returns(refSource);
        _commandFactory.Setup(i => i.Create(refSource)).Returns(new [] {addNuGetReferenceCommand});
        IReadOnlyCollection<ReferencingAssembly> assemblies = new []{_referencingAssembly1, _referencingAssembly2};
        _nuGetReferenceResolver.Setup(i => i.TryResolveAssemblies(addNuGetReferenceCommand.PackageId, addNuGetReferenceCommand.VersionRange, out assemblies)).Returns(true);
        var runtimeAssemblyPath = "Abc2Runtime.dll";
        _runtimeExplorer.Setup(i => i.TryFindRuntimeAssembly(_referencingAssembly1.FilePath, out runtimeAssemblyPath)).Returns(false);
        _runtimeExplorer.Setup(i => i.TryFindRuntimeAssembly(_referencingAssembly2.FilePath, out runtimeAssemblyPath)).Returns(true);
        
        var commandsSource = Mock.Of<ICodeSource>();
        _codeSourceFactory.Setup(i => i("cmd")).Returns(commandsSource);
        _commandFactory.Setup(i => i.Create(commandsSource)).Returns(new [] {HelpCommand.Shared, HelpCommand.Shared});

        // When
        var replacedScript = replacer.Replace(new[] {"code1", "ref", "cmd", "ref", "code2", "cmd"}).ToArray();

        // Then
        replacedScript.ShouldBe(new []{"code1", $"#load \"{scriptFile}\"", "code2"});
        _fileSystem.Verify(i => i.WriteAllLines(scriptFile, It.Is<IEnumerable<string>>(lines => lines.SequenceEqual(new []{"#r \"Abc1.dll\"", "#r \"Abc2Runtime.dll\""}))));
        _commandsRunner.Verify(i => i.Run(It.Is<IEnumerable<ICommand>>(commands => commands.SequenceEqual(new []{HelpCommand.Shared, HelpCommand.Shared, HelpCommand.Shared, HelpCommand.Shared}))));
    }

    private ScriptContentReplacer CreateInstance() =>
        new(
            _nuGetReferenceResolver.Object,
            _commandFactory.Object,
            _runtimeExplorer.Object,
            _commandsRunner.Object,
            _fileSystem.Object,
            _uniqueNameGenerator.Object,
            _environment.Object,
            _codeSourceFactory.Object);
}