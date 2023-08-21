namespace TeamCity.CSharpInteractive;

using HostApi;

internal record ProcessResult
{
    public IStartInfo StartInfo { get; init; }
    public int? ProcessId { get; init; }
    public ProcessState State { get; init; }
    public long ElapsedMilliseconds { get; init; }
    public int? ExitCode { get; init; }
    public Exception? Error { get; init; }

    private ProcessResult(IStartInfo startInfo,
        int? processId,
        ProcessState state,
        long elapsedMilliseconds,
        int? exitCode = default,
        Exception? error = default)
    {
        StartInfo = startInfo;
        ProcessId = processId;
        State = state;
        ElapsedMilliseconds = elapsedMilliseconds;
        ExitCode = exitCode;
        Error = error;
    }

    public static ProcessResult RanToCompletion(
        IStartInfo startInfo, int processId, long elapsedMilliseconds, int exitCode)
            => new(startInfo, processId, ProcessState.RanToCompletion, elapsedMilliseconds, exitCode);

    public static ProcessResult FailedToStart(
        IStartInfo startInfo, long elapsedMilliseconds, Exception? error)
            => new(startInfo, null, ProcessState.FailedToStart, elapsedMilliseconds, error: error);

    public static ProcessResult CancelledByTimeout(
        IStartInfo startInfo, int processId, long elapsedMilliseconds)
            => new(startInfo, processId, ProcessState.CancelledByTimeout, elapsedMilliseconds);
}
