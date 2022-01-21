namespace TeamCity.CSharpInteractive;

using Script.Cmd;

[Immutype.Target]
internal record ProcessInfo(IStartInfo StartInfo, IProcessMonitor Monitor, Action<Output>? Handler = default);