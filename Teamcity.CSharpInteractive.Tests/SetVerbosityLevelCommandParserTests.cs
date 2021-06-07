namespace Teamcity.CSharpInteractive.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Moq;
    using Shouldly;
    using Xunit;

    public class SetVerbosityLevelCommandParserTests
    {
        [Theory]
        [MemberData(nameof(Data))]
        internal void ShouldCreate(string replCommand, ICommand[] expectedCommands)
        {
            // Given
            var stringService = new Mock<IStringService>();
            stringService.Setup(i => i.TrimAndUnquote(It.IsAny<string>())).Returns<string>(i => i);
            var parser = new SetVerbosityLevelCommandFactory(Mock.Of<ILog<SetVerbosityLevelCommandFactory>>(),  stringService.Object);
            
            // When
            var actualResult = parser.Create(replCommand);

            // Then
            actualResult.ToArray().ShouldBe(expectedCommands);
        }
        
        public static IEnumerable<object[]> Data => new List<object[]> 
        {
            new object[] { "#l trace", new [] { new SetVerbosityLevelCommand(VerbosityLevel.Trace) } },
            new object[] { "#l   Trace", new [] { new SetVerbosityLevelCommand(VerbosityLevel.Trace) } },
            new object[] { "#L Trace", new [] { new SetVerbosityLevelCommand(VerbosityLevel.Trace) } },
            new object[] { "#  l   Trace", Array.Empty<ICommand>() },
            new object[] { "#lTrace", Array.Empty<ICommand>() },
            new object[] { "#l ", Array.Empty<ICommand>() },
            new object[] { "#Trace", Array.Empty<ICommand>() },
            new object[] { "#  ", Array.Empty<ICommand>() },
            new object[] { "#", Array.Empty<ICommand>() },
            new object[] { "", Array.Empty<ICommand>() }
        };
    }
}