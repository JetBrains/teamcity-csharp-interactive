namespace TeamCity.CSharpInteractive.Tests;

public class SettingCommandParserTests
{
    [Theory]
    [MemberData(nameof(Data))]
    internal void ShouldCreate(string replCommand, ICommand[] expectedCommands)
    {
        // Given
        var stringService = new Mock<IStringService>();
        stringService.Setup(i => i.TrimAndUnquote(It.IsAny<string>())).Returns<string>(i => i);
        var parser = new SettingCommandFactory<VerbosityLevel>(Mock.Of<ILog<SettingCommandFactory<VerbosityLevel>>>(),  stringService.Object, new [] {new VerbosityLevelSettingDescription()});
            
        // When
        var actualResult = parser.Create(replCommand);

        // Then
        actualResult.ToArray().ShouldBe(expectedCommands);
    }
        
    public static IEnumerable<object[]> Data => new List<object[]> 
    {
        new object[] { "#l diagnostic", new [] { new SettingCommand<VerbosityLevel>(VerbosityLevel.Diagnostic) } },
        new object[] { "#l   Diagnostic", new [] { new SettingCommand<VerbosityLevel>(VerbosityLevel.Diagnostic) } },
        new object[] { "#L Diagnostic", new [] { new SettingCommand<VerbosityLevel>(VerbosityLevel.Diagnostic) } },
        new object[] { "#  l   Diagnostic", Array.Empty<ICommand>() },
        new object[] { "#lDiagnostic", Array.Empty<ICommand>() },
        new object[] { "#l ", Array.Empty<ICommand>() },
        new object[] { "#Diagnostic", Array.Empty<ICommand>() },
        new object[] { "#  ", Array.Empty<ICommand>() },
        new object[] { "#", Array.Empty<ICommand>() },
        new object[] { "", Array.Empty<ICommand>() }
    };
}