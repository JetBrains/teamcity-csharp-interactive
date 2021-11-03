#l diagnostic
var result = await GetService<ICommandLine>().RunAsync(new CommandLine("whoami.exe").AddArgs("/all"), i => WriteLine(i.Line));
WriteLine(result);
