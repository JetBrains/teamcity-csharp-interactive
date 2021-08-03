namespace Teamcity.CSharpInteractive.Tests
{
    using System.Linq;
    using Moq;
    using Shouldly;
    using Xunit;

    public class LoadCommandFactoryTests
    {
        private readonly Mock<IFileCodeSourceFactory> _fileCodeSourceFactory;
        private readonly Mock<ICommandFactory<ICodeSource>> _commandFactory;
        
        public LoadCommandFactoryTests()
        {
            var codeSource = Mock.Of<ICodeSource>();

            _fileCodeSourceFactory = new Mock<IFileCodeSourceFactory>();
            _fileCodeSourceFactory.Setup(i => i.Create("Abc.csx")).Returns(codeSource);
            
            _commandFactory = new Mock<ICommandFactory<ICodeSource>>();
            _commandFactory.Setup(i => i.Create(codeSource)).Returns(new[]{Mock.Of<ICommand>()});
        }

        [Theory]
        [InlineData("#load \"Abc.csx\"", true)]
        [InlineData("   #load \"Abc.csx\"    ", true)]
        [InlineData("   #load \"Abc.csx  ", false)]
        [InlineData("   #load Abc.csx\"    ", false)]
        [InlineData("#load Abc.csx", false)]
        [InlineData("#load   Abc.csx   ", false)]
        [InlineData("#l \"Abc.csx\"", false)]
        [InlineData("load \"Abc.csx\"", false)]
        public void Should(string replCommand, bool expectedAccepted)
        {
            // Given
            var factory = CreateInstance();

            // When
            var actualAccepted = factory.Create(replCommand).Any();

            // Then
            actualAccepted.ShouldBe(expectedAccepted);
        }

        private LoadCommandFactory CreateInstance() =>
            new(_fileCodeSourceFactory.Object, () => _commandFactory.Object);
    }
}