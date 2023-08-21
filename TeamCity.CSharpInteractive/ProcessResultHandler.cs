// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using HostApi;

internal class ProcessResultHandler : IProcessResultHandler
{
    private readonly ILog<ProcessResultHandler> _log;
    private readonly IExitTracker _exitTracker;

    public ProcessResultHandler(
        ILog<ProcessResultHandler> log,
        IExitTracker exitTracker)
    {
        _log = log;
        _exitTracker = exitTracker;
    }

    public void Handle<T>(ProcessResult result, Action<T>? handler)
    {
        if (_exitTracker.IsTerminating)
        {
            return;
        }

        var description = ProcessResultDescriptionHelper.GetProcessResultDescription(result).ToArray();
        if (result.Error != default)
        {
            description = description + Text.Space + new Text(result.Error.Message);
        }

        if (handler == default)
        {
            switch (result.State)
            {
                case ProcessState.FailedToStart:
                    _log.Error(ErrorId.Process, description.WithDefaultColor(Color.Default));
                    break;

                case ProcessState.CancelledByTimeout:
                    _log.Warning(description.WithDefaultColor(Color.Default));
                    break;

                case ProcessState.RanToCompletion:
                default:
                    _log.Info(description);
                    break;
            }
        }
        else
        {
            _log.Info(description);
        }
    }
}
