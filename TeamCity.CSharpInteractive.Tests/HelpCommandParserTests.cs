namespace TeamCity.CSharpInteractive.Tests;

public class HelpCommandParserTests
{
    [Theory]
    [MemberData(nameof(Data))]
    internal void ShouldHelp(string replCommand, ICommand[] expectedCommands)
    {
        // Given
        var stringService = new Mock<IStringService>();
        stringService.Setup(i => i.TrimAndUnquote(It.IsAny<string>())).Returns<string>(i => i);
        var parser = new HelpCommandFactory(Mock.Of<ILog<HelpCommandFactory>>());
            
        // When
        var actualResult = parser.Create(replCommand);

        // Then
        actualResult.ShouldBe(expectedCommands);
    }
        
    public static IEnumerable<object[]> Data => new List<object[]> 
    {
        new object[] { "#help", new [] { HelpCommand.Shared } },
        new object[] { "#Help", new [] { HelpCommand.Shared } },
        new object[] { "#help ", new [] { HelpCommand.Shared } },
        new object[] { "#  help", Array.Empty<ICommand>() },
        new object[] { "#Abc", Array.Empty<ICommand>() },
        new object[] { "#  ", Array.Empty<ICommand>() },
        new object[] { "#", Array.Empty<ICommand>() },
        new object[] { "", Array.Empty<ICommand>() }
    };
}