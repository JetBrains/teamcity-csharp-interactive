// Run this from the working directory where the solution or project to build is located.
using HostApi;

return new DotNetBuild().Build().ExitCode ?? 1;