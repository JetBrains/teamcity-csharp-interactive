namespace TeamCity.CSharpInteractive;

internal interface IProcessRunner
{
    ProcessResult Run(ProcessInfo processInfo, TimeSpan timeout);

    Task<ProcessResult> RunAsync(ProcessInfo processInfo, CancellationToken cancellationToken);
}