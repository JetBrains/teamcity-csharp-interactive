namespace Teamcity.CSharpInteractive.Tests
{
    using Moq;
    using Xunit;

    public class TracePresenterTests
    {
        [Fact]
        public void ShouldShowTrace()
        {
            // Given
            var log = new Mock<ILog<TracePresenter>>();
            var presenter = new TracePresenter(log.Object);
            var src1 = new Mock<ITraceSource>();
            var text11 = new Text("a");
            var text12 = new Text("b");
            src1.Setup(i => i.GetTrace()).Returns(new [] {text11, text12});
            var src2 = new Mock<ITraceSource>();
            var text2 = new Text("c");
            src2.Setup(i => i.GetTrace()).Returns(new [] {text2});

            // When
            presenter.Show(new []{src1.Object, src2.Object});

            // Then
            log.Verify(i => i.Trace(new []{text11}));
            log.Verify(i => i.Trace(new []{text12}));
            log.Verify(i => i.Trace(new []{text2}));
        }
    }
}