namespace TeamCity.CSharpInteractive.Tests;

using Cmd;

public class StartInfoFactoryTests
{
    private readonly Mock<IEnvironment> _environment;

    public StartInfoFactoryTests()
    {
        _environment = new Mock<IEnvironment>();
    }

    [Fact]
    public void ShouldCreateStartInfo()
    {
        // Given
        var instance = CreateInstance();

        // When
        var startInfo = instance.Create(new CommandLine("WhoAmI", "/Abc", "xyz").AddVars(("Var1", "Val1"), ("Var2", "Val2")).WithWorkingDirectory("Wd"));

        // Then
        startInfo.FileName.ShouldBe("WhoAmI");
        startInfo.ArgumentList.ShouldBe(new List<string> {"/Abc", "xyz"});
        startInfo.Environment["Var1"].ShouldBe("Val1");
        startInfo.Environment["Var2"].ShouldBe("Val2");
        startInfo.UseShellExecute.ShouldBeFalse();
        startInfo.CreateNoWindow.ShouldBeTrue();
        startInfo.RedirectStandardOutput.ShouldBeTrue();
        startInfo.RedirectStandardError.ShouldBeTrue();
    }
        
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void ShouldReplaceWorkingDirectoryWhenEmpty(string workingDirectory)
    {
        // Given
        _environment.Setup(i => i.GetPath(SpecialFolder.Working)).Returns("WD");
        var instance = CreateInstance();

        // When
        var startInfo = instance.Create(new CommandLine("WhoAmI").WithWorkingDirectory(workingDirectory));

        // Then
        startInfo.WorkingDirectory.ShouldBe("WD");
    }

    private StartInfoFactory CreateInstance() =>
        new(Mock.Of<ILog<StartInfoFactory>>(), _environment.Object);
}