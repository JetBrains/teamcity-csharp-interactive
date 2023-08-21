namespace TeamCity.CSharpInteractive.Tests;

using HostApi;
using Xunit;

public class ProcessResultHandlerTests
{
    private readonly Mock<ILog<ProcessResultHandler>> _log = new();
    private readonly Mock<IStartInfo> _startInfo = new();
    private readonly Mock<IExitTracker> _exitTracker = new();
    private readonly Action<object> _handler = Mock.Of<Action<object>>();
    
    [Fact]
    internal void ShouldLogInfoWhenRanToCompletionAndHasHandler()
    {
        // Given
        var handler = CreateInstance();

        // When
        handler.Handle(ProcessResult.RanToCompletion(_startInfo.Object, 0, 12, 0), _handler);

        // Then
        _log.Verify(i => i.Info(It.IsAny<Text[]>()));
    }

    [Fact]
    public void ShouldLogInfoWhenRanToCompletionAndHasNoHandler()
    {
        // Given
        var handler = CreateInstance();

        // When
        handler.Handle(ProcessResult.RanToCompletion(_startInfo.Object, 0, 12, 0), _handler);

        // Then
        _log.Verify(i => i.Info(It.IsAny<Text[]>()));
    }

    [Fact]
    public void ShouldLogWarningWhenCanceledAndHasNoHandler()
    {
        // Given
        var handler = CreateInstance();

        // When
        handler.Handle(ProcessResult.CancelledByTimeout(_startInfo.Object, 0, 12), default(Action<object>));

        // Then
        _log.Verify(i => i.Warning(It.IsAny<Text[]>()));
    }
    
    [Fact]
    public void ShouldNotLogWarningWhenCancelledByTimeoutAndTerminating()
    {
        // Given
        var handler = CreateInstance();

        // When
        _exitTracker.SetupGet(i => i.IsTerminating).Returns(true);
        handler.Handle(ProcessResult.CancelledByTimeout(_startInfo.Object, 0, 12), default(Action<object>));

        // Then
        _log.Verify(i => i.Warning(It.IsAny<Text[]>()), Times.Never);
    }
    
    [Fact]
    public void ShouldLogErrorWhenFailedToStartAndHasNoHandler()
    {
        // Given
        var handler = CreateInstance();

        // When
        handler.Handle(ProcessResult.FailedToStart(_startInfo.Object, 0, null), default(Action<object>));

        // Then
        _log.Verify(i => i.Error(ErrorId.Process, It.IsAny<Text[]>()));
    }
    
    [Fact]
    public void ShouldLogErrorWhenFailedToStartAndHasNoHandlerAndHasError()
    {
        // Given
        var handler = CreateInstance();
        var error = new Exception("Some error.");

        // When
        handler.Handle(ProcessResult.FailedToStart(_startInfo.Object, 0, error), default(Action<object>));

        // Then
        _log.Verify(i => i.Error(ErrorId.Process, It.Is<Text[]>(x => x.Last().Value == error.Message)));
    }

    private ProcessResultHandler CreateInstance() =>
        new(_log.Object, _exitTracker.Object);
}
