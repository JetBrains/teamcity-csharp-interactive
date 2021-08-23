namespace Teamcity.CSharpInteractive.Tests.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using Shouldly;
    using Xunit;
    
    public class ScriptRunTests
    {
        private const int InitialLinesCount = 3;
            
        [Fact]
        public void ShouldRunEmptyScript()
        {
            // Given

            // When
            var result = Run();
            
            // Then
            result.ExitCode.Value.ShouldBe(0);
            result.StdErr.ShouldBeEmpty();
            result.StdOut.Count.ShouldBe(InitialLinesCount);
        }
        
        [Fact]
        public void ShouldAddSystemNamespace()
        {
            // Given

            // When
            var result = Run(@"Console.WriteLine(""Hello"");");
            
            // Then
            result.ExitCode.Value.ShouldBe(0);
            result.StdErr.ShouldBeEmpty();
            result.StdOut.Count.ShouldBe(InitialLinesCount + 1);
            result.StdOut.Any(i => i == "Hello").ShouldBeTrue();
        }
        
        [Theory]
        [InlineData(@"""Hello""", "Hello")]
        [InlineData("99", "99")]
        [InlineData("true", "True")]
        public void ShouldSupportWriteLine(string writeLineArg, string expectedOutput)
        {
            // Given

            // When
            var result = Run(@$"WriteLine({writeLineArg});");
            
            // Then
            result.ExitCode.Value.ShouldBe(0);
            result.StdErr.ShouldBeEmpty();
            result.StdOut.Count.ShouldBe(InitialLinesCount + 1);
            result.StdOut.Any(i => i == expectedOutput).ShouldBeTrue();
        }
        
        [Fact]
        public void ShouldSupportWriteLineWhenNoArgs()
        {
            // Given

            // When
            var result = Run("WriteLine();");
            
            // Then
            result.ExitCode.Value.ShouldBe(0);
            result.StdErr.ShouldBeEmpty();
            result.StdOut.Count.ShouldBe(InitialLinesCount + 1);
            result.StdOut.Any(i => i == "").ShouldBeTrue();
        }
        
        [Fact]
        public void ShouldSupportArgs()
        {
            // Given

            // When
            var result = Run(
                Array.Empty<string>(),
                new []{"Abc", "Xyz"},
                @"WriteLine($""Args: {Args.Length}, {Args[0]}, {Args[1]}"");"
            );
            
            // Then
            result.ExitCode.Value.ShouldBe(0);
            result.StdErr.ShouldBeEmpty();
            result.StdOut.Any(i => i == "Args: 2, Abc, Xyz").ShouldBeTrue();
        }
        
        [Fact]
        public void ShouldSupportProps()
        {
            // Given

            // When
            var result = Run(
                new []
                {
                    "--property", "Val1=Abc",
                    "/property", "val2=Xyz",
                    "-p", "val3=ASD",
                    "/p", "4=_"
                },
                Array.Empty<string>(),
                @"WriteLine(Props[""Val1""] + Props[""val2""] + Props[""val3""] + Props[""4""] + Props.Count);"
            );
            
            // Then
            result.ExitCode.Value.ShouldBe(0);
            result.StdErr.ShouldBeEmpty();
            result.StdOut.Any(i => i == "AbcXyzASD_4").ShouldBeTrue();
        }

        private static IProcessResult Run(IEnumerable<string> args, IEnumerable<string> scriptArgs, params string[] lines)
        {
            var fileSystem = Composer.Resolve<IFileSystem>();
            var scriptFile = fileSystem.CreateTempFilePath();
            try
            {
                fileSystem.AppendAllLines(scriptFile, lines);
                var allArgs = new List<string>(args) { scriptFile };
                allArgs.AddRange(scriptArgs);
                return Composer.Resolve<IProcessRunner>().Run(allArgs.Select(i => new CommandLineArgument(i)), Array.Empty<EnvironmentVariable>());
            }
            finally
            {
                fileSystem.DeleteFile(scriptFile);
            }
        }
        
        private static IProcessResult Run(params string[] lines) =>
            Run(Array.Empty<string>(), Array.Empty<string>(), lines);
    }
}