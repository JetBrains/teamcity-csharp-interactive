
## Usage Scenarios

- Global state
  - [Using _Args_](#using-_args_)
  - [Using _Props_ dictionary](#using-_props_-dictionary)
  - [Using the _Host_ property](#using-the-_host_-property)
  - [Get services](#get-services)
- Build log API
  - [Write a line to a build log](#write-a-line-to-a-build-log)
  - [Write a line highlighted with "Header" color to a build log](#write-a-line-highlighted-with-"header"-color-to-a-build-log)
  - [Write an empty line to a build log](#write-an-empty-line-to-a-build-log)
  - [Log an error to a build log](#log-an-error-to-a-build-log)
  - [Log a warning to a build log](#log-a-warning-to-a-build-log)
  - [Log information to a build log](#log-information-to-a-build-log)
  - [Log trace information to a build log](#log-trace-information-to-a-build-log)
- Command Line API
  - [Build command lines](#build-command-lines)
  - [Run a command line](#run-a-command-line)
  - [Run a command line asynchronously](#run-a-command-line-asynchronously)
  - [Run and process output](#run-and-process-output)
  - [Run asynchronously in parallel](#run-asynchronously-in-parallel)
  - [Cancellation of asynchronous run](#cancellation-of-asynchronous-run)
  - [Run timeout](#run-timeout)
- NuGet API
  - [Restore NuGet a package of newest version](#restore-nuget-a-package-of-newest-version)
  - [Restore a NuGet package by a version range for the specified .NET and path](#restore-a-nuget-package-by-a-version-range-for-the-specified-.net-and-path)
- TeamCity Service Messages API
  - [TeamCity integration via service messages](#teamcity-integration-via-service-messages)

### Using _Args_

_Args_ have got from the script arguments.

``` CSharp
if (Args.Count > 0)
{
    WriteLine(Args[0]);
}

if (Args.Count > 1)
{
    WriteLine(Args[1]);
}
```



### Using _Props_ dictionary

Properties _Props_ have got from TeamCity system properties automatically.

``` CSharp
WriteLine(Props["TEAMCITY_VERSION"]);
WriteLine(Props["TEAMCITY_PROJECT_NAME"]);

// This property will be available at the next TeamCity steps as system parameter _system.Version_
// and some runners, for instance, the .NET runner, pass it as a build property.
Props["Version"] = "1.1.6";
```



### Using the _Host_ property

[_Host_](TeamCity.CSharpInteractive.Contracts/IHost.cs) is actually the provider of all global properties and methods.

``` CSharp
var nuGet = Host.GetService<INuGet>();
Host.WriteLine("Hello");
```



### Get services

This method might be used to get access to different APIs like [INuGet](TeamCity.CSharpInteractive.Contracts/INuGet.cs) or [ICommandLine](TeamCity.CSharpInteractive.Contracts/ICommandLine.cs).

``` CSharp
GetService<INuGet>();

GetService<IServiceProvider>();
```

Besides that, it is possible to get an instance of [System.IServiceProvider](https://docs.microsoft.com/en-US/dotnet/api/system.iserviceprovider) to access APIs.

### Write a line to a build log



``` CSharp
WriteLine("Hello");
```



### Write an empty line to a build log



``` CSharp
WriteLine();
```



### Write a line highlighted with "Header" color to a build log



``` CSharp
WriteLine("Hello", Header);
```



### Log an error to a build log



``` CSharp
Error("Error info", "Error identifier");
```



### Log a warning to a build log



``` CSharp
Warning("Warning info");
```



### Log information to a build log



``` CSharp
Info("Some info");
```



### Log trace information to a build log



``` CSharp
Trace("Some trace info");
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



### Restore NuGet a package of newest version



``` CSharp
IEnumerable<NuGetPackage> packages = GetService<INuGet>().Restore("IoC.Container", "*");
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

