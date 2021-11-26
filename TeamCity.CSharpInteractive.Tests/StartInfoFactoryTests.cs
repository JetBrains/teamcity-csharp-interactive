namespace TeamCity.CSharpInteractive.Tests
{
    using System.Collections.Generic;
    using Cmd;
    using Moq;
    using Shouldly;
    using Xunit;

    public class StartInfoFactoryTests
    {
        private readonly Mock<IEnvironment> _environment;
        private readonly Mock<IWellknownValueResolver> _wellknownValueResolver;

        public StartInfoFactoryTests()
        {
            _environment = new Mock<IEnvironment>();
            _wellknownValueResolver = new Mock<IWellknownValueResolver>();
            _wellknownValueResolver.Setup(i => i.Resolve(It.IsAny<string>())).Returns<string>(i => i);
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
        
        [Fact]
        public void ShouldResolveValue()
        {
            // Given
            var instance = CreateInstance();

            // When
            _wellknownValueResolver.Setup(i => i.Resolve(It.IsAny<string>()))
                .Returns<string>(i => 
                    i.Replace("DDD", "AmI")
                        .Replace("ar", "AR")
                        .Replace("al", "Al")
                        .Replace("Abc", "ABC")
                        .Replace("xyz", "Xyz")
                    );
            var startInfo = instance.Create(new CommandLine("WhoDDD", "/Abc", "xyz").AddVars(("Var1", "Val1"), ("Var2", "Val2")).WithWorkingDirectory("Wd"));

            // Then
            startInfo.FileName.ShouldBe("WhoAmI");
            startInfo.ArgumentList.ShouldBe(new List<string> {"/ABC", "Xyz"});
            startInfo.Environment["VAR1"].ShouldBe("VAl1");
            startInfo.Environment["VAR2"].ShouldBe("VAl2");
            startInfo.UseShellExecute.ShouldBeFalse();
            startInfo.CreateNoWindow.ShouldBeTrue();
            startInfo.RedirectStandardOutput.ShouldBeTrue();
            startInfo.RedirectStandardError.ShouldBeTrue();
        }

        private StartInfoFactory CreateInstance() =>
            new(Mock.Of<ILog<StartInfoFactory>>(), _environment.Object, _wellknownValueResolver.Object);
    }
}