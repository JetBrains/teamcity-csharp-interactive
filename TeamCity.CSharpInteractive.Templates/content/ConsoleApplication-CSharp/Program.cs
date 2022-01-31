using HostApi;
using static Host;

var runner = GetService<IBuildRunner>();
runner.Run(new DotNetBuild());