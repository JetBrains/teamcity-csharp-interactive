// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

internal class CommandLineParser : ICommandLineParser
{
    private readonly IFileSystem _fileSystem;

    public CommandLineParser(IFileSystem fileSystem) =>
        _fileSystem = fileSystem;

    public IEnumerable<CommandLineArgument> Parse(IEnumerable<string> arguments, CommandLineArgumentType defaultArgType)
    {
        var enumerators = new List<IEnumerator<string>> {arguments.GetEnumerator()};
        CommandLineArgumentType? argumentType = null;
        try
        {
            while (enumerators.Count > 0)
            {
                var enumerator = enumerators[0];
                if (!enumerator.MoveNext())
                {
                    enumerator.Dispose();
                    enumerators.RemoveAt(0);
                    continue;
                }

                var argument = enumerator.Current;
                if (argumentType != null)
                {
                    // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                    switch (argumentType)
                    {
                        case CommandLineArgumentType.ScriptFile:
                            yield return new CommandLineArgument(CommandLineArgumentType.ScriptFile, argument);
                            argumentType = CommandLineArgumentType.ScriptArgument;
                            continue;

                        case CommandLineArgumentType.ScriptArgument:
                            yield return new CommandLineArgument(CommandLineArgumentType.ScriptArgument, argument);
                            continue;

                        case CommandLineArgumentType.NuGetSource:
                            yield return new CommandLineArgument(CommandLineArgumentType.NuGetSource, argument);
                            argumentType = null;
                            continue;

                        case CommandLineArgumentType.ScriptProperty:
                            var parts = argument.Split('=', 2);
                            if (parts.Length > 0)
                            {
                                yield return new CommandLineArgument(CommandLineArgumentType.ScriptProperty, parts.Length > 1 ? parts[1] : string.Empty, parts[0]);
                            }

                            argumentType = null;
                            continue;
                    }
                }
                else
                {
                    if (argument.StartsWith('@'))
                    {
                        enumerators.Insert(0, _fileSystem.ReadAllLines(argument[1..]).GetEnumerator());
                        continue;
                    }

                    switch (argument.ToLowerInvariant())
                    {
                        case "--help":
                        case "/help":
                        case "-h":
                        case "/h":
                        case "/?":
                            yield return new CommandLineArgument(CommandLineArgumentType.Help);
                            yield break;

                        case "--version":
                        case "/version":
                            yield return new CommandLineArgument(CommandLineArgumentType.Version);
                            yield break;

                        case "--source":
                        case "/source":
                        case "-s":
                        case "/s":
                            argumentType = CommandLineArgumentType.NuGetSource;
                            continue;

                        case "--property":
                        case "/property":
                        case "-p":
                        case "/p":
                            argumentType = CommandLineArgumentType.ScriptProperty;
                            continue;

                        case "--":
                            argumentType = defaultArgType;
                            continue;

                        default:
                            yield return new CommandLineArgument(defaultArgType, argument);
                            argumentType = CommandLineArgumentType.ScriptArgument;
                            continue;
                    }
                }
            }
        }
        finally
        {
            enumerators.ForEach(i => i.Dispose());
        }
    }
}