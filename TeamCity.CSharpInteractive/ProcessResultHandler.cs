// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using HostApi;

internal class ProcessResultHandler : IProcessResultHandler
{
    private readonly ILog<ProcessResultHandler> _log;

    public ProcessResultHandler(ILog<ProcessResultHandler> log) => _log = log;

    public void Handle<T>(ProcessResult result, Action<T>? handler)
    {
        var description = result.Description;
        if (result.Error != default)
        {
            description = description + Text.Space + new Text(result.Error.Message);
        }

        if (handler == default)
        {
            switch (result.State)
            {
                case ProcessState.Failed:
                    _log.Error(ErrorId.Process, description.WithDefaultColor(Color.Default));
                    break;

                case ProcessState.Canceled:
                    _log.Warning(description.WithDefaultColor(Color.Default));
                    break;

                case ProcessState.Finished:
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