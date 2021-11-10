
## Usage Scenarios

- NuGet API
  - [Restore NuGet a package of newest version](#restore-nuget-a-package-of-newest-version)
  - [Restore a NuGet package by a version range for the specified .NET and path](#restore-a-nuget-package-by-a-version-range-for-the-specified-.net-and-path)
- Command Line API
  - [Build command lines](#build-command-lines)
  - [Run a command line](#run-a-command-line)
  - [Run a command line asynchronously](#run-a-command-line-asynchronously)
  - [Run and process output](#run-and-process-output)
  - [Run asynchronously in parallel](#run-asynchronously-in-parallel)
  - [Cancellation of asynchronous run](#cancellation-of-asynchronous-run)
  - [Run timeout](#run-timeout)
- TeamCity Service Messages API
  - [TeamCity integration via service messages](#teamcity-integration-via-service-messages)

### Restore NuGet a package of newest version



``` CSharp
IEnumerable<NuGetPackage> packages = GetService<INuGet>().Restore("IoC.Container");
```



### Restore a NuGet package by a version range for the specified .NET and path



``` CSharp
var packagesPath = System.IO.Path.Combine(
    System.IO.Path.GetTempPath(),
    Guid.NewGuid().ToString()[..4]);

IEnumerable<NuGetPackage> packages = GetService<INuGet>().Restore(
    "IoC.Container",
    "[1.3, 1.3.8)",
    "net5.0",
    packagesPath);
```



### Build command lines



``` CSharp

// Creates a simple command line from just the name of the executable 
new CommandLine("whoami");

// Creates a command line with multiple command line arguments 
new CommandLine("cmd", "/c", "echo", "Hello");

// Same as previous statement
new CommandLine("cmd", "/c")
    .AddArgs("echo", "Hello");

// Builds a command line with multiple environment variables
new CommandLine("cmd", "/c", "echo", "Hello")
    .AddVars(("Var1", "val1"), ("var2", "Val2"));

// Builds a command line to run from a specific working directory 
new CommandLine("cmd", "/c", "echo", "Hello")
    .WithWorkingDirectory("MyDyrectory");

// Builds a command line and replaces all command line arguments
new CommandLine("cmd", "/c", "echo", "Hello")
    .WithArgs("/c", "echo", "Hello !!!");
```



### Run a command line



``` CSharp
int? exitCode = GetService<ICommandLine>().Run(new CommandLine("whoami", "/all"));
```



### Run a command line asynchronously



``` CSharp
int? exitCode = await GetService<ICommandLine>().RunAsync(new CommandLine("whoami", "/all"));
```



### Run and process output



``` CSharp
var lines = new System.Collections.Generic.List<string>();
int? exitCode = GetService<ICommandLine>().Run(
    new CommandLine("cmd").AddArgs("/c", "SET").AddVars(("MyEnv", "MyVal")),
    i => lines.Add(i.Line));

lines.ShouldContain("MyEnv=MyVal");
```



### Run asynchronously in parallel



``` CSharp
Task<int?> task = GetService<ICommandLine>().RunAsync(new CommandLine("whoami").AddArgs("/all"));
int? exitCode = GetService<ICommandLine>().Run(new CommandLine("cmd", "/c", "SET"));
task.Wait();
```



### Cancellation of asynchronous run

The cancellation will kill a related process.

``` CSharp
var cancellationTokenSource = new CancellationTokenSource();
Task<int?> task = GetService<ICommandLine>().RunAsync(
    new CommandLine("cmd", "/c", "TIMEOUT", "/T", "120"),
    default,
    cancellationTokenSource.Token);

cancellationTokenSource.CancelAfter(TimeSpan.FromMilliseconds(100));
task.IsCompleted.ShouldBeFalse();
```



### Run timeout

If timeout expired a process will be killed.

``` CSharp
int? exitCode = GetService<ICommandLine>().Run(
    new CommandLine("cmd", "/c", "TIMEOUT", "/T", "120"),
    default,
    TimeSpan.FromMilliseconds(1));

exitCode.HasValue.ShouldBeFalse();
```



### TeamCity integration via service messages

For more details how to use TeamCity service message API please see [this](https://github.com/JetBrains/TeamCity.ServiceMessages) page. Instead of creating a root message writer like in the following example:
``` CSharp
using var writer = new TeamCityServiceMessages().CreateWriter(Console.WriteLine);
```
use this statement:
``` CSharp
using var writer = GetService<ITeamCityWriter>();
```
This sample opens a block _My Tests_ and reports about two tests:

``` CSharp
using var writer = GetService<ITeamCityWriter>();
using (var tests = writer.OpenBlock("My Tests"))
{
    using (var test = tests.OpenTest("Test1"))
    {
        test.WriteStdOutput("Hello");
        test.WriteImage("TestsResults/Test1Screenshot.jpg", "Screenshot");
        test.WriteDuration(TimeSpan.FromMilliseconds(10));
    }
    
    using (var test = tests.OpenTest("Test2"))
    {
        test.WriteFailed("Some error", "Error details");
    }
}
```

For more information on TeamCity Service Messages, see [this](https://www.jetbrains.com/help/teamcity/service-messages.html) page.

