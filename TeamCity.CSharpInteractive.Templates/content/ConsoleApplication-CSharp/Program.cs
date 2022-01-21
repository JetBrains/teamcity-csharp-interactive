using Script.DotNet;

var runner = GetService<IBuildRunner>();
runner.Run(new Build());