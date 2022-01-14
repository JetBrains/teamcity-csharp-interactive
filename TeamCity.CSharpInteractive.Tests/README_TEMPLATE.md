
## Usage Scenarios

- Global state
  - [Using _Args_](#using-_args_)
  - [Using _Props_ dictionary](#using-_props_-dictionary)
  - [Using the _Host_ property](#using-the-_host_-property)
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
  - [Parallel builds](#parallel-builds)
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
// Adds the namespace "Cmd" to use Command Line API
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
// Adds the namespace "Cmd" to use Command Line API
using Cmd;

int? exitCode = GetService<ICommandLine>().Run(new CommandLine("whoami", "/all"));
```



### Run a command line asynchronously



``` CSharp
// Adds the namespace "Cmd" to use Command Line API
using Cmd;

int? exitCode = await GetService<ICommandLine>().RunAsync(new CommandLine("whoami", "/all"));
```



### Run and process output



``` CSharp
// Adds the namespace "Cmd" to use Command Line API
using Cmd;

var lines = new System.Collections.Generic.List<string>();
int? exitCode = GetService<ICommandLine>().Run(
    new CommandLine("cmd").AddArgs("/c", "SET").AddVars(("MyEnv", "MyVal")),
    i => lines.Add(i.Line));

lines.ShouldContain("MyEnv=MyVal");
```



### Run asynchronously in parallel



``` CSharp
// Adds the namespace "Cmd" to use Command Line API
using Cmd;

Task<int?> task = GetService<ICommandLine>().RunAsync(new CommandLine("whoami").AddArgs("/all"));
int? exitCode = GetService<ICommandLine>().Run(new CommandLine("cmd", "/c", "SET"));
task.Wait();
```



### Cancellation of asynchronous run

The cancellation will kill a related process.

``` CSharp
// Adds the namespace "Cmd" to use Command Line API
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
// Adds the namespace "Cmd" to use Command Line API
using Cmd;

int? exitCode = GetService<ICommandLine>().Run(
    new CommandLine("cmd", "/c", "TIMEOUT", "/T", "120"),
    default,
    TimeSpan.FromMilliseconds(1));

exitCode.HasValue.ShouldBeFalse();
```



### Build a project



``` CSharp
// Adds the namespace "Dotnet" to use .NET build API
using Dotnet;

// Resolves a build service
var build = GetService<IBuild>();

// Creates a new library project, running a command like: "dotnet new classlib -n MyLib --force"
var result = build.Run(new Custom("new", "classlib", "-n", "MyLib", "--force"));
result.Success.ShouldBeTrue();

// Builds the library project, running a command like: "dotnet build" from the directory "MyLib"
result = build.Run(new Build().WithWorkingDirectory("MyLib"));

// The "result" variable provides details about a build
result.Messages.Any(message => message.State == BuildMessageState.Error).ShouldBeFalse();
result.Success.ShouldBeTrue();
```



### Clean a project



``` CSharp
// Adds the namespace "Dotnet" to use .NET build API
using Dotnet;

// Resolves a build service
var build = GetService<IBuild>();

// Creates a new library project, running a command like: "dotnet new classlib -n MyLib --force"
var result = build.Run(new Custom("new", "classlib", "-n", "MyLib", "--force"));
result.Success.ShouldBeTrue();

// Builds the library project, running a command like: "dotnet build" from the directory "MyLib"
result = build.Run(new Build().WithWorkingDirectory("MyLib"));
result.Success.ShouldBeTrue();

// Clean the project, running a command like: "dotnet clean" from the directory "MyLib"
result = build.Run(new Clean().WithWorkingDirectory("MyLib"));

// The "result" variable provides details about a build
result.Success.ShouldBeTrue();
```



### Run a custom .NET command



``` CSharp
// Adds the namespace "Dotnet" to use .NET build API
using Dotnet;

// Resolves a build service
var build = GetService<IBuild>();

// Gets the dotnet version, running a command like: "dotnet --version"
Version? version = default;
var result = build.Run(
    new Custom("--version"),
    output => Version.TryParse(output.Line, out version));

result.Success.ShouldBeTrue();
version.ShouldNotBeNull();
```



### Build a project using MSBuild



``` CSharp
// Adds the namespace "Dotnet" to use .NET build API
using Dotnet;

// Resolves a build service
var build = GetService<IBuild>();

// Creates a new library project, running a command like: "dotnet new classlib -n MyLib --force"
var result = build.Run(new Custom("new", "classlib", "-n", "MyLib", "--force"));
result.Success.ShouldBeTrue();

// Builds the library project, running a command like: "dotnet msbuild /t:Build -restore /p:configuration=Release -verbosity=detailed" from the directory "MyLib"
result = build.Run(
    new MSBuild()
        .WithWorkingDirectory("MyLib")
        .WithTarget("Build")
        .WithRestore(true)
        .AddProps(("configuration", "Release"))
        .WithVerbosity(Verbosity.Detailed));

// The "result" variable provides details about a build
result.Messages.Any(message => message.State == BuildMessageState.Error).ShouldBeFalse();
result.Success.ShouldBeTrue();
```



### Pack a project



``` CSharp
// Adds the namespace "Dotnet" to use .NET build API
using Dotnet;

// Resolves a build service
var build = GetService<IBuild>();

// Creates a new library project, running a command like: "dotnet new classlib -n MyLib --force"
var result = build.Run(new Custom("new", "classlib", "-n", "MyLib", "--force"));
result.Success.ShouldBeTrue();

// Creates a NuGet package of version 1.2.3 for the project, running a command like: "dotnet pack /p:version=1.2.3" from the directory "MyLib"
result = build.Run(
    new Pack()
        .WithWorkingDirectory("MyLib")
        .AddProps(("version", "1.2.3")));

result.Success.ShouldBeTrue();
```



### Publish a project



``` CSharp
// Adds the namespace "Dotnet" to use .NET build API
using Dotnet;

// Resolves a build service
var build = GetService<IBuild>();

// Creates a new library project, running a command like: "dotnet new classlib -n MyLib --force"
var result = build.Run(new Custom("new", "classlib", "-n", "MyLib", "--force"));
result.Success.ShouldBeTrue();

// Publish the project, running a command like: "dotnet publish --framework net6.0" from the directory "MyLib"
result = build.Run(new Publish().WithWorkingDirectory("MyLib").WithFramework("net6.0"));
result.Success.ShouldBeTrue();
```



### Restore a project



``` CSharp
// Adds the namespace "Dotnet" to use .NET build API
using Dotnet;

// Resolves a build service
var build = GetService<IBuild>();

// Creates a new library project, running a command like: "dotnet new classlib -n MyLib --force"
var result = build.Run(new Custom("new", "classlib", "-n", "MyLib", "--force"));
result.Success.ShouldBeTrue();

// Restore the project, running a command like: "dotnet restore" from the directory "MyLib"
result = build.Run(new Restore().WithWorkingDirectory("MyLib"));
result.Success.ShouldBeTrue();
```



### Run a project



``` CSharp
// Adds the namespace "Dotnet" to use .NET build API
using Dotnet;

// Resolves a build service
var build = GetService<IBuild>();

// Creates a new console project, running a command like: "dotnet new console -n MyApp --force"
var result = build.Run(new Custom("new", "console", "-n", "MyApp", "--force"));
result.Success.ShouldBeTrue();

// Runs the console project using a command like: "dotnet run" from the directory "MyApp"
var stdOut = new List<string>(); 
result = build.Run(new Run().WithWorkingDirectory("MyApp"), output => stdOut.Add(output.Line));
result.Success.ShouldBeTrue();
// Checks StdOut
stdOut.ShouldBe(new []{ "Hello, World!" });
```



### Test a project



``` CSharp
// Adds the namespace "Dotnet" to use .NET build API
using Dotnet;

// Resolves a build service
var build = GetService<IBuild>();

// Creates a new test project, running a command like: "dotnet new mstest -n MyTests --force"
var result = build.Run(new Custom("new", "mstest", "-n", "MyTests", "--force"));
result.Success.ShouldBeTrue();

// Runs tests via a command like: "dotnet test" from the directory "MyTests"
result = build.Run(new Test().WithWorkingDirectory("MyTests"));

// The "result" variable provides details about a build
result.Tests.Count(test => test.State == TestState.Passed).ShouldBe(1);
result.Success.ShouldBeTrue();
```



### Test an assembly



``` CSharp
// Adds the namespace "Dotnet" to use .NET build API
using Dotnet;

// Resolves a build service
var build = GetService<IBuild>();

// Creates a new test project, running a command like: "dotnet new mstest -n MyTests --force"
var result = build.Run(new Custom("new", "mstest", "-n", "MyTests", "--force"));
result.Success.ShouldBeTrue();

// Builds the test project, running a command like: "dotnet build -c Release" from the directory "MyTests"
result = build.Run(new Build().WithWorkingDirectory("MyTests").WithConfiguration("Release").WithOutput("MyOutput"));
result.Success.ShouldBeTrue();

// Runs tests via a command like: "dotnet vstest" from the directory "MyTests"
result = build.Run(
    new VSTest()
        .AddTestFileNames(Path.Combine("MyOutput", "MyTests.dll"))
        .WithWorkingDirectory("MyTests"));

// The "result" variable provides details about a build
result.Tests.Count(test => test.State == TestState.Passed).ShouldBe(1);
result.Success.ShouldBeTrue();
```



### Parallel builds



``` CSharp
// Adds the namespace "Dotnet" to use .NET build API
using Dotnet;

// Resolves a build service
var build = GetService<IBuild>();

// Creates a new test project, running a command like: "dotnet new mstest -n MSTestTests --force"
var createMSTestTask = build.RunAsync(new Custom("new", "mstest", "-n", "MSTestTests", "--force"));

// Runs tests via a command like: "dotnet test" from the directory "MSTestTests"
var runMSTestTask = build.RunAsync(new Test().WithWorkingDirectory("MSTestTests"));

// Creates a another test project, running a command like: "dotnet new xunit -n XUnitTests --force"
var createXUnitTask = build.RunAsync(new Custom("new", "xunit", "-n", "XUnitTests", "--force"));

// Runs tests via a command like: "dotnet test" from the directory "XUnitTests"
var runXUnitTask = build.RunAsync(new Test().WithWorkingDirectory("XUnitTests"));

// Creates a new library project, running a command like: "dotnet new classlib -n MyLib --force"
var createLibTask = build.RunAsync(new Custom("new", "classlib", "-n", "MyLib", "--force"));

// Publish the project, running a command like: "dotnet publish --framework net6.0" from the directory "MyLib"
var publishLibTask = build.RunAsync(new Publish().WithWorkingDirectory("MyLib").WithFramework("net6.0"));

// Runs pipelines in parallel
var results = await Task.WhenAll(
    // MSTest tests pipeline
    createMSTestTask.ContinueWith(_ => runMSTestTask.Result),
    // XUnit tests pipeline
    createXUnitTask.ContinueWith(_ => runXUnitTask.Result),
    // Publish pipeline
    createLibTask.ContinueWith(_ => publishLibTask.Result));

// The "results" variable provides details about all builds
results.Length.ShouldBe(3);
results.All(result => result.Success).ShouldBeTrue();
```



### Restore NuGet a package of newest version



``` CSharp
// Adds the namespace "NuGet" to use INuGet
using NuGet;

IEnumerable<NuGetPackage> packages = GetService<INuGet>().Restore("IoC.Container", "*");
```



### Restore a NuGet package by a version range for the specified .NET and path



``` CSharp
// Adds the namespace "NuGet" to use INuGet
using NuGet;

var packagesPath = System.IO.Path.Combine(
    System.IO.Path.GetTempPath(),
    Guid.NewGuid().ToString()[..4]);

IEnumerable<NuGetPackage> packages = GetService<INuGet>().Restore(
    "IoC.Container",
    "[1.3, 1.3.8)",
    "net5.0",
    packagesPath);
```



### Running in docker



``` CSharp
// Adds the namespace "Cmd" to use Command Line API
using Cmd;
// Adds the namespace "Docker" to use Docker API
using Docker;

// Resolves a build service
var commandLine = GetService<ICommandLine>();

// Creates some command line to run in a docker container
var cmd = new CommandLine("whoami");

// Runs the command line in a docker container
var result = commandLine.Run(new Run(cmd, "mcr.microsoft.com/dotnet/sdk").WithAutoRemove(true));
result.ShouldBe(0);
```



### Build a project in a docker container



``` CSharp
// Adds the namespace "Dotnet" to use .NET build API
using Dotnet;
// Adds the namespace "Docker" to use Docker API
using Docker;

// Resolves a build service
var build = GetService<IBuild>();

// Creates a base docker command line
var baseDockerCmd = new Docker.Run()
    .WithImage("mcr.microsoft.com/dotnet/sdk")
    .WithPlatform("linux")
    .WithContainerWorkingDirectory("/MyProjects")
    .AddVolumes((Environment.CurrentDirectory, "/MyProjects"));

// Creates a new library project in a docker container
var customCmd = new Custom("new", "classlib", "-n", "MyLib", "--force").WithExecutablePath("dotnet");
var result = build.Run(baseDockerCmd.WithProcess(customCmd));
result.Success.ShouldBeTrue();

// Builds the library project in a docker container
var buildCmd = new Build().WithProject("MyLib/MyLib.csproj").WithExecutablePath("dotnet");
result = build.Run(baseDockerCmd.WithProcess(buildCmd), output => {});

// The "result" variable provides details about a build
result.Messages.Any(message => message.State == BuildMessageState.Error).ShouldBeFalse();
result.Success.ShouldBeTrue();
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
        test.WriteFailed("Some error", "Error details");
    }
}
```

For more information on TeamCity Service Messages, see [this](https://www.jetbrains.com/help/teamcity/service-messages.html) page.

