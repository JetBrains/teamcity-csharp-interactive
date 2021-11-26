using Cmd;

var cmd = new CommandLine("whoami");
var dockerCmd = new Docker.Run("ubuntu:20.04", cmd);
GetService<ICommandLine>().Run(dockerCmd);