using HostApi;

var runner = GetService<IBuildRunner>();
runner.Run(new DotNetBuild());