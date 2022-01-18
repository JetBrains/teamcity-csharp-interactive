namespace TeamCity.CSharpInteractive
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal interface IProcessRunner
    {
        ProcessResult Run(ProcessRun processRun, TimeSpan timeout);

        Task<ProcessResult> RunAsync(ProcessRun processRun, CancellationToken cancellationToken);
    }
}