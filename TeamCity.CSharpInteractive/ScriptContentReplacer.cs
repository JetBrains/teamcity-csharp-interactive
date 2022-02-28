// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using NuGet.Packaging;

internal class ScriptContentReplacer : IScriptContentReplacer
{
    private readonly INuGetReferenceResolver _nuGetReferenceResolver;
    private readonly ICommandFactory<ICodeSource> _commandFactory;
    private readonly IRuntimeExplorer _runtimeExplorer;
    private readonly ICommandsRunner _commandsRunner;
    private readonly IFileSystem _fileSystem;
    private readonly IUniqueNameGenerator _uniqueNameGenerator;
    private readonly IEnvironment _environment;
    private readonly Func<string, ICodeSource> _codeSourceFactory;

    public ScriptContentReplacer(
        INuGetReferenceResolver nuGetReferenceResolver,
        ICommandFactory<ICodeSource> commandFactory,
        IRuntimeExplorer runtimeExplorer,
        ICommandsRunner commandsRunner,
        IFileSystem fileSystem,
        IUniqueNameGenerator uniqueNameGenerator,
        IEnvironment environment,
        [Tag(typeof(LineCodeSource))] Func<string, ICodeSource> codeSourceFactory)
    {
        _nuGetReferenceResolver = nuGetReferenceResolver;
        _commandFactory = commandFactory;
        _runtimeExplorer = runtimeExplorer;
        _commandsRunner = commandsRunner;
        _fileSystem = fileSystem;
        _uniqueNameGenerator = uniqueNameGenerator;
        _environment = environment;
        _codeSourceFactory = codeSourceFactory;
    }

    public IEnumerable<string> Replace(IEnumerable<string> lines)
    {
        var allAssemblies = new HashSet<string>();
        var commandsToRun = new List<ICommand>();
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var assemblies = new HashSet<string>();
            var commands = _commandFactory.Create(_codeSourceFactory(line)).ToArray();
            var repl = false;
            foreach (var command in commands)
            {
                switch (command)
                {
                    case AddNuGetReferenceCommand referenceCommand:
                        repl = true;
                        if (_nuGetReferenceResolver.TryResolveAssemblies(referenceCommand.PackageId, referenceCommand.VersionRange, out var resolvedAssemblies))
                        {
                            foreach (var assembly in resolvedAssemblies.Where(i => !allAssemblies.Contains(i.FilePath)))
                            {
                                // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                                if (_runtimeExplorer.TryFindRuntimeAssembly(assembly.FilePath, out var runtimeAssemblyPath))
                                {
                                    assemblies.Add(runtimeAssemblyPath);
                                }
                                else
                                {
                                    assemblies.Add(assembly.FilePath);
                                }
                            }
                        }
                        break;

                    case ScriptCommand:
                    case CodeCommand:
                        break;

                    default:
                        repl = true;
                        commandsToRun.Add(command);
                        break;
                }
            }

            var newAssemblies = assemblies.Except(allAssemblies).ToArray();
            if (!newAssemblies.Any())
            {
                if (!repl)
                {
                    yield return line;
                }
            }
            else
            {
                allAssemblies.AddRange(newAssemblies);
                var scriptFile = Path.Combine(_environment.GetPath(SpecialFolder.Temp), _uniqueNameGenerator.Generate());
                _fileSystem.WriteAllLines(scriptFile, newAssemblies.Select(i => $"#r \"{i}\""));
                yield return $"#load \"{scriptFile}\"";
            }
        }

        // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
        _commandsRunner.Run(commandsToRun).ToArray();
    }

}