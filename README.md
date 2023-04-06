## C# script tool for [<img src="https://cdn.worldvectorlogo.com/logos/teamcity.svg" height="20" align="center"/>](https://www.jetbrains.com/teamcity/)

[<img src="http://jb.gg/badges/official.svg"/>](https://confluence.jetbrains.com/display/ALL/JetBrains+on+GitHub) [![NuGet TeamCity.csi](https://buildstats.info/nuget/TeamCity.csi?includePreReleases=true)](https://www.nuget.org/packages/TeamCity.csi) ![GitHub](https://img.shields.io/github/license/jetbrains/teamcity-csharp-interactive) [<img src="http://teamcity.jetbrains.com/app/rest/builds/buildType:(id:TeamCityPluginsByJetBrains_TeamCityCScript_BuildAndTestBuildType)/statusIcon.svg"/>](http://teamcity.jetbrains.com/viewType.html?buildTypeId=TeamCityPluginsByJetBrains_TeamCityCScript_BuildAndTestBuildType&guest=1)

This is a repository of TeamCity.csi which is an interactive tool for running C# scripts. It can be used as
a [TeamCity build runner](https://github.com/JetBrains/teamcity-dotnet-plugin#c-script-runner) or installed as a
command-line tool on Windows, Linux, or macOS.

## Prerequisites

The tool requires [.NET 6+ runtime](https://dotnet.microsoft.com/en-us/download).

## Use Inside TeamCity

Currently, the tool can be used as a TeamCity build runner provided in terms of TeamCity
2021.2 [Early Access Program](https://www.jetbrains.com/teamcity/nextversion/). Read the runner's [documentation]() for
more details.

## Use Outside TeamCity

After installing tool you can use this tool independently of TeamCity, to run C# scripts from the command line. TeamCity.csi is available as a [NuGet package](https://www.nuget.org/packages/TeamCity.csi/).

Before installing TeamCity.csi as a local tool dot not forget to create .NET local tool manifest file if it is not exist:

```Shell
dotnet new tool-manifest
```

Install the tool and add to the local tool manifest:

```Shell
dotnet tool install TeamCity.csi
```

Or install the tool for the current user:

```Shell
dotnet tool install TeamCity.csi -g
```

Launch the tool in the interactive mode:

```Shell
dotnet csi
```

Run a specified script with a given argument:

```Shell
dotnet csi Samples/Scripts/hello.csx World 
```

Run a single script located in the _MyDirectory_ directory:

```Shell
dotnet csi Samples/Build
```

Usage:

```Shell
dotnet csi [options] [--] [script] [script arguments]
```

Executes a script if specified, otherwise launches an interactive REPL (Read Eval Print Loop).

Supported arguments:

| Option                  | Description                                                                                                                                                     | Alternative form                                                                              |
|:------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------|-----------------------------------------------------------------------------------------------|
| script                  | The path to the script file to run. If no such file is found, the command will treat it as a directory and look for a single script file inside that directory. |                                                                                               |
| script arguments        | Script arguments are accessible in a script via the global list Args[index] by an argument index.                                                               |                                                                                               |
| --                      | Indicates that the remaining arguments should not be treated as options.                                                                                        |                                                                                               |
| --help                  | Show how to use the command.                                                                                                                                    | `/?`, `-h`, `/h`, `/help`                                                                     |
| --version               | Display the tool version.                                                                                                                                       | `/version`                                                                                    |
| --source                | Specify the NuGet package source to use. Supported formats: URL, or a UNC directory path.                                                                       | `-s`, `/s`, `/source`                                                                         |
| --property <key=value>  | Define a key-value pair(s) for the script properties called _Props_, which is accessible in scripts.                                                            | `-p`, `/property`, `/p`                                                                       |
| --property:<key=value>  | Define a key-value pair(s) in MSBuild style for the script properties called _Props_, which is accessible in scripts.                                           | `-p:<key=value>`, `/property:<key=value>`, `/p:<key=value>`, `--property:key1=val1;key2=val2` |
| @file                   | Read the response file for more options.                                                                                                                        |                                                                                               |

```using HostApi;``` directive in a script allows you to use host API types without specifying the fully qualified namespace of these types.

## Debug scripts easy!

Install the C# script template [TeamCity.CSharpInteractive.Templates](https://www.nuget.org/packages/TeamCity.CSharpInteractive.Templates)

```shell
dotnet new -i TeamCity.CSharpInteractive.Templates
```

Create a console project "Build" containing a script from the template *__build__*

```shell
dotnet new build -o ./Build
```

This projects contains the script *__./Build/Program.csx__*. To run this script from the command line from the directory *__Build__*:

```shell
dotnet csi Build
```

To run this script as a console application:

```shell
dotnet run --project Build
```

Open the *__./Build/Build.csproj__* in IDE and debug the script.

## Report and Track Issues

Please use our YouTrack
to [report](https://youtrack.jetbrains.com/newIssue?project=TW&description=Expected%20behavior%20and%20actual%20behavior%3A%0A%0ASteps%20to%20reproduce%20the%20problem%3A%0A%0ASpecifications%20like%20the%20tool%20version%2C%20operating%20system%3A%0A%0AResult%20of%20'dotnet%20--info'%3A&c=Subsystem%20Agent%20-%20.NET&c=Assignee%20Nikolay.Pianikov&c=tag%20.NET%20Core&c=tag%20cs%20script%20step)
related issues.

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
  - [Build a project](#build-a-project)
  - [Build a project using MSBuild](#build-a-project-using-msbuild)
  - [Clean a project](#clean-a-project)
  - [Pack a project](#pack-a-project)
  - [Publish a project](#publish-a-project)
  - [Restore a project](#restore-a-project)
  - [Restore local tools](#restore-local-tools)
  - [Run a custom .NET command](#run-a-custom-.net-command)
  - [Run a project](#run-a-project)
  - [Run tests under dotCover](#run-tests-under-dotcover)
  - [Test a project](#test-a-project)
  - [Test a project using the MSBuild VSTest target](#test-a-project-using-the-msbuild-vstest-target)
  - [Test an assembly](#test-an-assembly)
  - [Shuts down build servers](#shuts-down-build-servers)
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

// Creates and run a simple command line 
"whoami".AsCommandLine().Run();

// Creates and run a simple command line 
new CommandLine("whoami").Run();

// Creates and run a command line with arguments 
new CommandLine("cmd", "/c", "echo", "Hello").Run();

// Same as previous statement
new CommandLine("cmd", "/c")
    .AddArgs("echo", "Hello")
    .Run();

(new CommandLine("cmd") + "/c" + "echo" + "Hello").Run();

("cmd".AsCommandLine("/c", "echo", "Hello")).Run();

("cmd".AsCommandLine() + "/c" + "echo" + "Hello").Run();

// Just builds a command line with multiple environment variables
var cmd = new CommandLine("cmd", "/c", "echo", "Hello")
    .AddVars(("Var1", "val1"), ("var2", "Val2"));

// Same as previous statement
cmd = new CommandLine("cmd") + "/c" + "echo" + "Hello" + ("Var1", "val1") + ("var2", "Val2");

// Builds a command line to run from a specific working directory 
cmd = new CommandLine("cmd", "/c", "echo", "Hello")
    .WithWorkingDirectory("MyDyrectory");

// Builds a command line and replaces all command line arguments
cmd = new CommandLine("cmd", "/c", "echo", "Hello")
    .WithArgs("/c", "echo", "Hello !!!");
```



### Run a command line



``` CSharp
// Adds the namespace "HostApi" to use Command Line API
using HostApi;

var exitCode = GetService<ICommandLineRunner>().Run(new CommandLine("cmd", "/c", "DIR"));
exitCode.ShouldBe(0);

// or the same thing using the extension method
exitCode = new CommandLine("cmd", "/c", "DIR").Run();
exitCode.ShouldBe(0);

// using operator '+'
var cmd = new CommandLine("cmd") + "/c" + "DIR";
exitCode = cmd.Run();
exitCode.ShouldBe(0);

// with environment variables
cmd = new CommandLine("cmd") + "/c" + "DIR" + ("MyEnvVar", "Some Value");
exitCode = cmd.Run();
exitCode.ShouldBe(0);
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
int? exitCode = new CommandLine("cmd", "/c", "SET")
    .AddVars(("MyEnv", "MyVal"))
    .Run(output => lines.Add(output.Line));

lines.ShouldContain("MyEnv=MyVal");
```



### Run asynchronously in parallel



``` CSharp
// Adds the namespace "HostApi" to use Command Line API
using HostApi;

Task<int?> task = new CommandLine("cmd", "/c", "DIR").RunAsync();
int? exitCode = new CommandLine("cmd", "/c", "SET").Run();
task.Wait();
```



### Cancellation of asynchronous run

The cancellation will kill a related process.

``` CSharp
// Adds the namespace "HostApi" to use Command Line API
using HostApi;

var cancellationTokenSource = new CancellationTokenSource();
Task<int?> task = new CommandLine("cmd", "/c", "TIMEOUT", "/T", "120")
    .RunAsync(default, cancellationTokenSource.Token);

cancellationTokenSource.CancelAfter(TimeSpan.FromMilliseconds(100));
task.IsCompleted.ShouldBeFalse();
```



### Run timeout

If timeout expired a process will be killed.

``` CSharp
// Adds the namespace "HostApi" to use Command Line API
using HostApi;

int? exitCode = new CommandLine("cmd", "/c", "TIMEOUT", "/T", "120")
    .Run(default, TimeSpan.FromMilliseconds(1));

exitCode.HasValue.ShouldBeFalse();
```



### Build a project



``` CSharp
// Adds the namespace "HostApi" to use .NET build API
using HostApi;

// Creates a new library project, running a command like: "dotnet new classlib -n MyLib --force"
var result = new DotNetNew("classlib", "-n", "MyLib", "--force").Build();
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
var result = new DotNetNew("classlib", "-n", "MyLib", "--force").Build();
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
NuGetVersion? version = default;
var exitCode = new DotNetCustom("--version")
    .Run(message => NuGetVersion.TryParse(message.Line, out version));

exitCode.ShouldBe(0);
version.ShouldNotBeNull();
```



### Test a project using the MSBuild VSTest target



``` CSharp
// Adds the namespace "HostApi" to use .NET build API
using HostApi;

// Creates a new test project, running a command like: "dotnet new mstest -n MyTests --force"
var result = new DotNetNew("mstest", "-n", "MyTests", "--force").Build();
result.ExitCode.ShouldBe(0);

// Runs tests via a command like: "dotnet msbuild /t:VSTest" from the directory "MyTests"
result = new MSBuild()
    .WithTarget("VSTest")
    .WithWorkingDirectory("MyTests").Build();

// The "result" variable provides details about a build
result.ExitCode.ShouldBe(0);
result.Summary.Tests.ShouldBe(1);
result.Tests.Count(test => test.State == TestState.Passed).ShouldBe(1);
```



### Pack a project



``` CSharp
// Adds the namespace "HostApi" to use .NET build API
using HostApi;

// Creates a new library project, running a command like: "dotnet new classlib -n MyLib --force"
var result = new DotNetNew("classlib", "-n", "MyLib", "--force").Build();
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
var result = new DotNetNew("classlib", "-n", "MyLib", "--force", "-f", "net6.0").Build();
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
var result = new DotNetNew("classlib", "-n", "MyLib", "--force").Build();
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
var result = new DotNetNew("console", "-n", "MyApp", "--force").Build();
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
var result = new DotNetNew("mstest", "-n", "MyTests", "--force").Build();
result.ExitCode.ShouldBe(0);

// Runs tests via a command like: "dotnet test" from the directory "MyTests"
result = new DotNetTest().WithWorkingDirectory("MyTests").Build();

// The "result" variable provides details about a build
result.ExitCode.ShouldBe(0);
result.Summary.Tests.ShouldBe(1);
result.Tests.Count(test => test.State == TestState.Passed).ShouldBe(1);
```



### Run tests under dotCover



``` CSharp
// Adds the namespace "HostApi" to use .NET build API
using HostApi;

// Creates a new test project, running a command like: "dotnet new mstest -n MyTests --force"
var exitCode = new DotNetNew("mstest", "-n", "MyTests", "--force").Run();
exitCode.ShouldBe(0);

// Creates the tool manifest and installs the dotCover tool locally
// It is better to run the following 2 commands manually
// and commit these changes to a source control
exitCode = new DotNetNew("tool-manifest").Run();
exitCode.ShouldBe(0);

exitCode = new DotNetCustom("tool",  "install", "--local", "JetBrains.dotCover.GlobalTool").Run();
exitCode.ShouldBe(0);

// Creates a test command
var test = new DotNetTest().WithProject("MyTests");

var dotCoverSnapshot = Path.Combine("MyTests", "dotCover.dcvr");
var dotCoverReport = Path.Combine("MyTests", "dotCover.html");
// Modifies the test command by putting "dotCover" in front of all arguments
// to have something like "dotnet dotcover test ..."
// and adding few specific arguments to the end
var testUnderDotCover = test.Customize(cmd =>
    cmd.ClearArgs()
    + "dotcover"
    + cmd.Args
    + $"--dcOutput={dotCoverSnapshot}"
    + "--dcFilters=+:module=TeamCity.CSharpInteractive.HostApi;+:module=dotnet-csi"
    + "--dcAttributeFilters=System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage");
    
// Runs tests under dotCover via a command like: "dotnet dotcover test ..."
var result = testUnderDotCover.Build();

// The "result" variable provides details about a build
result.ExitCode.ShouldBe(0);
result.Tests.Count(test => test.State == TestState.Passed).ShouldBe(1);

// Generates a HTML code coverage report.
exitCode = new DotNetCustom("dotCover", "report", $"--source={dotCoverSnapshot}", $"--output={dotCoverReport}", "--reportType=HTML").Run();
exitCode.ShouldBe(0);

// Check for a dotCover report
File.Exists(dotCoverReport).ShouldBeTrue();
```



### Restore local tools



``` CSharp
// Adds the namespace "HostApi" to use .NET build API
using HostApi;

var projectDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()[..4]);
Directory.CreateDirectory(projectDir);
    
// Creates a local tool manifest 
var exitCode = new DotNetNew("tool-manifest").WithWorkingDirectory(projectDir).Run();
exitCode.ShouldBe(0);

// Restore local tools
exitCode = new DotNetToolRestore().WithWorkingDirectory(projectDir).Run();
exitCode.ShouldBe(0);
```



### Test an assembly



``` CSharp
// Adds the namespace "HostApi" to use .NET build API
using HostApi;

// Creates a new test project, running a command like: "dotnet new mstest -n MyTests --force"
var result = new DotNetNew("mstest", "-n", "MyTests", "--force").Build();
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
result.ExitCode.ShouldBe(0);
result.Summary.Tests.ShouldBe(1);
result.Tests.Count(test => test.State == TestState.Passed).ShouldBe(1);
```



### Build a project using MSBuild



``` CSharp
// Adds the namespace "HostApi" to use .NET build API
using HostApi;

// Creates a new library project, running a command like: "dotnet new classlib -n MyLib --force"
var result = new DotNetNew("classlib", "-n", "MyLib", "--force").Build();
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



### Shuts down build servers



``` CSharp
// Adds the namespace "HostApi" to use .NET build API
using HostApi;

// Shuts down all build servers that are started from dotnet.
var exitCode = new DotNetBuildServerShutdown().Run();

exitCode.ShouldBe(0);
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

// Creates a base docker command line
var dockerRun = new DockerRun()
    .WithAutoRemove(true)
    .WithImage("mcr.microsoft.com/dotnet/sdk")
    .WithPlatform("linux")
    .WithContainerWorkingDirectory("/MyProjects")
    .AddVolumes((Environment.CurrentDirectory, "/MyProjects"));

// Creates a new library project in a docker container
var exitCode = dockerRun
    .WithCommandLine(new DotNetCustom("new", "classlib", "-n", "MyLib", "--force"))
    .Run();

exitCode.ShouldBe(0);

// Builds the library project in a docker container
var result = dockerRun
    .WithCommandLine(new DotNetBuild().WithProject("MyLib/MyLib.csproj"))
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

