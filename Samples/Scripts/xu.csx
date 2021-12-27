using Dotnet;

var build = GetService<IBuild>();
var result = build.Run(new Test().WithWorkingDirectory(@"C:\Projects\_temp\xu"));
WriteLine(result);