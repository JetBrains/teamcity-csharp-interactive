namespace TeamCity.CSharpInteractive.Tests.Integration
{
    using System.Linq;
    using Shouldly;
    using Xunit;

    [CollectionDefinition("Integration", DisableParallelization = true)]
    public class ScriptRunTeamCityScriptRunTestsTests
    {
        private const int InitialMessagesCount = 3;

        [Fact]
        public void ShouldAddSystemNamespace()
        {
            // Given

            // When
            var result = TestTool.RunUnderTeamCity(@"Console.WriteLine(""Hello"");");

            // Then
            result.ExitCode.ShouldBe(0);
            result.StdErr.ShouldBeEmpty();
            var messages = result.StdOut.ParseMessages();
            messages.Count.ShouldBe(InitialMessagesCount);
            result.StdOut.Contains("Hello").ShouldBeTrue();
        }

        [Fact]
        public void ShouldSupportWriteLine()
        {
            // Given

            // When
            var result = TestTool.RunUnderTeamCity(@"WriteLine(99);");

            // Then
            result.ExitCode.ShouldBe(0);
            result.StdErr.ShouldBeEmpty();
            var messages = result.StdOut.ParseMessages();
            messages.Count.ShouldBe(InitialMessagesCount + 1);
            messages.ShouldContainNormalTextMessage(i => i == "99");
        }
        
        [Fact]
        public void ShouldSupportError()
        {
            // Given

            // When
            var result = TestTool.RunUnderTeamCity(@"Error(""My error"", ""errId"");");

            // Then
            result.ExitCode.ShouldBe(1);
            result.StdErr.ShouldBeEmpty();
            var messages = result.StdOut.ParseMessages();
            messages.ShouldContainBuildProblem(i => i == "My error", i => i == "errId");
        }
        
        [Fact]
        public void ShouldSupportWarning()
        {
            // Given

            // When
            var result = TestTool.RunUnderTeamCity(@"Warning(""My warning"");");

            // Then
            result.ExitCode.ShouldBe(0);
            result.StdErr.ShouldBeEmpty();
            var messages = result.StdOut.ParseMessages();
            messages.ShouldContainWarningTextMessage(i => i == "My warning");
        }
        
        [Fact]
        public void ShouldSupportInfo()
        {
            // Given

            // When
            var result = TestTool.RunUnderTeamCity(@"Info(""My info"");");

            // Then
            result.ExitCode.ShouldBe(0);
            result.StdErr.ShouldBeEmpty();
            var messages = result.StdOut.ParseMessages();
            messages.Count.ShouldBe(InitialMessagesCount + 1);
            messages.ShouldContainNormalTextMessage(i => i == "My info");
        }
        
        [Fact]
        public void ShouldProcessCompilationError()
        {
            // Given

            // When
            var result = TestTool.RunUnderTeamCity("i = 10;");

            // Then
            result.ExitCode.ShouldBe(1);
            result.StdErr.ShouldBeEmpty();
            var messages = result.StdOut.ParseMessages();
            messages.ShouldContainBuildProblem(i => i.Contains("error CS0103"), i => i.StartsWith("CS0103,0,1"));
        }
        
        [Fact]
        public void ShouldProcessRuntimeException()
        {
            // Given

            // When
            var result = TestTool.RunUnderTeamCity(@"throw new Exception(""Test"");");

            // Then
            result.ExitCode.ShouldBe(1);
            result.StdErr.ShouldBeEmpty();
            var messages = result.StdOut.ParseMessages();
            messages.ShouldContainBuildProblem(i => i.Contains("System.Exception: Test"), i => i == "CSI006");
        }
        
        [Fact]
        public void ShouldSendBuildProblemWhenScripIsUncompleted()
        {
            // Given

            // When
            var result = TestTool.RunUnderTeamCity(@"var i=10");

            // Then
            result.ExitCode.ShouldBe(1);
            result.StdErr.ShouldBeEmpty();
            var messages = result.StdOut.ParseMessages();
            messages.ShouldContainBuildProblem(i => i.Contains("Script is uncompleted."), i => i == "CSI007");
        }
    }
}