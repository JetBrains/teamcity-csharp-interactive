namespace TeamCity.CSharpInteractive.Tests;

public class TracePresenterTests
{
    private readonly Mock<ILog<TracePresenter>> _log = new();

    [Fact]
    public void ShouldShowTraceFromSources()
    {
        // Given
        var presenter = CreateInstance();
        
        var source1 = new Mock<ITraceSource>();
        var text11 = new Text("a");
        var text12 = new Text("a");
        source1.SetupGet(i => i.Trace).Returns(new[] { text11, text12 });
        
        var source2 = new Mock<ITraceSource>();
        var text21 = new Text("c");
        source2.SetupGet(i => i.Trace).Returns(new[] { text21 });

        // When
        presenter.Show(new []{ source1.Object, source2.Object });

        // Then
        _log.Verify(i => i.Trace(It.Is<Func<Text[]>>(func => func().SequenceEqual(new []{ text11, text12, text21 })), string.Empty));
    }

    private TracePresenter CreateInstance() =>
        new(_log.Object);
}