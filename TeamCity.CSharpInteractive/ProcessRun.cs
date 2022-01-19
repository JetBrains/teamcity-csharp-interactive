namespace TeamCity.CSharpInteractive
{

    using System;
    using Cmd;

    [Immutype.Target]
    internal record ProcessRun(IStartInfo StartInfo, IProcessMonitor Monitor, Action<Output>? Handler = default);
}