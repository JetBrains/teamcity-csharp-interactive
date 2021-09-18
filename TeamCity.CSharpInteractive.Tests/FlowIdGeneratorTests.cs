namespace TeamCity.CSharpInteractive.Tests
{
    using Moq;
    using Shouldly;
    using Xunit;

    public class FlowIdGeneratorTests
    {
        private readonly Mock<ITeamCitySettings> _teamCitySettings;

        public FlowIdGeneratorTests() => 
            _teamCitySettings = new Mock<ITeamCitySettings>();

        [Fact]
        public void ShouldProvideRootFlowIdWhenItIsSpecified()
        {
            // Given
            var generator = CreateInstance();
            _teamCitySettings.SetupGet(i => i.FlowId).Returns("root");

            // When
            var flowId = generator.NewFlowId();

            // Then
            flowId.ShouldBe("root");
        }
        
        [Fact]
        public void ShouldProvideNexFlowIdAfterRootFlowIdWhenItIsSpecified()
        {
            // Given
            var generator = CreateInstance();
            _teamCitySettings.SetupGet(i => i.FlowId).Returns("root");

            // When
            generator.NewFlowId();
            var flowId = generator.NewFlowId();

            // Then
            flowId.ShouldNotBe("root");
        }
        
        [Fact]
        public void ShouldProvideFlowIdWhenRootFlowIdIsNotSpecified()
        {
            // Given
            var generator = CreateInstance();

            // When
            var flowId = generator.NewFlowId();

            // Then
            flowId.ShouldNotBe("root");
        }
        
        [Fact]
        public void ShouldProvideGenerateFlowIds()
        {
            // Given
            var generator = CreateInstance();

            // When
            var flowId1 = generator.NewFlowId();
            var flowId2 = generator.NewFlowId();

            // Then
            flowId1.ShouldNotBe(flowId2);
        }

        private FlowIdGenerator CreateInstance() =>
            new(_teamCitySettings.Object);
    }
}