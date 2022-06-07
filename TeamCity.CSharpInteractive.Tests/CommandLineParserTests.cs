namespace TeamCity.CSharpInteractive.Tests;

public class CommandLineParserTests
{
    private readonly Mock<IFileSystem> _fileSystem;

    public CommandLineParserTests() => _fileSystem = new Mock<IFileSystem>();

    public static IEnumerable<object?[]> Data => new List<object?[]>
    {
        // help
        new object[]
        {
            new[] {"--help"},
            CommandLineArgumentType.ScriptFile,
            new[] {new CommandLineArgument(CommandLineArgumentType.Help)}
        },
        
        new object[]
        {
            new[] {"--help "},
            CommandLineArgumentType.ScriptFile,
            new[] {new CommandLineArgument(CommandLineArgumentType.Help)}
        },

        new object[]
        {
            new[] {"--help", "--version"},
            CommandLineArgumentType.ScriptFile,
            new[] {new CommandLineArgument(CommandLineArgumentType.Help)}
        },

        new object[]
        {
            new[] {"--HelP", "--version"},
            CommandLineArgumentType.ScriptFile,
            new[] {new CommandLineArgument(CommandLineArgumentType.Help)}
        },

        new object[]
        {
            new[] {"-h", "--version"},
            CommandLineArgumentType.ScriptFile,
            new[] {new CommandLineArgument(CommandLineArgumentType.Help)}
        },

        new object[]
        {
            new[] {"/help", "--version"},
            CommandLineArgumentType.ScriptFile,
            new[] {new CommandLineArgument(CommandLineArgumentType.Help)}
        },

        new object[]
        {
            new[] {"/h", "--version"},
            CommandLineArgumentType.ScriptFile,
            new[] {new CommandLineArgument(CommandLineArgumentType.Help)}
        },

        new object[]
        {
            new[] {"/?", "--version"},
            CommandLineArgumentType.ScriptFile,
            new[] {new CommandLineArgument(CommandLineArgumentType.Help)}
        },

        // version
        new object[]
        {
            new[] {"--version"},
            CommandLineArgumentType.ScriptFile,
            new[] {new CommandLineArgument(CommandLineArgumentType.Version)}
        },

        new object[]
        {
            new[] {"--version", "-h"},
            CommandLineArgumentType.ScriptFile,
            new[] {new CommandLineArgument(CommandLineArgumentType.Version)}
        },

        new object[]
        {
            new[] {"--VerSion", "-h"},
            CommandLineArgumentType.ScriptFile,
            new[] {new CommandLineArgument(CommandLineArgumentType.Version)}
        },

        new object[]
        {
            new[] {"/version", "-h"},
            CommandLineArgumentType.ScriptFile,
            new[] {new CommandLineArgument(CommandLineArgumentType.Version)}
        },

        // nuget source
        new object[]
        {
            new[] {"--source", "Src1", "-S", "Src2", "/Source", "Src3", "/S", "Src4"},
            CommandLineArgumentType.ScriptFile,
            new[]
            {
                new CommandLineArgument(CommandLineArgumentType.NuGetSource, "Src1"),
                new CommandLineArgument(CommandLineArgumentType.NuGetSource, "Src2"),
                new CommandLineArgument(CommandLineArgumentType.NuGetSource, "Src3"),
                new CommandLineArgument(CommandLineArgumentType.NuGetSource, "Src4")
            }
        },

        // @file
        new object[]
        {
            new[] {"--source", "Src1", "@rspFile", "/S", "Src4"},
            CommandLineArgumentType.ScriptFile,
            new[]
            {
                new CommandLineArgument(CommandLineArgumentType.NuGetSource, "Src1"),
                new CommandLineArgument(CommandLineArgumentType.NuGetSource, "Src2"),
                new CommandLineArgument(CommandLineArgumentType.NuGetSource, "Src3"),
                new CommandLineArgument(CommandLineArgumentType.NuGetSource, "Src4")
            }
        },

        // Script
        new object[]
        {
            new[] {"--source", "Src1", "scriptFile"},
            CommandLineArgumentType.ScriptFile,
            new[]
            {
                new CommandLineArgument(CommandLineArgumentType.NuGetSource, "Src1"),
                new CommandLineArgument(CommandLineArgumentType.ScriptFile, "scriptFile")
            }
        },

        // Script with arguments
        new object[]
        {
            new[] {"--source", "Src1", "scriptFile", "-v", "Arg 2"},
            CommandLineArgumentType.ScriptFile,
            new[]
            {
                new CommandLineArgument(CommandLineArgumentType.NuGetSource, "Src1"),
                new CommandLineArgument(CommandLineArgumentType.ScriptFile, "scriptFile"),
                new CommandLineArgument(CommandLineArgumentType.ScriptArgument, "-v"),
                new CommandLineArgument(CommandLineArgumentType.ScriptArgument, "Arg 2")
            }
        },

        // Script with arguments when is not Tool
        new object[]
        {
            new[] {"--source", "Src1", "Abc", "-v", "Arg 2"},
            CommandLineArgumentType.ScriptArgument,
            new[]
            {
                new CommandLineArgument(CommandLineArgumentType.NuGetSource, "Src1"),
                new CommandLineArgument(CommandLineArgumentType.ScriptArgument, "Abc"),
                new CommandLineArgument(CommandLineArgumentType.ScriptArgument, "-v"),
                new CommandLineArgument(CommandLineArgumentType.ScriptArgument, "Arg 2")
            }
        },

        new object[]
        {
            new[] {"--", "--source", "Src1", "Abc", "-v", "Arg 2"},
            CommandLineArgumentType.ScriptArgument,
            new[]
            {
                new CommandLineArgument(CommandLineArgumentType.ScriptArgument, "--source"),
                new CommandLineArgument(CommandLineArgumentType.ScriptArgument, "Src1"),
                new CommandLineArgument(CommandLineArgumentType.ScriptArgument, "Abc"),
                new CommandLineArgument(CommandLineArgumentType.ScriptArgument, "-v"),
                new CommandLineArgument(CommandLineArgumentType.ScriptArgument, "Arg 2")
            }
        },

        new object[]
        {
            new[] {"Arg1", "Arg2", "Abc", "-v", "Arg 2"},
            CommandLineArgumentType.ScriptArgument,
            new[]
            {
                new CommandLineArgument(CommandLineArgumentType.ScriptArgument, "Arg1"),
                new CommandLineArgument(CommandLineArgumentType.ScriptArgument, "Arg2"),
                new CommandLineArgument(CommandLineArgumentType.ScriptArgument, "Abc"),
                new CommandLineArgument(CommandLineArgumentType.ScriptArgument, "-v"),
                new CommandLineArgument(CommandLineArgumentType.ScriptArgument, "Arg 2")
            }
        },

        // --
        new object[]
        {
            new[] {"--source", "Src1", "--", "ScriptFile", "Src2"},
            CommandLineArgumentType.ScriptFile,
            new[]
            {
                new CommandLineArgument(CommandLineArgumentType.NuGetSource, "Src1"),
                new CommandLineArgument(CommandLineArgumentType.ScriptFile, "ScriptFile"),
                new CommandLineArgument(CommandLineArgumentType.ScriptArgument, "Src2")
            }
        },
        
        new object[]
        {
            new[] {"--source", "Src1", "--", "ScriptFile", "Src2", "-p:Abc=Xyz"},
            CommandLineArgumentType.ScriptFile,
            new[]
            {
                new CommandLineArgument(CommandLineArgumentType.NuGetSource, "Src1"),
                new CommandLineArgument(CommandLineArgumentType.ScriptFile, "ScriptFile"),
                new CommandLineArgument(CommandLineArgumentType.ScriptArgument, "Src2"),
                new CommandLineArgument(CommandLineArgumentType.ScriptArgument, "-p:Abc=Xyz")
            }
        },

        // when Tool
        new object[]
        {
            new[] {"--source", "Src1", "--", "Abc", "Src2"},
            CommandLineArgumentType.ScriptArgument,
            new[]
            {
                new CommandLineArgument(CommandLineArgumentType.NuGetSource, "Src1"),
                new CommandLineArgument(CommandLineArgumentType.ScriptArgument, "Abc"),
                new CommandLineArgument(CommandLineArgumentType.ScriptArgument, "Src2")
            }
        },

        // script properties
        new object[]
        {
            new[] {"-p", "Key1=Val1"},
            CommandLineArgumentType.ScriptFile,
            new[]
            {
                new CommandLineArgument(CommandLineArgumentType.ScriptProperty, "Val1", "Key1")
            }
        },

        new object[]
        {
            new[] {"--Property", "Key1=Val1"},
            CommandLineArgumentType.ScriptFile,
            new[]
            {
                new CommandLineArgument(CommandLineArgumentType.ScriptProperty, "Val1", "Key1")
            }
        },

        new object[]
        {
            new[] {"/P", "Key1=Val1"},
            CommandLineArgumentType.ScriptFile,
            new[]
            {
                new CommandLineArgument(CommandLineArgumentType.ScriptProperty, "Val1", "Key1")
            }
        },

        new object[]
        {
            new[] {"/property", "Key1=Val1"},
            CommandLineArgumentType.ScriptFile,
            new[]
            {
                new CommandLineArgument(CommandLineArgumentType.ScriptProperty, "Val1", "Key1")
            }
        },

        new object[]
        {
            new[] {"-p", "Key1"},
            CommandLineArgumentType.ScriptFile,
            new[]
            {
                new CommandLineArgument(CommandLineArgumentType.ScriptProperty, "", "Key1")
            }
        },
        
        new object[]
        {
            new[] {"--Property:Key1=Val1"},
            CommandLineArgumentType.ScriptFile,
            new[]
            {
                new CommandLineArgument(CommandLineArgumentType.ScriptProperty, "Val1", "Key1")
            }
        },
        
        new object[]
        {
            new[] {"--Property:Key12ab.,+-~#=Val12ab.,+-~#"},
            CommandLineArgumentType.ScriptFile,
            new[]
            {
                new CommandLineArgument(CommandLineArgumentType.ScriptProperty,"Val12ab.,+-~#", "Key12ab.,+-~#")
            }
        },
        
        new object[]
        {
            new[] {"--Property:Key12ab.,+-~#=Val12a=b.,+-~#"},
            CommandLineArgumentType.ScriptFile,
            new[]
            {
                new CommandLineArgument(CommandLineArgumentType.ScriptProperty,"Val12a=b.,+-~#", "Key12ab.,+-~#")
            }
        },
        
        new object[]
        {
            new[] {"--Property: Key1  = Val1  "},
            CommandLineArgumentType.ScriptFile,
            new[]
            {
                new CommandLineArgument(CommandLineArgumentType.ScriptProperty, " Val1  ", " Key1  ")
            }
        },
        
        new object[]
        {
            new[] {"/Property:Key1=Val1"},
            CommandLineArgumentType.ScriptFile,
            new[]
            {
                new CommandLineArgument(CommandLineArgumentType.ScriptProperty, "Val1", "Key1")
            }
        },
        
        new object[]
        {
            new[] {"-p:Key1=Val1"},
            CommandLineArgumentType.ScriptFile,
            new[]
            {
                new CommandLineArgument(CommandLineArgumentType.ScriptProperty, "Val1", "Key1")
            }
        },
        
        new object[]
        {
            new[] {"/P:Key1=Val1"},
            CommandLineArgumentType.ScriptFile,
            new[]
            {
                new CommandLineArgument(CommandLineArgumentType.ScriptProperty, "Val1", "Key1")
            }
        },
    };
    
    [Theory]
    [MemberData(nameof(Data))]
    internal void ShouldParseArguments(string[] arguments, CommandLineArgumentType defaultArgType, CommandLineArgument[] expectedArguments)
    {
        // Given
        var parser = CreateInstance();
        _fileSystem.Setup(i => i.ReadAllLines("rspFile")).Returns(new[] {"-S", "Src2", "/Source", "Src3"});

        // When
        var actualArguments = parser.Parse(arguments, defaultArgType).ToList();

        // Then
        actualArguments.ShouldBe(expectedArguments);
    }

    private CommandLineParser CreateInstance() =>
        new(_fileSystem.Object);
}