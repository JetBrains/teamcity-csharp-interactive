## C# script tool for [<img src="https://cdn.worldvectorlogo.com/logos/teamcity.svg" height="20" align="center"/>](https://www.jetbrains.com/teamcity/)

[<img src="http://jb.gg/badges/official.svg"/>](https://confluence.jetbrains.com/display/ALL/JetBrains+on+GitHub) [![NuGet TeamCity.csi](https://buildstats.info/nuget/TeamCity.csi?includePreReleases=true)](https://www.nuget.org/packages/TeamCity.csi) ![GitHub](https://img.shields.io/github/license/jetbrains/teamcity-csharp-interactive) [<img src="http://teamcity.jetbrains.com/app/rest/builds/buildType:(id:TeamCityPluginsByJetBrains_TeamCityCScript_BuildAndTestBuildType)/statusIcon.svg"/>](http://teamcity.jetbrains.com/viewType.html?buildTypeId=TeamCityPluginsByJetBrains_TeamCityCScript_BuildAndTestBuildType&guest=1)

This is a repository of TeamCity.csi which is an interactive tool for running C# scripts. It can be used as a [TeamCity build runner](https://github.com/JetBrains/teamcity-dotnet-plugin#c-script-runner) or installed as a command-line tool on Windows, Linux, or macOS.

## Prerequisites

The tool requires .NET runtime 3.1.

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
dotnet csi <script_name>.csx --<arg>
```

Supported arguments:

<table>

<tr><td>Argument</td><td>Description</td><td>Alternative form</td></tr>

<tr>
<td>

`--help`

</td>
<td>

Show how to use the command.

</td>
<td>

`/?`, `-h`, `/h`, `/help`

</td>
</tr>

<tr>
<td>

`--version`

</td>
<td>

Display the tool version.

</td>
<td>

`/version`

</td>
</tr>

<tr>
<td>

`--source`

</td>
<td>

Specify the NuGet package source to use. Supported formats: URL, or a UNC directory path.

</td>
<td>

`-s`, `/s`, `/source`

</td>
</tr>

<tr>
<td>

`--property <key=value>`

</td>
<td>

Define a `key=value` pair for the global dictionary called `Props`, which is accessible in scripts.

</td>
<td>

`-p`, `/property`, `/p`

</td>
</tr>

<tr>
<td>

`@file`

</td>
<td>

Read the response file for more options.

</td>
<td></td>
</tr>

<tr>
<td>

`--`

</td>
<td>

Indicates that the remaining arguments should not be treated as options.

</td>
<td>
</td>
</tr>

</table>


## Report and Track Issues

Please use our YouTrack to [report](https://youtrack.jetbrains.com/newIssue?project=TW&description=Expected%20behavior%20and%20actual%20behavior%3A%0A%0ASteps%20to%20reproduce%20the%20problem%3A%0A%0ASpecifications%20like%20the%20tool%20version%2C%20operating%20system%3A%0A%0AResult%20of%20'dotnet%20--info'%3A&c=Subsystem%20Agent%20-%20.NET&c=Assignee%20Nikolay.Pianikov&c=tag%20.NET%20Core&c=tag%20cs%20script%20step) related issues.
