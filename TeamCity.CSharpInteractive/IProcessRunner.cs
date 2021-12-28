namespace TeamCity.CSharpInteractive
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Cmd;

    internal interface IProcessRunner
    {
        int? Run(IStartInfo startInfo, IProcessState state, Action<Output>? handler = default, TimeSpan timeout = default);

        Task<int?> RunAsync(IStartInfo startInfo, IProcessState state, Action<Output>? handler = default, CancellationToken cancellationToken = default);
    }
}