//#l diagnostic
using Cmd;

var result = GetService<ICommandLine>().Run(new CommandLine("wsl", "whoami"));
WriteLine(result);