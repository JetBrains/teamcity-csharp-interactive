namespace TeamCity.CSharpInteractive.Tests;

using Script;

public class SummaryPresenterTests
{
    private readonly Mock<ILog<SummaryPresenter>> _log = new();
    private readonly Mock<IStatistics> _statistics = new();
    private readonly Mock<IPresenter<IStatistics>> _statisticsPresenter = new();

    [Theory]
    [InlineData(true, false, false)]
    [InlineData(true, true, true)]
    [InlineData(false, false, true)]
    [InlineData(false, true, true)]
    public void ShouldSummary(bool? success, bool hasError, bool showError)
    {
        // Given
        var presenter = CreateInstance();

        // When
        if (hasError)
        {
            _statistics.SetupGet(i => i.Errors).Returns(new[] {"Err"});
        }
        else
        {
            _statistics.SetupGet(i => i.Errors).Returns(Array.Empty<string>());
        }

        presenter.Show(new Summary(success));

        // Then
        _statisticsPresenter.Verify(i => i.Show(_statistics.Object));
        var state = showError
            ? new Text("Running FAILED.", Color.Error)
            : new Text("Running succeeded.", Color.Success);
        _log.Verify(i => i.Info(state));
    }

    private SummaryPresenter CreateInstance() =>
        new(_log.Object, _statistics.Object, _statisticsPresenter.Object);
}