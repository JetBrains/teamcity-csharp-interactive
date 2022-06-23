// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using System.Text.RegularExpressions;

internal class CommandLineParser : ICommandLineParser
{
    private static readonly Regex PropertyRegex = new(@"^(--property|-p|/property|/p):(.+)$", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
    private static readonly char[] PropertiesSeparators = {';', ','};
    private readonly IFileSystem _fileSystem;
    private readonly IMSBuildArgumentsTool _msBuildArgumentsTool;

    public CommandLineParser(
        IFileSystem fileSystem,
        IMSBuildArgumentsTool msBuildArgumentsTool)
    {
        _fileSystem = fileSystem;
        _msBuildArgumentsTool = msBuildArgumentsTool;
    }

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

                var argument = _msBuildArgumentsTool.Unescape(enumerator.Current);
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
                            foreach (var propertyArgument in CreatePropertyArguments(argument))
                            {
                                yield return propertyArgument;
                            }
                            
                            argumentType = null;
                            continue;
                    }
                }
                else
                {
                    if (argument.StartsWith('@') && !argument.StartsWith("@@"))
                    {
                        enumerators.Insert(0, _fileSystem.ReadAllLines(argument[1..]).GetEnumerator());
                        continue;
                    }

                    switch (argument.ToLowerInvariant().Trim())
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
                        case "-property":
                        case "/property":
                        case "-p":
                        case "/p":
                            argumentType = CommandLineArgumentType.ScriptProperty;
                            continue;

                        case "--":
                            argumentType = defaultArgType;
                            continue;

                        default:
                            if (argumentType != defaultArgType)
                            {
                                var propertyMatch = PropertyRegex.Match(argument);
                                if (propertyMatch.Success)
                                {
                                    foreach (var propertyArgument in CreatePropertyArguments(propertyMatch.Groups[2].Value))
                                    {
                                        yield return propertyArgument;
                                    }
                                    
                                    argumentType = null;
                                    continue;
                                }
                            }

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

    private static IEnumerable<CommandLineArgument> CreatePropertyArguments(string argument) =>
        from arg in argument.Split(PropertiesSeparators, StringSplitOptions.RemoveEmptyEntries) 
        select arg.Split('=', 2)
        into parts
        where parts.Length > 0 
        select new CommandLineArgument(CommandLineArgumentType.ScriptProperty, parts.Length > 1 ? parts[1] : string.Empty, parts[0]);
}