namespace TeamCity.CSharpInteractive;

using HostApi;

internal static class ProcessResultDescriptionHelper
{
    public static IEnumerable<Text> GetProcessResultDescription(ProcessResult processResult)
    {
        string stateText = processResult.State switch
        {
            ProcessState.FailedToStart => processResult.ExitCode.HasValue ? "failed" : "failed to start",
            ProcessState.CancelledByTimeout => "canceled",
            ProcessState.RanToCompletion => "finished",
            _ => throw new NotImplementedException()
        };

        yield return new Text($"{processResult.StartInfo.GetDescription(processResult.ProcessId)} process ", Color.Highlighted);
        yield return new Text(stateText, Color.Highlighted);
        yield return new Text($" (in {processResult.ElapsedMilliseconds} ms)");

        if (processResult.ExitCode.HasValue)
        {
            yield return new Text($" with exit code {processResult.ExitCode}");
        }

        yield return new Text(".");
    }
}
