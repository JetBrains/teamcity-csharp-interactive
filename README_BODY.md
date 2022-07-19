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
