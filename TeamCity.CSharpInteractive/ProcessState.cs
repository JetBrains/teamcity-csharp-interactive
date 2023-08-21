namespace TeamCity.CSharpInteractive;

internal enum ProcessState
{
    RanToCompletion,
    FailedToStart,
    CancelledByTimeout
}