namespace Teamcity.CSharpInteractive.Tests
{
    using System.Collections.Generic;
    using Moq;
    using Shouldly;
    using Xunit;

    public class AddReferenceCommandParserTests
    {
        [Theory]
        [MemberData(nameof(Data))]
        internal void ShouldParse(string replCommand, bool expectedResult, ICommand? expectedCommand)
        {
            // Given
            var stringService = new Mock<IStringService>();
            stringService.Setup(i => i.TrimAndUnquote(It.IsAny<string>())).Returns<string>(i => i);
            var parser = new AddReferenceCommandParser(Mock.Of<ILog<AddReferenceCommandParser>>(),  stringService.Object);
            
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
            new object[] { "r Abc", true, new AddReferenceCommand("Abc") },
            new object[] { "r   Abc", true, new AddReferenceCommand("Abc") },
            new object[] { "r \"Abc\"", true, new AddReferenceCommand("\"Abc\"") },
            new object[] { "R Abc", true, new AddReferenceCommand("Abc") },
            new object?[] { "  r   Abc", false, null },
            new object?[] { "rAbc", false, null },
            new object?[] { "r ", false, null },
            new object?[] { "Abc", false, null },
            new object?[] { "  ", false, null },
            new object?[] { "", false, null },
        };
    }
}