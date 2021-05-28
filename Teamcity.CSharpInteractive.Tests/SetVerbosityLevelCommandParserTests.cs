namespace Teamcity.CSharpInteractive.Tests
{
    using System.Collections.Generic;
    using Moq;
    using Shouldly;
    using Xunit;

    public class SetVerbosityLevelCommandParserTests
    {
        [Theory]
        [MemberData(nameof(Data))]
        internal void ShouldParse(string replCommand, bool expectedResult, ICommand? expectedCommand)
        {
            // Given
            var stringService = new Mock<IStringService>();
            stringService.Setup(i => i.TrimAndUnquote(It.IsAny<string>())).Returns<string>(i => i);
            var parser = new SetVerbosityLevelCommandParser(Mock.Of<ILog<SetVerbosityLevelCommandParser>>(),  stringService.Object);
            
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
            new object[] { "l trace", true, new SetVerbosityLevelCommand(VerbosityLevel.Trace) },
            new object[] { "l   Trace", true, new SetVerbosityLevelCommand(VerbosityLevel.Trace) },
            new object[] { "L Trace", true, new SetVerbosityLevelCommand(VerbosityLevel.Trace) },
            new object?[] { "  l   Trace", false, null },
            new object?[] { "lTrace", false, null },
            new object?[] { "l ", false, null },
            new object?[] { "Trace", false, null },
            new object?[] { "  ", false, null },
            new object?[] { "", false, null },
        };
    }
}