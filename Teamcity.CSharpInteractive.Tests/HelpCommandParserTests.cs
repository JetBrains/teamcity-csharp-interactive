namespace Teamcity.CSharpInteractive.Tests
{
    using System.Collections.Generic;
    using Moq;
    using Shouldly;
    using Xunit;

    public class HelpCommandParserTests
    {
        [Theory]
        [MemberData(nameof(Data))]
        internal void ShouldParse(string replCommand, bool expectedResult, ICommand? expectedCommand)
        {
            // Given
            var stringService = new Mock<IStringService>();
            stringService.Setup(i => i.TrimAndUnquote(It.IsAny<string>())).Returns<string>(i => i);
            var parser = new HelpCommandParser(Mock.Of<ILog<HelpCommandParser>>());
            
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
            new object[] { "help", true, HelpCommand.Shared },
            new object[] { "Help", true, HelpCommand.Shared },
            new object[] { "help ", true, HelpCommand.Shared },
            new object?[] { "  help", false, null },
            new object?[] { "Abc", false, null },
            new object?[] { "  ", false, null },
            new object?[] { "", false, null },
        };
    }
}