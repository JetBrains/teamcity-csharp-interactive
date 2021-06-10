namespace Teamcity.CSharpInteractive.Tests
{
    using Moq;
    using Xunit;

    public class FlushableRegistryTests
    {
        [Fact]
        public void ShouldFlushRegistered()
        {
            // Given
            var flushable1 = new Mock<IFlushable>();
            var flushable2 = new Mock<IFlushable>();
            var registry = new FlushableRegistry();
            registry.Register(flushable1.Object);
            registry.Register(flushable2.Object);

            // When
            registry.Flush();
            registry.Flush();
            
            // Then
            flushable1.Verify(i => i.Flush(), Times.Once);
            flushable2.Verify(i => i.Flush(), Times.Once);
        }
    }
}