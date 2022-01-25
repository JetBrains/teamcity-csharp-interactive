namespace TeamCity.CSharpInteractive;

using HostApi;

[Immutype.Target]
internal record ProcessInfo(IStartInfo StartInfo, IProcessMonitor Monitor, Action<Output>? Handler = default);