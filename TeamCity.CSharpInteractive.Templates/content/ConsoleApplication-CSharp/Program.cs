// Run this project from the working directory where a target solution or project is located.
using HostApi;

var runner = GetService<IBuildRunner>();
runner.Run(new DotNetBuild());