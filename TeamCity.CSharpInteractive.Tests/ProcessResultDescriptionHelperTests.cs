namespace TeamCity.CSharpInteractive.Tests;

using HostApi;

public class ProcessResultDescriptionHelperTests
{
    [Fact]
    public void ShouldReturnExpectedDescriptionWhenProcessRanToCompletion()
    {
        // Given
        var processResult = ProcessResult.RanToCompletion(Mock.Of<IStartInfo>(), 99, 22, 33);

        // When
        var description = ProcessResultDescriptionHelper.GetProcessResultDescription(processResult);

        // Then
        description.ShouldBe(
            new Text[] {new("99 process ", Color.Highlighted), new("finished", Color.Highlighted), new(" (in 22 ms)"), new(" with exit code 33"), new(".")}
        );
    }

    [Fact]
    public void ShouldReturnExpectedDescriptionWhenProcessFailedToStart()
    {
        // Given
        var processResult = ProcessResult.FailedToStart(Mock.Of<IStartInfo>(), 22, null);

        // When
        var description = ProcessResultDescriptionHelper.GetProcessResultDescription(processResult);

        // Then
        description.ShouldBe(
            new Text[] {new("The process ", Color.Highlighted), new("failed to start", Color.Highlighted), new(" (in 22 ms)"), new(".")}
        );
    }

    [Fact]
    public void ShouldReturnExpectedDescriptionWhenProcessWasCanceledByTimeout()
    {
        // Given
        var processResult = ProcessResult.CancelledByTimeout(Mock.Of<IStartInfo>(), 99, 22);

        // When
        var description = ProcessResultDescriptionHelper.GetProcessResultDescription(processResult);

        // Then
        description.ShouldBe(
            new Text[] {new("99 process ", Color.Highlighted), new("canceled", Color.Highlighted), new(" (in 22 ms)"), new(".")}
        );
    }
}
