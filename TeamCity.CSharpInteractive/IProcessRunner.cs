namespace TeamCity.CSharpInteractive;

internal interface IProcessRunner
{
    ProcessResult Run(ProcessRun processRun, TimeSpan timeout);

    Task<ProcessResult> RunAsync(ProcessRun processRun, CancellationToken cancellationToken);
}