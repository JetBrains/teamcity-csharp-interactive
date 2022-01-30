namespace TeamCity.CSharpInteractive.Tests;

public class CommandSourceTests
{
    [Fact]
    public void ShouldProvideCommands()
    {
        // Given
        var codeSource1 = new Mock<ICodeSource>();
        var codeSource2 = new Mock<ICodeSource>();

        var settings = new Mock<ISettings>();
        settings.SetupGet(i => i.CodeSources).Returns(new[] {codeSource1.Object, codeSource2.Object});

        var factory = new Mock<ICommandFactory<ICodeSource>>();
        factory.Setup(i => i.Create(codeSource1.Object)).Returns(new[] {HelpCommand.Shared});
        factory.Setup(i => i.Create(codeSource2.Object)).Returns(new[] {ResetCommand.Shared});

        var source = new CommandSource(settings.Object, factory.Object);

        // When
        var actualCommands = source.GetCommands().ToArray();

        // Then
        actualCommands.ShouldBe(new[] {HelpCommand.Shared, ResetCommand.Shared});
    }
}