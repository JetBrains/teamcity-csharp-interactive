namespace TeamCity.CSharpInteractive
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Cmd;

    internal interface IProcessRunner
    {
        int? Run(IStartInfo startInfo, Action<Output>? handler, IProcessStateProvider? stateProvider, IProcessMonitor monitor, TimeSpan timeout);

        Task<int?> RunAsync(IStartInfo startInfo, Action<Output>? handler, IProcessStateProvider? stateProvider, IProcessMonitor monitor, CancellationToken cancellationToken);
    }
}