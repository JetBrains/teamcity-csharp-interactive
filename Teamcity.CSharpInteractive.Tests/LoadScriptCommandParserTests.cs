namespace Teamcity.CSharpInteractive.Tests
{
    using System.Collections.Generic;
    using Moq;
    using Shouldly;
    using Xunit;

    public class LoadScriptCommandParserTests
    {
        [Theory]
        [MemberData(nameof(Data))]
        internal void ShouldParse(string replCommand, bool expectedResult, ICommand? expectedCommand)
        {
            // Given
            var stringService = new Mock<IStringService>();
            stringService.Setup(i => i.TrimAndUnquote(It.IsAny<string>())).Returns<string>(i => i);
            var parser = new LoadScriptCommandParser(Mock.Of<ILog<LoadScriptCommandParser>>(),  stringService.Object);
            
            // When
            var actualResult = parser.TryParse(replCommand, out var actualCommand);

            // Then
            actualResult.ShouldBe(expectedResult);
            if (expectedResult)
            {
                actualCommand.ShouldBe(expectedCommand);
            }
        }
        
        public static IEnumerable<object?[]> Data => new List<object?[]> 
        {
            new object[] { "load Abc", true, new LoadScriptCommand("Abc") },
            new object[] { "load   Abc", true, new LoadScriptCommand("Abc") },
            new object[] { "load \"Abc\"", true, new LoadScriptCommand("\"Abc\"") },
            new object[] { "LoaD Abc", true, new LoadScriptCommand("Abc") },
            new object?[] { "  load   Abc", false, null },
            new object?[] { "loadAbc", false, null },
            new object?[] { "load ", false, null },
            new object?[] { "Abc", false, null },
            new object?[] { "  ", false, null },
            new object?[] { "", false, null },
        };
    }
}