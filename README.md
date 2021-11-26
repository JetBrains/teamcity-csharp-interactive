## C# script tool for [<img src="https://cdn.worldvectorlogo.com/logos/teamcity.svg" height="20" align="center"/>](https://www.jetbrains.com/teamcity/)

[<img src="http://jb.gg/badges/official.svg"/>](https://confluence.jetbrains.com/display/ALL/JetBrains+on+GitHub) [![NuGet TeamCity.csi](https://buildstats.info/nuget/TeamCity.csi?includePreReleases=true)](https://www.nuget.org/packages/TeamCity.csi) ![GitHub](https://img.shields.io/github/license/jetbrains/teamcity-csharp-interactive) [<img src="http://teamcity.jetbrains.com/app/rest/builds/buildType:(id:TeamCityPluginsByJetBrains_TeamCityCScript_BuildAndTestBuildType)/statusIcon.svg"/>](http://teamcity.jetbrains.com/viewType.html?buildTypeId=TeamCityPluginsByJetBrains_TeamCityCScript_BuildAndTestBuildType&guest=1)

This is a repository of TeamCity.csi which is an interactive tool for running C# scripts. It can be used as a [TeamCity build runner](https://github.com/JetBrains/teamcity-dotnet-plugin#c-script-runner) or installed as a command-line tool on Windows, Linux, or macOS.

## Prerequisites

The tool requires [.NET 6 runtime](https://dotnet.microsoft.com/download/dotnet/6.0).

## Download and Install TeamCity.csi

TeamCity.csi is available as a [NuGet package](https://www.nuget.org/packages/TeamCity.csi/).

Install the tool on your OS:

```Shell
dotnet tool install dotnet-csi -g --version <version>
```

Uninstall the tool:
```Shell
dotnet tool uninstall dotnet-csi -g
```

## Use Inside TeamCity

Currently, the tool can be used as a TeamCity build runner provided in terms of TeamCity 2021.2 [Early Access Program](https://www.jetbrains.com/teamcity/nextversion/). Read the runner's [documentation]() for more details.

## Use Outside TeamCity

You can use this tool independently of TeamCity, to run tasks in C# from the command line on any supported OS.

Launch the tool in the interactive mode:
```Shell
dotnet csi
```

Run a specified script with a given argument:
```Shell
dotnet csi script-file.csx
```

Usage:

```Shell
dotnet csi [options] [script-file.csx] [script-arguments]
```

Script arguments are accessible in scripts via a global list called _Args_.

Supported arguments:

| Option | Description | Alternative form |
| -------- | ------------| ---------------- |
| `--help` | Show how to use the command. | `/?`, `-h`, `/h`, `/help` |
| `--version` | Display the tool version. | `/version` |
| `--source` | Specify the NuGet package source to use. Supported formats: URL, or a UNC directory path. | `-s`, `/s`, `/source` |
| `--property <key=value>` | Define a _key=value_ pair for the global dictionary called _Props_, which is accessible in scripts. | `-p`, `/property`, `/p` |
| `@file` | Read the response file for more options. | |
| `--` | Indicates that the remaining arguments should not be treated as options. | |

## Report and Track Issues

Please use our YouTrack to [report](https://youtrack.jetbrains.com/newIssue?project=TW&description=Expected%20behavior%20and%20actual%20behavior%3A%0A%0ASteps%20to%20reproduce%20the%20problem%3A%0A%0ASpecifications%20like%20the%20tool%20version%2C%20operating%20system%3A%0A%0AResult%20of%20'dotnet%20--info'%3A&c=Subsystem%20Agent%20-%20.NET&c=Assignee%20Nikolay.Pianikov&c=tag%20.NET%20Core&c=tag%20cs%20script%20step) related issues.

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
var packages = Host.GetService<INuGet>();
Host.WriteLine("Hello");
```



### Get services

This method might be used to get access to different APIs like [INuGet](TeamCity.CSharpInteractive.Contracts/INuGet.cs) or [ICommandLine](TeamCity.CSharpInteractive.Contracts/ICommandLine.cs).

``` CSharp
GetService<INuGet>();

var serviceProvider = GetService<IServiceProvider>();
serviceProvider.GetService(typeof(INuGet));
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
// Adds the namespace "Cmd" to use ICommandLine
using Cmd;

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
// Adds the namespace "Cmd" to use ICommandLine
using Cmd;

int? exitCode = GetService<ICommandLine>().Run(new CommandLine("whoami", "/all"));
```



### Run a command line asynchronously



``` CSharp
// Adds the namespace "Cmd" to use ICommandLine
using Cmd;
int? exitCode = await GetService<ICommandLine>().RunAsync(new CommandLine("whoami", "/all"));
```



### Run and process output



``` CSharp
// Adds the namespace "Cmd" to use ICommandLine
using Cmd;

var lines = new System.Collections.Generic.List<string>();
int? exitCode = GetService<ICommandLine>().Run(
    new CommandLine("cmd").AddArgs("/c", "SET").AddVars(("MyEnv", "MyVal")),
    i => lines.Add(i.Line));

lines.ShouldContain("MyEnv=MyVal");
```



### Run asynchronously in parallel



``` CSharp
// Adds the namespace "Cmd" to use ICommandLine
using Cmd;

Task<int?> task = GetService<ICommandLine>().RunAsync(new CommandLine("whoami").AddArgs("/all"));
int? exitCode = GetService<ICommandLine>().Run(new CommandLine("cmd", "/c", "SET"));
task.Wait();
```



### Cancellation of asynchronous run

The cancellation will kill a related process.

``` CSharp
// Adds the namespace "Cmd" to use ICommandLine
using Cmd;
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
// Adds the namespace "Cmd" to use ICommandLine
using Cmd;

int? exitCode = GetService<ICommandLine>().Run(
    new CommandLine("cmd", "/c", "TIMEOUT", "/T", "120"),
    default,
    TimeSpan.FromMilliseconds(1));

exitCode.HasValue.ShouldBeFalse();
```



### Restore NuGet a package of newest version



``` CSharp
// Adds the namespace "NuGet" to use INuGet
;

IEnumerable<NuGetPackage> packages = GetService<INuGet>().Restore("IoC.Container", "*");
```



### Restore a NuGet package by a version range for the specified .NET and path



``` CSharp
// Adds the namespace "NuGet" to use INuGet
;

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

