namespace Teamcity.CSharpInteractive.Tests
{
    using System.Collections.Generic;
    using Moq;
    using Shouldly;
    using Xunit;

    public class CommandLineParserTests
    {
        private readonly Mock<IFileTextReader> _fileReader;

        public CommandLineParserTests() => _fileReader = new Mock<IFileTextReader>();

        [Theory]
        [MemberData(nameof(Data))]
        internal void ShouldParseArguments(string[] arguments, CommandLineArgument[] expectedArguments)
        {
            // Given
            var parser = CreateInstance();
            _fileReader.Setup(i => i.ReadLines("rspFile")).Returns(new [] {"-S", "Src2", "/Source", "Src3"});

            // When
            var actualArguments = parser.Parse(arguments);

            // Then
            actualArguments.ShouldBe(expectedArguments);
        }
        
        public static IEnumerable<object?[]> Data => new List<object?[]>
        {
            // help
            new object[]
            {
                new[] {"--help"},
                new[] {new CommandLineArgument(CommandLineArgumentType.Help)}
            },
            
            new object[]
            {
                new[] {"--help", "--version"},
                new[] {new CommandLineArgument(CommandLineArgumentType.Help)}
            },
            
            new object[]
            {
                new[] {"--HelP", "--version"},
                new[] {new CommandLineArgument(CommandLineArgumentType.Help)}
            },
            
            new object[]
            {
                new[] {"-h", "--version"},
                new[] {new CommandLineArgument(CommandLineArgumentType.Help)}
            },
            
            new object[]
            {
                new[] {"/help", "--version"},
                new[] {new CommandLineArgument(CommandLineArgumentType.Help)}
            },
            
            new object[]
            {
                new[] {"/h", "--version"},
                new[] {new CommandLineArgument(CommandLineArgumentType.Help)}
            },
            
            new object[]
            {
                new[] {"/?", "--version"},
                new[] {new CommandLineArgument(CommandLineArgumentType.Help)}
            },
            
            // version
            new object[]
            {
                new[] {"--version"},
                new[] {new CommandLineArgument(CommandLineArgumentType.Version)}
            },
            
            new object[]
            {
                new[] {"--version", "-h"},
                new[] {new CommandLineArgument(CommandLineArgumentType.Version)}
            },
            
            new object[]
            {
                new[] {"--VerSion", "-h"},
                new[] {new CommandLineArgument(CommandLineArgumentType.Version)}
            },
            
            new object[]
            {
                new[] {"/version", "-h"},
                new[] {new CommandLineArgument(CommandLineArgumentType.Version)}
            },
            
            // nuget source
            new object[]
            {
                new[] {"--source", "Src1", "-S", "Src2", "/Source", "Src3", "/S", "Src4"},
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
                new[]
                {
                    new CommandLineArgument(CommandLineArgumentType.NuGetSource, "Src1"),
                    new CommandLineArgument(CommandLineArgumentType.ScriptFile, "scriptFile"),
                    new CommandLineArgument(CommandLineArgumentType.ScriptArgument, "-v"),
                    new CommandLineArgument(CommandLineArgumentType.ScriptArgument, "Arg 2")
                }
            },
            
            // --
            new object[]
            {
                new[] {"--source", "Src1", "--", "-s", "Src2"},
                new[]
                {
                    new CommandLineArgument(CommandLineArgumentType.NuGetSource, "Src1"),
                    new CommandLineArgument(CommandLineArgumentType.ScriptFile, "-s"),
                    new CommandLineArgument(CommandLineArgumentType.ScriptArgument, "Src2")
                }
            },
            
            // script properties
            new object[]
            {
                new[] {"-p", "Key1=Val1"},
                new[]
                {
                    new CommandLineArgument(CommandLineArgumentType.ScriptProperty, "Val1", "Key1"),
                }
            },
            
            new object[]
            {
                new[] {"--Property", "Key1=Val1"},
                new[]
                {
                    new CommandLineArgument(CommandLineArgumentType.ScriptProperty, "Val1", "Key1"),
                }
            },
            
            new object[]
            {
                new[] {"/P", "Key1=Val1"},
                new[]
                {
                    new CommandLineArgument(CommandLineArgumentType.ScriptProperty, "Val1", "Key1"),
                }
            },
            
            new object[]
            {
                new[] {"/property", "Key1=Val1"},
                new[]
                {
                    new CommandLineArgument(CommandLineArgumentType.ScriptProperty, "Val1", "Key1"),
                }
            },
            
            new object[]
            {
                new[] {"-p", "Key1"},
                new[]
                {
                    new CommandLineArgument(CommandLineArgumentType.ScriptProperty, "", "Key1"),
                }
            }
        };

        private CommandLineParser CreateInstance() =>
            new(_fileReader.Object);
    }
}