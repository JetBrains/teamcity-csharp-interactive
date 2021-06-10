namespace Teamcity.CSharpInteractive.Tests
{
    using JetBrains.TeamCity.ServiceMessages.Write.Special;
    using Moq;
    using Shouldly;
    using Xunit;

    public class HierarchicalTeamCityWriterTests
    {
        private readonly Mock<ITeamCityWriter> _teamCityRootWriter;

        public HierarchicalTeamCityWriterTests() => _teamCityRootWriter = new Mock<ITeamCityWriter>();

        [Fact]
        public void ShouldInitializeCurrentWriterByRootWriterOnInit()
        {
            // Given

            // When
            var writer = CreateInstance();

            // Then
            writer.CurrentWriter.ShouldBe(_teamCityRootWriter.Object);
        }
        
        [Fact]
        public void ShouldUseBlockWriterAsCurrent()
        {
            // Given
            var blockWriter = new Mock<ITeamCityWriter>();
            var writer = CreateInstance();
            _teamCityRootWriter.Setup(i => i.OpenBlock("Abc")).Returns(blockWriter.Object);

            // When
            writer.OpenBlock("Abc");

            // Then
            writer.CurrentWriter.ShouldBe(blockWriter.Object);
        }
        
        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        public void ShouldSkipOpeningBlockForInvalidBlockName(string blockName)
        {
            // Given
            var writer = CreateInstance();

            // When
            writer.OpenBlock(blockName);

            // Then
            _teamCityRootWriter.Verify(i => i.OpenBlock(It.IsAny<string>()), Times.Never);
        }
        
        [Fact]
        public void ShouldRollbackWriterAfterBlockClosed()
        {
            // Given
            var blockWriter = new Mock<ITeamCityWriter>();
            var writer = CreateInstance();
            _teamCityRootWriter.Setup(i => i.OpenBlock("Abc")).Returns(blockWriter.Object);
            var blockToken = writer.OpenBlock("Abc");

            // When
            blockToken.Dispose();

            // Then
            writer.CurrentWriter.ShouldBe(_teamCityRootWriter.Object);
        }
        
        [Fact]
        public void ShouldSupportNestedBlocks()
        {
            // Given
            var blockWriter1 = new Mock<ITeamCityWriter>();
            var blockWriter2 = new Mock<ITeamCityWriter>();
            var writer = CreateInstance();
            _teamCityRootWriter.Setup(i => i.OpenBlock("Abc")).Returns(blockWriter1.Object);
            blockWriter1.Setup(i => i.OpenBlock("Xyz")).Returns(blockWriter2.Object);

            // When
            
            // Then
            var blockToken1= writer.OpenBlock("Abc");
            writer.CurrentWriter.ShouldBe(blockWriter1.Object);

            var blockToken2 = writer.OpenBlock("Xyz");
            writer.CurrentWriter.ShouldBe(blockWriter2.Object);
            
            blockToken2.Dispose();
            writer.CurrentWriter.ShouldBe(blockWriter1.Object);
            
            blockToken1.Dispose();
            writer.CurrentWriter.ShouldBe(_teamCityRootWriter.Object);
        }
        
        [Fact]
        public void ShouldSupportWriteError()
        {
            // Given
            var writer = CreateInstance();

            // When
            writer.WriteError("err", "details");

            // Then
            _teamCityRootWriter.Verify(i => i.WriteError("err", "details"));
        }
        
        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void ShouldNotWriteEmptyError(string error)
        {
            // Given
            var writer = CreateInstance();

            // When
            writer.WriteError(error, "details");

            // Then
            _teamCityRootWriter.Verify(i => i.WriteError(It.IsAny<string>(), "details"), Times.Never);
        }
        
        [Fact]
        public void ShouldSupportWriteWarning()
        {
            // Given
            var writer = CreateInstance();

            // When
            writer.WriteWarning("warn");

            // Then
            _teamCityRootWriter.Verify(i => i.WriteWarning("warn"));
        }
        
        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void ShouldNotWriteEmptyWarning(string warning)
        {
            // Given
            var writer = CreateInstance();

            // When
            writer.WriteWarning(warning);

            // Then
            _teamCityRootWriter.Verify(i => i.WriteWarning(It.IsAny<string>()), Times.Never);
        }
        
        [Fact]
        public void ShouldSupportWriteMessage()
        {
            // Given
            var writer = CreateInstance();

            // When
            writer.WriteMessage("msg");

            // Then
            _teamCityRootWriter.Verify(i => i.WriteMessage("msg"));
        }

        private HierarchicalTeamCityWriter CreateInstance() => new(_teamCityRootWriter.Object);
    }
}