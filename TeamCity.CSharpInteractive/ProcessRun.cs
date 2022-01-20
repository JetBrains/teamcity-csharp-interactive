namespace TeamCity.CSharpInteractive;

using Cmd;

[Immutype.Target]
internal record ProcessRun(IStartInfo StartInfo, IProcessMonitor Monitor, Action<Output>? Handler = default);