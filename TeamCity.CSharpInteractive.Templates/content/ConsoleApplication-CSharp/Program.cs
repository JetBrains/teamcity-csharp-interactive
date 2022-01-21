using DotNet;

var build = GetService<IBuild>();
var buildStep = new Build()
    .WithConfiguration("Release");

var result = build.Run(buildStep);
if (result.ExitCode != 0)
{
    Error(result.ToString());
}