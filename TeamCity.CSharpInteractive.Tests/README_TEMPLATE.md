
## Usage Scenarios

- NuGet API
  - [Restore NuGet a package of newest version](#restore-nuget-a-package-of-newest-version)
  - [Restore NuGet the package of version in the range for the specified .NET to a path](#restore-nuget-the-package-of-version-in-the-range-for-the-specified-.net-to-a-path)
- Command Line API
  - [Run](#run)
  - [Cancellation of asynchronous run](#cancellation-of-asynchronous-run)
  - [Run asynchronously](#run-asynchronously)
  - [Run and process output](#run-and-process-output)
  - [Run asynchronously in parallel](#run-asynchronously-in-parallel)

### Restore NuGet a package of newest version



``` CSharp
var packagesPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString()[..4]); 
var packages = GetService<INuGet>().Restore("IoC.Container");
```



### Restore NuGet the package of version in the range for the specified .NET to a path



``` CSharp
var packagesPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString()[..4]); 
var packages = GetService<INuGet>().Restore("IoC.Container", "[1.3, 1.3.8)", "net5.0", packagesPath);
```



### Run



``` CSharp
var exitCode = GetService<ICommandLine>().Run(new CommandLine("whoami", "/all"));
```



### Run asynchronously



``` CSharp
var exitCode = await GetService<ICommandLine>().RunAsync(new CommandLine("whoami", "/all"));
```



### Cancellation of asynchronous run



``` CSharp
var cancellationTokenSource = new CancellationTokenSource();
var task = GetService<ICommandLine>().RunAsync(new CommandLine("cmd", "TIMEOUT", "/T",  "120"), default, cancellationTokenSource.Token);

cancellationTokenSource.CancelAfter(TimeSpan.FromMilliseconds(100));
task.IsCompleted.ShouldBeFalse();
```



### Run and process output



``` CSharp
var lines = new System.Collections.Generic.List<string>();
var exitCode = GetService<ICommandLine>().Run(
    new CommandLine("cmd").AddArgs("/c", "SET").AddVars(("MyEnv", "MyVal")),
    i => lines.Add(i.Line));

lines.ShouldContain("MyEnv=MyVal");
```



### Run asynchronously in parallel



``` CSharp
var task = GetService<ICommandLine>().RunAsync(new CommandLine("whoami").AddArgs("/all"));
var exitCode = GetService<ICommandLine>().Run(new CommandLine("cmd", "/c", "SET"));
task.Wait();
```



