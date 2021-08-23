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
            result.StdOut.Contains("Hello").ShouldBeTrue();
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
            result.StdOut.Contains(expectedOutput).ShouldBeTrue();
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
            result.StdOut.Contains(string.Empty).ShouldBeTrue();
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
            result.StdOut.Contains("Args: 2, Abc, Xyz").ShouldBeTrue();
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
            result.StdOut.Contains("AbcXyzASD_4").ShouldBeTrue();
        }
        
        [Fact]
        public void ShouldSupportError()
        {
            // Given

            // When
            var result = Run(@$"Error(""My error"");");
            
            // Then
            result.ExitCode.Value.ShouldBe(1);
            result.StdErr.ShouldBe(new []{ "My error" });
            result.StdOut.Count.ShouldBe(InitialLinesCount + 2);
            result.StdOut.Contains("My error").ShouldBeTrue();
        }
        
        [Fact]
        public void ShouldSupportWarning()
        {
            // Given

            // When
            var result = Run(@$"Warning(""My warning"");");
            
            // Then
            result.ExitCode.Value.ShouldBe(0);
            result.StdErr.ShouldBeEmpty();
            result.StdOut.Count.ShouldBe(InitialLinesCount + 3);
            result.StdOut.Contains("My warning").ShouldBeTrue();
        }
        
        [Fact]
        public void ShouldSupportInfo()
        {
            // Given

            // When
            var result = Run(@$"Info(""My info"");");
            
            // Then
            result.ExitCode.Value.ShouldBe(0);
            result.StdErr.ShouldBeEmpty();
            result.StdOut.Count.ShouldBe(InitialLinesCount + 1);
            result.StdOut.Contains("My info").ShouldBeTrue();
        }
        
        [Theory()]
        [InlineData("nuget: IoC.Container, 1.3.6", "//1")]
        [InlineData("nuget:IoC.Container,1.3.6", "//1")]
        [InlineData("nuget: IoC.Container, [1.3.6, 2)", "//1")]
        [InlineData("nuget: IoC.Container", "container://1")]
        public void ShouldSupportNuGetRestore(string package, string name)
        {
            // Given

            // When
            var result = Run(
                @$"#r ""{package}""",
                "using IoC;",
                "WriteLine(Container.Create());");
            
            // Then
            result.ExitCode.Value.ShouldBe(0);
            result.StdErr.ShouldBeEmpty();
            result.StdOut.Any(i => i.Trim() == "Installed:").ShouldBeTrue();
            result.StdOut.Contains(name).ShouldBeTrue();
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