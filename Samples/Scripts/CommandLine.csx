//#l diagnostic
using Cmd;

var result = GetService<ICommandLine>().Run(new CommandLine("whoami.exe", "/all"));
WriteLine(result);