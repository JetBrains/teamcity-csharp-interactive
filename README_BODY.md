## C# script tool for [<img src="https://cdn.worldvectorlogo.com/logos/teamcity.svg" height="20" align="center"/>](https://www.jetbrains.com/teamcity/)

[<img src="http://jb.gg/badges/official.svg"/>](https://confluence.jetbrains.com/display/ALL/JetBrains+on+GitHub) [![NuGet TeamCity.csi](https://buildstats.info/nuget/TeamCity.csi?includePreReleases=true)](https://www.nuget.org/packages/TeamCity.csi) ![GitHub](https://img.shields.io/github/license/jetbrains/teamcity-csharp-interactive) [<img src="http://teamcity.jetbrains.com/app/rest/builds/buildType:(id:TeamCityPluginsByJetBrains_TeamCityCScript_BuildAndTestBuildType)/statusIcon.svg"/>](http://teamcity.jetbrains.com/viewType.html?buildTypeId=TeamCityPluginsByJetBrains_TeamCityCScript_BuildAndTestBuildType&guest=1)

This is a repository of TeamCity.csi which is an interactive tool for running C# scripts. It can be used as
a [TeamCity build runner](https://github.com/JetBrains/teamcity-dotnet-plugin#c-script-runner) or installed as a
command-line tool on Windows, Linux, or macOS.

## Prerequisites

The tool requires .NET 6+ runtime.

## Use Inside TeamCity

Currently, the tool can be used as a TeamCity build runner provided in terms of TeamCity
2021.2 [Early Access Program](https://www.jetbrains.com/teamcity/nextversion/). Read the runner's [documentation]() for
more details.

## Use Outside TeamCity

After installing tool you can use this tool independently of TeamCity, to run C# scripts from the command line. TeamCity.csi is available as a [NuGet package](https://www.nuget.org/packages/TeamCity.csi/). Before installing TeamCity.csi as a local tool dot not forget to create .NET local tool manifest file if it is not exist. Install the tool and add to the local tool manifest:

```Shell
dotnet new tool-manifest
dotnet tool install TeamCity.csi --version <version>
```

Or install the tool for the current user:

```Shell
dotnet tool install TeamCity.csi -g --version <version>
```

Launch the tool in the interactive mode:

```Shell
dotnet csi
```

Run a specified script with a given argument:

```Shell
dotnet csi script-file.csx
```

Run a single script located in the _MyDirectory_ directory:

```Shell
dotnet csi MyDirectory
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

## Command line sample

```Shell
echo Creates a new solution "MySolution" in the current directory.
dotnet new sln -n MySolution

echo Creates a sample project "MyLib" and adds it to the solution "MySolution".
dotnet new classlib -n MyLib
dotnet sln add MyLib

echo Installs template "build".
dotnet new install TeamCity.CSharpInteractive.Templates

echo Creates a sample build project "Build" using the template "build" and adds it to the solution "MySolution".
dotnet new build -n Build
dotnet sln add Build

echo Creates a local manifest file for the solution "MySolution" and installs the .NET tool TeamCity.csi locally.
dotnet new tool-manifest
dotnet tool install TeamCity.csi

echo Runs a script from the "Build" project to build solution "MySolution" from the solution directory.
dotnet csi Build
```

You can modify, debug and run the project "Build" as a ordinary .NET console application and run it as a C# script using ```dotnet csi Build``` from the command line.

## Report and Track Issues

Please use our YouTrack
to [report](https://youtrack.jetbrains.com/newIssue?project=TW&description=Expected%20behavior%20and%20actual%20behavior%3A%0A%0ASteps%20to%20reproduce%20the%20problem%3A%0A%0ASpecifications%20like%20the%20tool%20version%2C%20operating%20system%3A%0A%0AResult%20of%20'dotnet%20--info'%3A&c=Subsystem%20Agent%20-%20.NET&c=Assignee%20Nikolay.Pianikov&c=tag%20.NET%20Core&c=tag%20cs%20script%20step)
related issues.
