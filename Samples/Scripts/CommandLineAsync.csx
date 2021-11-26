#l diagnostic
using Cmd;

var result1 = GetService<ICommandLine>().RunAsync(new CommandLine("whoami.exe").AddArgs("/all"), i => WriteLine(i.Line));
var result2 = GetService<ICommandLine>().RunAsync(new CommandLine("whoami.exe").AddArgs("/all"), i => WriteLine(i.Line));
WriteLine(result1.Result);
WriteLine(result2.Result);
