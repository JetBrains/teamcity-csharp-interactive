
## Usage Scenarios

- Global state
  - [Using Args](#using-args)
  - [Using Props dictionary](#using-props-dictionary)
  - [Using the Host property](#using-the-host-property)
  - [Get services](#get-services)
  - [Service collection](#service-collection)
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
  - [Build a project in a docker container](#build-a-project-in-a-docker-container)
  - [Running in docker](#running-in-docker)
- .NET build API
  - [.NET Scenarios](#.net-scenarios)
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

### Service collection



``` CSharp
public void Run()
{
    var serviceProvider = 
        GetService<IServiceCollection>()
        .AddTransient<MyTask>()
        .BuildServiceProvider();

    var myTask = serviceProvider.GetRequiredService<MyTask>();
    var exitCode = myTask.Run();
    exitCode.ShouldBe(0);
}

class MyTask
{
    private readonly ICommandLineRunner _runner;

    public MyTask(ICommandLineRunner runner) => 
        _runner = runner;

    public int? Run() => 
        _runner.Run(new CommandLine("whoami"));
}

```



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
using HostApi;

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
// Adds the namespace "HostApi" to use Command Line API
using HostApi;

int? exitCode = GetService<ICommandLineRunner>().Run(new CommandLine("cmd", "/c", "DIR"));

// or the same thing using the extension method
exitCode = new CommandLine("cmd", "/c", "DIR").Run();

```



### Run a command line asynchronously



``` CSharp
// Adds the namespace "HostApi" to use Command Line API
using HostApi;

int? exitCode = await GetService<ICommandLineRunner>().RunAsync(new CommandLine("cmd", "/C", "DIR"));

// or the same thing using the extension method
exitCode = await new CommandLine("cmd", "/c", "DIR").RunAsync();
```



### Run and process output



``` CSharp
// Adds the namespace "HostApi" to use Command Line API
using HostApi;

var lines = new List<string>();
int? exitCode = new CommandLine("cmd")
    .AddArgs("/c", "SET")
    .AddVars(("MyEnv", "MyVal"))
    .Run(output => lines.Add(output.Line));

lines.ShouldContain("MyEnv=MyVal");
```



### Run asynchronously in parallel



``` CSharp
// Adds the namespace "HostApi" to use Command Line API
using HostApi;

Task<int?> task = GetService<ICommandLineRunner>().RunAsync(new CommandLine("cmd", "/c", "DIR"));
int? exitCode = new CommandLine("cmd", "/c", "SET").Run();
task.Wait();
```



### Cancellation of asynchronous run

The cancellation will kill a related process.

``` CSharp
// Adds the namespace "HostApi" to use Command Line API
using HostApi;

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
// Adds the namespace "HostApi" to use Command Line API
using HostApi;

int? exitCode = GetService<ICommandLineRunner>().Run(
    new CommandLine("cmd", "/c", "TIMEOUT", "/T", "120"),
    default,
    TimeSpan.FromMilliseconds(1));

exitCode.HasValue.ShouldBeFalse();
```



### .NET Scenarios



``` CSharp
// Adds the namespace "HostApi" to use .NET build API
using HostApi;

var clean = new DotNetClean();

var restore = new DotNetRestore()
    .WithWorkingDirectory("MyLib");

var build = new DotNetBuild()
    .WithConfiguration("Release");

// Resolves a build service
var buildRunner = GetService<IBuildRunner>();

// runs using a runner
buildRunner.Run(restore);

// or using the extension method
build.Build();
```



### Build a project



``` CSharp
// Adds the namespace "HostApi" to use .NET build API
using HostApi;

// Creates a new library project, running a command like: "dotnet new classlib -n MyLib --force"
var result = new DotNetCustom("new", "classlib", "-n", "MyLib", "--force").Build();
result.ExitCode.ShouldBe(0);

// Builds the library project, running a command like: "dotnet build" from the directory "MyLib"
result = new DotNetBuild().WithWorkingDirectory("MyLib").Build();

// The "result" variable provides details about a build
result.Errors.Any(message => message.State == BuildMessageState.StdError).ShouldBeFalse();
result.ExitCode.ShouldBe(0);
```



### Clean a project



``` CSharp
// Adds the namespace "HostApi" to use .NET build API
using HostApi;

// Creates a new library project, running a command like: "dotnet new classlib -n MyLib --force"
var result = new DotNetCustom("new", "classlib", "-n", "MyLib", "--force").Build();
result.ExitCode.ShouldBe(0);

// Builds the library project, running a command like: "dotnet build" from the directory "MyLib"
result = new DotNetBuild().WithWorkingDirectory("MyLib").Build();
result.ExitCode.ShouldBe(0);

// Clean the project, running a command like: "dotnet clean" from the directory "MyLib"
result = new DotNetClean().WithWorkingDirectory("MyLib").Build();

// The "result" variable provides details about a build
result.ExitCode.ShouldBe(0);
```



### Run a custom .NET command



``` CSharp
// Adds the namespace "HostApi" to use .NET build API
using HostApi;

// Gets the dotnet version, running a command like: "dotnet --version"
Version? version = default;
var result = new DotNetCustom("--version")
    .Build(message => Version.TryParse(message.Text, out version));

result.ExitCode.ShouldBe(0);
version.ShouldNotBeNull();
```



### Pack a project



``` CSharp
// Adds the namespace "HostApi" to use .NET build API
using HostApi;

// Creates a new library project, running a command like: "dotnet new classlib -n MyLib --force"
var result = new DotNetCustom("new", "classlib", "-n", "MyLib", "--force").Build();
result.ExitCode.ShouldBe(0);

// Creates a NuGet package of version 1.2.3 for the project, running a command like: "dotnet pack /p:version=1.2.3" from the directory "MyLib"
result = new DotNetPack()
        .WithWorkingDirectory("MyLib")
        .AddProps(("version", "1.2.3"))
        .Build();

result.ExitCode.ShouldBe(0);
```



### Publish a project



``` CSharp
// Adds the namespace "HostApi" to use .NET build API
using HostApi;

// Creates a new library project, running a command like: "dotnet new classlib -n MyLib --force"
var result = new DotNetCustom("new", "classlib", "-n", "MyLib", "--force").Build();
result.ExitCode.ShouldBe(0);

// Publish the project, running a command like: "dotnet publish --framework net6.0" from the directory "MyLib"
result = new DotNetPublish().WithWorkingDirectory("MyLib").WithFramework("net6.0").Build();
result.ExitCode.ShouldBe(0);
```



### Restore a project



``` CSharp
// Adds the namespace "HostApi" to use .NET build API
using HostApi;

// Creates a new library project, running a command like: "dotnet new classlib -n MyLib --force"
var result = new DotNetCustom("new", "classlib", "-n", "MyLib", "--force").Build();
result.ExitCode.ShouldBe(0);

// Restore the project, running a command like: "dotnet restore" from the directory "MyLib"
result = new DotNetRestore().WithWorkingDirectory("MyLib").Build();
result.ExitCode.ShouldBe(0);
```



### Run a project



``` CSharp
// Adds the namespace "HostApi" to use .NET build API
using HostApi;

// Creates a new console project, running a command like: "dotnet new console -n MyApp --force"
var result = new DotNetCustom("new", "console", "-n", "MyApp", "--force").Build();
result.ExitCode.ShouldBe(0);

// Runs the console project using a command like: "dotnet run" from the directory "MyApp"
var stdOut = new List<string>();
result = new DotNetRun().WithWorkingDirectory("MyApp").Build(message => stdOut.Add(message.Text));
result.ExitCode.ShouldBe(0);

// Checks StdOut
stdOut.ShouldBe(new[] {"Hello, World!"});
```



### Test a project



``` CSharp
// Adds the namespace "HostApi" to use .NET build API
using HostApi;

// Creates a new test project, running a command like: "dotnet new mstest -n MyTests --force"
var result = new DotNetCustom("new", "mstest", "-n", "MyTests", "--force").Build();
result.ExitCode.ShouldBe(0);

// Runs tests via a command like: "dotnet test" from the directory "MyTests"
result = new DotNetTest().WithWorkingDirectory("MyTests").Build();

// The "result" variable provides details about a build
result.ExitCode.ShouldBe(0);
result.Tests.Count(test => test.State == TestState.Passed).ShouldBe(1);
```



### Test an assembly



``` CSharp
// Adds the namespace "HostApi" to use .NET build API
using HostApi;

// Creates a new test project, running a command like: "dotnet new mstest -n MyTests --force"
var result = new DotNetCustom("new", "mstest", "-n", "MyTests", "--force").Build();
result.ExitCode.ShouldBe(0);

// Builds the test project, running a command like: "dotnet build -c Release" from the directory "MyTests"
result = new DotNetBuild().WithWorkingDirectory("MyTests").WithConfiguration("Release").WithOutput("MyOutput").Build();
result.ExitCode.ShouldBe(0);

// Runs tests via a command like: "dotnet vstest" from the directory "MyTests"
result = new VSTest()
    .AddTestFileNames(Path.Combine("MyOutput", "MyTests.dll"))
    .WithWorkingDirectory("MyTests")
    .Build();

// The "result" variable provides details about a build
result.Tests.Count(test => test.State == TestState.Passed).ShouldBe(1);
result.ExitCode.ShouldBe(0);
```



### Build a project using MSBuild



``` CSharp
// Adds the namespace "HostApi" to use .NET build API
using HostApi;

// Creates a new library project, running a command like: "dotnet new classlib -n MyLib --force"
var result = new DotNetCustom("new", "classlib", "-n", "MyLib", "--force").Build();
result.ExitCode.ShouldBe(0);

// Builds the library project, running a command like: "dotnet msbuild /t:Build -restore /p:configuration=Release -verbosity=detailed" from the directory "MyLib"
result = new MSBuild()
    .WithWorkingDirectory("MyLib")
    .WithTarget("Build")
    .WithRestore(true)
    .AddProps(("configuration", "Release"))
    .WithVerbosity(DotNetVerbosity.Detailed)
    .Build();

// The "result" variable provides details about a build
result.Errors.Any(message => message.State == BuildMessageState.StdError).ShouldBeFalse();
result.ExitCode.ShouldBe(0);
```



### Restore NuGet a package of newest version



``` CSharp
// Adds the namespace "HostApi" to use INuGet
using HostApi;

IEnumerable<NuGetPackage> packages = GetService<INuGet>().Restore(new NuGetRestoreSettings("IoC.Container").WithVersionRange(VersionRange.All));
```



### Restore a NuGet package by a version range for the specified .NET and path



``` CSharp
// Adds the namespace "HostApi" to use INuGet
using HostApi;

var packagesPath = Path.Combine(
    Path.GetTempPath(),
    Guid.NewGuid().ToString()[..4]);

var settings = new NuGetRestoreSettings("IoC.Container")
    .WithVersionRange(VersionRange.Parse("[1.3, 1.3.8)"))
    .WithTargetFrameworkMoniker("net5.0")
    .WithPackagesPath(packagesPath);

IEnumerable<NuGetPackage> packages = GetService<INuGet>().Restore(settings);
```



### Build a project in a docker container



``` CSharp
// Adds the namespace "HostApi" to use .NET build API and Docker API
using HostApi;

// Resolves a build service
var buildRunner = GetService<IBuildRunner>();

// Creates a base docker command line
var baseDockerCmd = new DockerRun()
    .WithAutoRemove(true)
    .WithImage("mcr.microsoft.com/dotnet/sdk")
    .WithPlatform("linux")
    .WithContainerWorkingDirectory("/MyProjects")
    .AddVolumes((Environment.CurrentDirectory, "/MyProjects"));

// Creates a new library project in a docker container
var result = baseDockerCmd.WithCommandLine(
        new DotNetCustom("new", "classlib", "-n", "MyLib", "--force")
        .WithExecutablePath("dotnet"))
    .Build();

result.ExitCode.ShouldBe(0);

// Builds the library project in a docker container
result = baseDockerCmd.WithCommandLine(
        new DotNetBuild()
        .WithProject("MyLib/MyLib.csproj")
        .WithExecutablePath("dotnet"))
    .Build();

// The "result" variable provides details about a build
result.Errors.Any(message => message.State == BuildMessageState.StdError).ShouldBeFalse();
result.ExitCode.ShouldBe(0);
```



### Running in docker



``` CSharp
// Adds the namespace "HostApi" to use Command Line API and Docker API
using HostApi;

// Creates some command line to run in a docker container
var cmd = new CommandLine("whoami");

// Runs the command line in a docker container
var result = new DockerRun(cmd, "mcr.microsoft.com/dotnet/sdk")
    .WithAutoRemove(true)
    .Run();

result.ShouldBe(0);
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

