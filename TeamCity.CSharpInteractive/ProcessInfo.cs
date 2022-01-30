namespace TeamCity.CSharpInteractive;

using HostApi;
using Immutype;

[Target]
internal record ProcessInfo(IStartInfo StartInfo, IProcessMonitor Monitor, Action<Output>? Handler = default);