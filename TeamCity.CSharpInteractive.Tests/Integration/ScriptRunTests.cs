// ReSharper disable StringLiteralTypo
// ReSharper disable RedundantUsingDirective
namespace TeamCity.CSharpInteractive.Tests.Integration;

using Core;
using HostApi;
using Script;

[CollectionDefinition("Integration", DisableParallelization = true)]
[Trait("Integration", "true")]
public class ScriptRunTests
{
    private const int InitialLinesCount = 3;

    [Fact]
    public void ShouldRunEmptyScript()
    {
        // Given

        // When
        var result = TestTool.Run();

        // Then
        result.ExitCode.ShouldBe(0, result.ToString());
        result.StdErr.ShouldBeEmpty(result.ToString());
        result.StdOut.Count.ShouldBe(InitialLinesCount, result.ToString());
    }

    [Fact]
    public void ShouldAddSystemNamespace()
    {
        // Given

        // When
        var result = TestTool.Run(@"Console.WriteLine(""Hello"");");

        // Then
        result.ExitCode.ShouldBe(0, result.ToString());
        result.StdErr.ShouldBeEmpty(result.ToString());
        result.StdOut.Count.ShouldBe(InitialLinesCount + 1, result.ToString());
        result.StdOut.Contains("Hello").ShouldBeTrue(result.ToString());
    }

    [Theory]
    [InlineData(@"""Hello""", "Hello")]
    [InlineData("99", "99")]
    [InlineData("true", "True")]
    public void ShouldSupportWriteLine(string writeLineArg, string expectedOutput)
    {
        // Given

        // When
        var result = TestTool.Run(@$"WriteLine({writeLineArg});");

        // Then
        result.ExitCode.ShouldBe(0, result.ToString());
        result.StdErr.ShouldBeEmpty(result.ToString());
        result.StdOut.Count.ShouldBe(InitialLinesCount + 1, result.ToString());
        result.StdOut.Contains(expectedOutput).ShouldBeTrue(result.ToString());
    }

    [Fact]
    public void ShouldSupportWriteLineWhenNoArgs()
    {
        // Given

        // When
        var result = TestTool.Run("WriteLine();");

        // Then
        result.ExitCode.ShouldBe(0, result.ToString());
        result.StdErr.ShouldBeEmpty(result.ToString());
        result.StdOut.Count.ShouldBe(InitialLinesCount + 1, result.ToString());
        result.StdOut.Contains(string.Empty).ShouldBeTrue(result.ToString());
    }

    [Fact]
    public void ShouldSupportArgs()
    {
        // Given

        // When
        var result = TestTool.Run(
            Array.Empty<string>(),
            new[] {"Abc", "Xyz"},
            TestTool.DefaultVars,
            @"WriteLine($""Args: {Args.Count}, {Args[0]}, {Args[1]}"");"
        );

        // Then
        result.ExitCode.ShouldBe(0, result.ToString());
        result.StdErr.ShouldBeEmpty(result.ToString());
        result.StdOut.Contains("Args: 2, Abc, Xyz").ShouldBeTrue(result.ToString());
    }

    [Theory]
    [InlineData("")]
    [InlineData("2021")]
    public void ShouldSupportProps(string teamcityVersionEnvVar)
    {
        // Given

        // When
        var result = TestTool.Run(
            new[]
            {
                "--property", "Val1=Abc",
                "/property", "val2=Xyz",
                "-p", "val3=ASD",
                "/p", "4=_"
            },
            Array.Empty<string>(),
            new[] {("TEAMCITY_VERSION", teamcityVersionEnvVar)},
            @"WriteLine(Props[""Val1""] + Props[""val2""] + Props[""val3""] + Props[""4""] + Props.Count);"
        );

        // Then
        result.ExitCode.ShouldBe(0, result.ToString());
        result.StdErr.ShouldBeEmpty(result.ToString());
        result.StdOut.Any(i => i.Contains("AbcXyzASD_4")).ShouldBeTrue();
    }

    [Fact]
    public void ShouldSetTeamCitySystemParamViaProp()
    {
        // Given

        // When
        var result = TestTool.Run(
            Array.Empty<string>(),
            Array.Empty<string>(),
            new[] {("TEAMCITY_VERSION", "2021")},
            @"Props[""Val1""]=""Xyz"";"
        );

        // Then
        result.ExitCode.ShouldBe(0, result.ToString());
        result.StdErr.ShouldBeEmpty(result.ToString());
        result.StdOut.Any(i => i.Contains("##teamcity[setParameter name='system.Val1' value='Xyz'")).ShouldBeTrue();
    }

    [Fact]
    public void ShouldSupportError()
    {
        // Given

        // When
        var result = TestTool.Run(@"Error(""My error"");");

        // Then
        result.ExitCode.ShouldBe(1, result.ToString());
        result.StdOut.Contains("My error").ShouldBeTrue(result.ToString());
    }

    [Fact]
    public void ShouldSupportWarning()
    {
        // Given

        // When
        var result = TestTool.Run(@"Warning(""My warning"");");

        // Then
        result.ExitCode.ShouldBe(0, result.ToString());
        result.StdErr.ShouldBeEmpty(result.ToString());
        result.StdOut.Count.ShouldBe(InitialLinesCount + 3, result.ToString());
        result.StdOut.Contains("My warning").ShouldBeTrue(result.ToString());
    }

    [Fact]
    public void ShouldSupportInfo()
    {
        // Given

        // When
        var result = TestTool.Run(@"Info(""My info"");");

        // Then
        result.ExitCode.ShouldBe(0, result.ToString());
        result.StdErr.ShouldBeEmpty(result.ToString());
        result.StdOut.Count.ShouldBe(InitialLinesCount + 1, result.ToString());
        result.StdOut.Contains("My info").ShouldBeTrue(result.ToString());
    }

    [Theory]
    [InlineData("nuget: IoC.Container, 1.3.6", "//1")]
    [InlineData("nuget:IoC.Container,1.3.6", "//1")]
    [InlineData("nuget: IoC.Container, [1.3.6, 2)", "//1")]
    [InlineData("nuget: IoC.Container", "//1")]
    public void ShouldSupportNuGetRestore(string package, string name)
    {
        // Given

        // When
        var result = TestTool.Run(
            @$"#r ""{package}""",
            "using IoC;",
            "WriteLine(Container.Create());");

        // Then
        result.ExitCode.ShouldBe(0, result.ToString());
        result.StdErr.ShouldBeEmpty(result.ToString());
        result.StdOut.Count(i => i.Contains("//1")).ShouldBe(1, result.ToString());
        result.StdOut.Contains(name).ShouldBeTrue(result.ToString());
    }

    [Fact]
    public void ShouldSupportLoad()
    {
        // Given
        var fileSystem = Composer.Resolve<IFileSystem>();
        var refScriptFile = fileSystem.CreateTempFilePath();
        try
        {
            fileSystem.AppendAllLines(refScriptFile, new[] {@"Console.WriteLine(""Hello"");"});

            // When
            var result = TestTool.Run(@$"#load ""{refScriptFile}""");

            // Then
            result.ExitCode.ShouldBe(0, result.ToString());
            result.StdErr.ShouldBeEmpty(result.ToString());
            result.StdOut.Count.ShouldBe(InitialLinesCount + 1, result.ToString());
            result.StdOut.Contains("Hello").ShouldBeTrue(result.ToString());
        }
        finally
        {
            fileSystem.DeleteFile(refScriptFile);
        }
    }

    [Fact]
    public void ShouldSupportLoadWhenRelativePath()
    {
        // Given
        var workingDirectory = DotNetScript.GetWorkingDirectory();
        // ReSharper disable once UnusedVariable
        var scriptAbc = DotNetScript.Create("abc.csx", workingDirectory, Enumerable.Empty<string>(), "Console.WriteLine(\"Hello\");");
        var script = DotNetScript.Create("script.csx", workingDirectory, Enumerable.Empty<string>(), "#load \"abc.csx\"").WithVars(TestTool.DefaultVars);

        // When
        var result = TestTool.Run(script);

        // Then
        result.ExitCode.ShouldBe(0, result.ToString());
        result.StdErr.ShouldBeEmpty(result.ToString());
        result.StdOut.Count.ShouldBe(InitialLinesCount + 1, result.ToString());
        result.StdOut.Contains("Hello").ShouldBeTrue(result.ToString());
    }

    [Fact]
    public void ShouldSupportLoadMultiple()
    {
        // Given
        var workingDirectory = DotNetScript.GetWorkingDirectory();
        // ReSharper disable once UnusedVariable
        var scriptClass1 = DotNetScript.Create("class1.csx", workingDirectory, Enumerable.Empty<string>(), "class Class1 { public Class1(Class2 val) {}};");
        // ReSharper disable once UnusedVariable
        var scriptClass2 = DotNetScript.Create("class2.csx", workingDirectory, Enumerable.Empty<string>(), "class Class2 {};");
        var script = DotNetScript.Create("script.csx", workingDirectory, Enumerable.Empty<string>(), "#load \"class2.csx\"", "#load \"class1.csx\"").WithVars(TestTool.DefaultVars);

        // When
        var result = TestTool.Run(script);

        // Then
        result.ExitCode.ShouldBe(0, result.ToString());
        result.StdErr.ShouldBeEmpty(result.ToString());
    }

    [Fact]
    public void ShouldSupportLoadMultiple2()
    {
        // Given
        var workingDirectory = DotNetScript.GetWorkingDirectory();
        // ReSharper disable once UnusedVariable
        var scriptClass1 = DotNetScript.Create("class1.csx", workingDirectory, Enumerable.Empty<string>(), "class Class1 { public Class1(Class2 val) {}};");
        // ReSharper disable once UnusedVariable
        var scriptClass2 = DotNetScript.Create("class2.csx", workingDirectory, Enumerable.Empty<string>(), "class Class2 {};");
        var script = DotNetScript.Create("script.csx", workingDirectory, Enumerable.Empty<string>(), "// #load \"class1.csx\"", "#load \"class2.csx\"").WithVars(TestTool.DefaultVars);

        // When
        var result = TestTool.Run(script);

        // Then
        result.ExitCode.ShouldBe(0, result.ToString());
        result.StdErr.ShouldBeEmpty(result.ToString());
    }

    [Fact]
    public void ShouldProcessCompilationError()
    {
        // Given

        // When
        var result = TestTool.Run("i = 10;");

        // Then
        result.ExitCode.ShouldBe(1, result.ToString());
        result.StdOut.Count(i => i.Contains("CS0103")).ShouldBe(2, result.ToString());
    }

    [Fact]
    public void ShouldProcessRuntimeException()
    {
        // Given

        // When
        var result = TestTool.Run(@"throw new Exception(""Test"");");

        // Then
        result.ExitCode.ShouldBe(1, result.ToString());
        result.StdOut.Count(i => i.Contains("System.Exception: Test")).ShouldBe(2, result.ToString());
    }

    [Theory]
    [InlineData("")]
    [InlineData("2021")]
    public void ShouldNotAddAlreadyAddedReferencesWhenRestore(string teamcityVersionEnvVar)
    {
        // Given

        // When
        var result = TestTool.Run(
            new[] {"-s", Path.Combine(Directory.GetCurrentDirectory(), "Integration", "Resources")},
            Array.Empty<string>(),
            new[] {("TEAMCITY_VERSION", teamcityVersionEnvVar)},
            @"#r ""nuget: csinetstandard11, 1.0.0""",
            "using System.Collections.Generic;",
            "using System.Linq;",
            "var list = new List<int>{1, 2};",
            "var list2 = list.Where(i => i == 1).ToList();",
            "WriteLine(list2.Count);");

        // Then
        result.StdErr.ShouldBeEmpty(result.ToString());
        result.ExitCode.ShouldBe(0, result.ToString());
    }

    [Fact]
    public void ShouldSupportHostFromLocalFunctions()
    {
        // Given

        // When
        var result = TestTool.Run(
            "using HostApi;",
            "class Abc",
            "{",
            "  private readonly IHost _host;",
            "  public Abc(IHost host) => _host = host;",
            "  public void Fun()",
            "  {",
            "    void LocalFun()",
            "    {",
            @"      _host.Info(""Abc"");",
            "    }",
            "    LocalFun();",
            "  }",
            "}",
            "new Abc(Host).Fun();"
        );

        // Then
        result.StdErr.ShouldBeEmpty(result.ToString());
        result.ExitCode.ShouldBe(0, result.ToString());
        result.StdOut.Contains("Abc").ShouldBeTrue(result.ToString());
    }

    [Fact]
    public void ShouldSupportComplexStatements()
    {
        // Given

        // When
        var result = TestTool.Run(
            "if(true)",
            "{",
            "}",
            "else",
            "{",
            "}"
        );

        // Then
        result.StdErr.ShouldBeEmpty(result.ToString());
        result.ExitCode.ShouldBe(0, result.ToString());
    }

    [Fact]
    public void ShouldSupportTeamCityServiceMessages()
    {
        // Given

        // When
        var result = TestTool.Run(
            "using JetBrains.TeamCity.ServiceMessages.Write.Special;",
            "GetService<ITeamCityWriter>().WriteBuildParameter(\"system.hello\", \"Abc\");");

        // Then
        result.StdErr.ShouldBeEmpty(result.ToString());
        result.ExitCode.ShouldBe(0, result.ToString());
    }

    [Theory]
    [InlineData("System.Linq", "Enumerable.Empty<int>()")]
    [InlineData("System.Collections.Generic", "new List<int>()")]
    [InlineData("System.Net.Http", "new HttpClient()")]
    public void ShouldUseType(string ns, string ctor)
    {
        // Given

        // When
        var result = TestTool.Run(
            $"using {ns};",
            $"{ctor};");

        // Then
        result.StdErr.ShouldBeEmpty(result.ToString());
        result.ExitCode.ShouldBe(0, result.ToString());
    }

    [Fact]
    public void ShouldFailWhenHasErrors()
    {
        // Given

        // When
        var result = TestTool.Run(@"Abc.Abc();");

        // Then
        result.ExitCode.ShouldBe(1, result.ToString());
        result.StdErr.ShouldBeEmpty();
        result.StdOut.Any(i => i.Contains("error CS0103")).ShouldBeTrue(result.ToString());
    }
}