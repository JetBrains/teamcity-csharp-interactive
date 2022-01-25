## C# script tool for [<img src="https://cdn.worldvectorlogo.com/logos/teamcity.svg" height="20" align="center"/>](https://www.jetbrains.com/teamcity/)

[<img src="http://jb.gg/badges/official.svg"/>](https://confluence.jetbrains.com/display/ALL/JetBrains+on+GitHub) [![NuGet TeamCity.csi](https://buildstats.info/nuget/TeamCity.csi?includePreReleases=true)](https://www.nuget.org/packages/TeamCity.csi) ![GitHub](https://img.shields.io/github/license/jetbrains/teamcity-csharp-interactive) [<img src="http://teamcity.jetbrains.com/app/rest/builds/buildType:(id:TeamCityPluginsByJetBrains_TeamCityCScript_BuildAndTestBuildType)/statusIcon.svg"/>](http://teamcity.jetbrains.com/viewType.html?buildTypeId=TeamCityPluginsByJetBrains_TeamCityCScript_BuildAndTestBuildType&guest=1)

This is a repository of TeamCity.csi which is an interactive tool for running C# scripts. It can be used as a [TeamCity build runner](https://github.com/JetBrains/teamcity-dotnet-plugin#c-script-runner) or installed as a command-line tool on Windows, Linux, or macOS.

## Prerequisites

The tool requires [.NET 6 runtime](https://dotnet.microsoft.com/download/dotnet/6.0).

## Download and Install TeamCity.csi

TeamCity.csi is available as a [NuGet package](https://www.nuget.org/packages/TeamCity.csi/).

Install the tool on your OS:

```Shell
dotnet tool install TeamCity.csi -g --version <version>
```

Uninstall the tool:
```Shell
dotnet tool uninstall TeamCity.csi -g
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
  - [Using Args](#using-args)
  - [Using Props dictionary](#using-props-dictionary)
  - [Using the Host property](#using-the-host-property)
  - [Get services](#get-services)
- Logging
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
- Docker API
  - [Running in docker](#running-in-docker)
  - [Build a project in a docker container](#build-a-project-in-a-docker-container)
- .NET build API
  - [Build a project](#build-a-project)
  - [Build a project using MSBuild](#build-a-project-using-msbuild)
  - [Clean a project](#clean-a-project)
  - [Pack a project](#pack-a-project)
  - [Publish a project](#publish-a-project)
  - [Restore a project](#restore-a-project)
  - [Run a custom .NET command](#run-a-custom-.net-command)
  - [Run a project](#run-a-project)
  - [Test a project](#test-a-project)
  - [Test an assembly](#test-an-assembly)
- NuGet API
  - [Restore NuGet a package of newest version](#restore-nuget-a-package-of-newest-version)
  - [Restore a NuGet package by a version range for the specified .NET and path](#restore-a-nuget-package-by-a-version-range-for-the-specified-.net-and-path)
- TeamCity Service Messages API
  - [TeamCity integration via service messages](#teamcity-integration-via-service-messages)

### Using Args

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



### Using Props dictionary

Properties _Props_ have got from TeamCity system properties automatically.

``` CSharp
WriteLine(Props["TEAMCITY_VERSION"]);
WriteLine(Props["TEAMCITY_PROJECT_NAME"]);

// This property will be available at the next TeamCity steps as system parameter _system.Version_
// and some runners, for instance, the .NET runner, pass it as a build property.
Props["Version"] = "1.1.6";
```



### Using the Host property

[_Host_](TeamCity.CSharpInteractive.HostApi/IHost.cs) is actually the provider of all global properties and methods.

``` CSharp
var packages = Host.GetService<INuGet>();
Host.WriteLine("Hello");
```



### Get services

This method might be used to get access to different APIs like [INuGet](TeamCity.CSharpInteractive.HostApi/INuGet.cs) or [ICommandLine](TeamCity.CSharpInteractive.HostApi/ICommandLine.cs).

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
// Adds the namespace "Script.Cmd" to use Command Line API
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
// Adds the namespace "Script.Cmd" to use Command Line API
using Cmd;

int? exitCode = GetService<ICommandLineRunner>().Run(new CommandLine("cmd", "/c", "DIR"));
```



### Run a command line asynchronously



``` CSharp
// Adds the namespace "Script.Cmd" to use Command Line API
using Cmd;

int? exitCode = await GetService<ICommandLineRunner>().RunAsync(new CommandLine("cmd", "/C", "DIR"));
```



### Run and process output



``` CSharp
// Adds the namespace "Script.Cmd" to use Command Line API
using Cmd;

var lines = new System.Collections.Generic.List<string>();
int? exitCode = GetService<ICommandLineRunner>().Run(
    new CommandLine("cmd").AddArgs("/c", "SET").AddVars(("MyEnv", "MyVal")),
    i => lines.Add(i.Line));
    
lines.ShouldContain("MyEnv=MyVal");
```



### Run asynchronously in parallel



``` CSharp
// Adds the namespace "Script.Cmd" to use Command Line API
using Cmd;

Task<int?> task = GetService<ICommandLineRunner>().RunAsync(new CommandLine("cmd", "/c", "DIR"));
int? exitCode = GetService<ICommandLineRunner>().Run(new CommandLine("cmd", "/c", "SET"));
task.Wait();
```



### Cancellation of asynchronous run

The cancellation will kill a related process.

``` CSharp
// Adds the namespace "Script.Cmd" to use Command Line API
using Cmd;

var cancellationTokenSource = new CancellationTokenSource();
Task<int?> task = GetService<ICommandLineRunner>().RunAsync(
    new CommandLine("cmd", "/c", "TIMEOUT", "/T", "120"),
    default,
    cancellationTokenSource.Token);
    
cancellationTokenSource.CancelAfter(TimeSpan.FromMilliseconds(100));
task.IsCompleted.ShouldBeFalse();
```



### Run timeout

If timeout expired a process will be killed.

``` CSharp
// Adds the namespace "Script.Cmd" to use Command Line API
using Cmd;

int? exitCode = GetService<ICommandLineRunner>().Run(
    new CommandLine("cmd", "/c", "TIMEOUT", "/T", "120"),
    default,
    TimeSpan.FromMilliseconds(1));
    
exitCode.HasValue.ShouldBeFalse();
```



### Build a project



``` CSharp
// Adds the namespace "Script.DotNet" to use .NET build API
using DotNet;

// Resolves a build service
var buildRunner = GetService<IBuildRunner>();
    
// Creates a new library project, running a command like: "dotnet new classlib -n MyLib --force"
var result = buildRunner.Run(new Custom("new", "classlib", "-n", "MyLib", "--force"));
result.ExitCode.ShouldBe(0);

// Builds the library project, running a command like: "dotnet build" from the directory "MyLib"
result = buildRunner.Run(new Build().WithWorkingDirectory("MyLib"));
    
// The "result" variable provides details about a build
result.Errors.Any(message => message.State == BuildMessageState.StdError).ShouldBeFalse();
result.ExitCode.ShouldBe(0);
```



### Clean a project



``` CSharp
// Adds the namespace "Script.DotNet" to use .NET build API
using DotNet;

// Resolves a build service
var buildRunner = GetService<IBuildRunner>();
    
// Creates a new library project, running a command like: "dotnet new classlib -n MyLib --force"
var result = buildRunner.Run(new Custom("new", "classlib", "-n", "MyLib", "--force"));
result.ExitCode.ShouldBe(0);

// Builds the library project, running a command like: "dotnet build" from the directory "MyLib"
result = buildRunner.Run(new Build().WithWorkingDirectory("MyLib"));
result.ExitCode.ShouldBe(0);
    
// Clean the project, running a command like: "dotnet clean" from the directory "MyLib"
result = buildRunner.Run(new Clean().WithWorkingDirectory("MyLib"));
    
// The "result" variable provides details about a build
result.ExitCode.ShouldBe(0);
```



### Run a custom .NET command



``` CSharp
// Adds the namespace "Script.DotNet" to use .NET build API
using DotNet;

// Resolves a build service
var buildRunner = GetService<IBuildRunner>();
    
// Gets the dotnet version, running a command like: "dotnet --version"
Version? version = default;
var result = buildRunner.Run(
    new Custom("--version"),
    message => Version.TryParse(message.Text, out version));

result.ExitCode.ShouldBe(0);
version.ShouldNotBeNull();
```



### Build a project using MSBuild



``` CSharp
// Adds the namespace "Script.DotNet" to use .NET build API
using DotNet;

// Resolves a build service
var buildRunner = GetService<IBuildRunner>();
    
// Creates a new library project, running a command like: "dotnet new classlib -n MyLib --force"
var result = buildRunner.Run(new Custom("new", "classlib", "-n", "MyLib", "--force"));
result.ExitCode.ShouldBe(0);

// Builds the library project, running a command like: "dotnet msbuild /t:Build -restore /p:configuration=Release -verbosity=detailed" from the directory "MyLib"
result = buildRunner.Run(
    new MSBuild()
        .WithWorkingDirectory("MyLib")
        .WithTarget("Build")
        .WithRestore(true)
        .AddProps(("configuration", "Release"))
        .WithVerbosity(Verbosity.Detailed));
    
// The "result" variable provides details about a build
result.Errors.Any(message => message.State == BuildMessageState.StdError).ShouldBeFalse();
result.ExitCode.ShouldBe(0);
```



### Pack a project



``` CSharp
// Adds the namespace "Script.DotNet" to use .NET build API
using DotNet;

// Resolves a build service
var buildRunner = GetService<IBuildRunner>();
    
// Creates a new library project, running a command like: "dotnet new classlib -n MyLib --force"
var result = buildRunner.Run(new Custom("new", "classlib", "-n", "MyLib", "--force"));
result.ExitCode.ShouldBe(0);

// Creates a NuGet package of version 1.2.3 for the project, running a command like: "dotnet pack /p:version=1.2.3" from the directory "MyLib"
result = buildRunner.Run(
    new Pack()
        .WithWorkingDirectory("MyLib")
        .AddProps(("version", "1.2.3")));

result.ExitCode.ShouldBe(0);
```



### Publish a project



``` CSharp
// Adds the namespace "Script.DotNet" to use .NET build API
using DotNet;

// Resolves a build service
var buildRunner = GetService<IBuildRunner>();
    
// Creates a new library project, running a command like: "dotnet new classlib -n MyLib --force"
var result = buildRunner.Run(new Custom("new", "classlib", "-n", "MyLib", "--force"));
result.ExitCode.ShouldBe(0);

// Publish the project, running a command like: "dotnet publish --framework net6.0" from the directory "MyLib"
result = buildRunner.Run(new Publish().WithWorkingDirectory("MyLib").WithFramework("net6.0"));
result.ExitCode.ShouldBe(0);
```



### Restore a project



``` CSharp
// Adds the namespace "Script.DotNet" to use .NET build API
using DotNet;

// Resolves a build service
var buildRunner = GetService<IBuildRunner>();
    
// Creates a new library project, running a command like: "dotnet new classlib -n MyLib --force"
var result = buildRunner.Run(new Custom("new", "classlib", "-n", "MyLib", "--force"));
result.ExitCode.ShouldBe(0);

// Restore the project, running a command like: "dotnet restore" from the directory "MyLib"
result = buildRunner.Run(new Restore().WithWorkingDirectory("MyLib"));
result.ExitCode.ShouldBe(0);
```



### Run a project



``` CSharp
// Adds the namespace "Script.DotNet" to use .NET build API
using DotNet;

// Resolves a build service
var buildRunner = GetService<IBuildRunner>();
    
// Creates a new console project, running a command like: "dotnet new console -n MyApp --force"
var result = buildRunner.Run(new Custom("new", "console", "-n", "MyApp", "--force"));
result.ExitCode.ShouldBe(0);

// Runs the console project using a command like: "dotnet run" from the directory "MyApp"
var stdOut = new List<string>(); 
result = buildRunner.Run(new Run().WithWorkingDirectory("MyApp"), message => stdOut.Add(message.Text));
result.ExitCode.ShouldBe(0);
    
// Checks StdOut
stdOut.ShouldBe(new []{ "Hello, World!" });
```



### Test a project



``` CSharp
// Adds the namespace "Script.DotNet" to use .NET build API
using DotNet;

// Resolves a build service
var build = GetService<IBuildRunner>();
    
// Creates a new test project, running a command like: "dotnet new mstest -n MyTests --force"
var result = build.Run(new Custom("new", "mstest", "-n", "MyTests", "--force"));
result.ExitCode.ShouldBe(0);

// Runs tests via a command like: "dotnet test" from the directory "MyTests"
result = build.Run(new Test().WithWorkingDirectory("MyTests"));
    
// The "result" variable provides details about a build
result.ExitCode.ShouldBe(0);
result.Tests.Count(test => test.State == TestState.Passed).ShouldBe(1);
```



### Test an assembly



``` CSharp
// Adds the namespace "Script.DotNet" to use .NET build API
using DotNet;

// Resolves a build service
var buildRunner = GetService<IBuildRunner>();
    
// Creates a new test project, running a command like: "dotnet new mstest -n MyTests --force"
var result = buildRunner.Run(new Custom("new", "mstest", "-n", "MyTests", "--force"));
result.ExitCode.ShouldBe(0);

// Builds the test project, running a command like: "dotnet build -c Release" from the directory "MyTests"
result = buildRunner.Run(new Build().WithWorkingDirectory("MyTests").WithConfiguration("Release").WithOutput("MyOutput"));
result.ExitCode.ShouldBe(0);
    
// Runs tests via a command like: "dotnet vstest" from the directory "MyTests"
result = buildRunner.Run(
    new VSTest()
        .AddTestFileNames(Path.Combine("MyOutput", "MyTests.dll"))
        .WithWorkingDirectory("MyTests"));
    
// The "result" variable provides details about a build
result.Tests.Count(test => test.State == TestState.Passed).ShouldBe(1);
result.ExitCode.ShouldBe(0);
```



### Restore NuGet a package of newest version



``` CSharp
// Adds the namespace "Script.NuGet" to use INuGet
using NuGet;

IEnumerable<NuGetPackage> packages = GetService<INuGet>().Restore(new RestoreSettings("IoC.Container").WithVersionRange(VersionRange.All));
```



### Restore a NuGet package by a version range for the specified .NET and path



``` CSharp
// Adds the namespace "Script.NuGet" to use INuGet
using NuGet;

var packagesPath = System.IO.Path.Combine(
    System.IO.Path.GetTempPath(),
    Guid.NewGuid().ToString()[..4]);

var settings = new RestoreSettings("IoC.Container")
    .WithVersionRange(VersionRange.Parse("[1.3, 1.3.8)"))
    .WithTargetFrameworkMoniker("net5.0")
    .WithPackagesPath(packagesPath);

IEnumerable<NuGetPackage> packages = GetService<INuGet>().Restore(settings);
```



### Running in docker



``` CSharp
// Adds the namespace "Script.Cmd" to use Command Line API
using Cmd;
// Adds the namespace "Script.Docker" to use Docker API
using Docker;

// Resolves a build service
var commandLineRunner = GetService<ICommandLineRunner>();

// Creates some command line to run in a docker container
var cmd = new CommandLine("whoami");

// Runs the command line in a docker container
var result = commandLineRunner.Run(new Run(cmd, "mcr.microsoft.com/dotnet/sdk").WithAutoRemove(true));
result.ShouldBe(0);
```



### Build a project in a docker container



``` CSharp
// Adds the namespace "Script.DotNet" to use .NET build API
using DotNet;
// Adds the namespace "Script.Docker" to use Docker API
using Docker;

// Resolves a build service
var buildRunner = GetService<IBuildRunner>();

// Creates a base docker command line
var baseDockerCmd = new Run()
    .WithImage("mcr.microsoft.com/dotnet/sdk")
    .WithPlatform("linux")
    .WithContainerWorkingDirectory("/MyProjects")
    .AddVolumes((System.Environment.CurrentDirectory, "/MyProjects"));
    
// Creates a new library project in a docker container
var customCmd = new Custom("new", "classlib", "-n", "MyLib", "--force").WithExecutablePath("dotnet");
var result = buildRunner.Run(baseDockerCmd.WithCommandLine(customCmd));
result.ExitCode.ShouldBe(0);

// Builds the library project in a docker container
var buildCmd = new Build().WithProject("MyLib/MyLib.csproj").WithExecutablePath("dotnet");
result = buildRunner.Run(baseDockerCmd.WithCommandLine(buildCmd), _ => {});
    
// The "result" variable provides details about a build
result.Errors.Any(message => message.State == BuildMessageState.StdError).ShouldBeFalse();
result.ExitCode.ShouldBe(0);
```



### TeamCity integration via service messages

For more details how to use TeamCity service message API please see [this](https://github.com/JetBrains/TeamCity.ServiceMessages) page. Instead of creating a root message writer like in the following example:
``` CSharp
using JetBrains.TeamCity.ServiceMessages.Write.Special;
using var writer = new TeamCityServiceMessages().CreateWriter(Console.WriteLine);
```
use this statement:
``` CSharp
using JetBrains.TeamCity.ServiceMessages.Write.Special;
using var writer = GetService<ITeamCityWriter>();
```
This sample opens a block _My Tests_ and reports about two tests:

``` CSharp
// Adds a namespace to use ITeamCityWriter
using JetBrains.TeamCity.ServiceMessages.Write.Special;

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
        test.WriteIgnored("Some reason");
    }
}
```

For more information on TeamCity Service Messages, see [this](https://www.jetbrains.com/help/teamcity/service-messages.html) page.

