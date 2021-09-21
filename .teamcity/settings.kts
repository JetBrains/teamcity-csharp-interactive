import jetbrains.buildServer.configs.kotlin.v2019_2.BuildType
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.*
import jetbrains.buildServer.configs.kotlin.v2019_2.project
import jetbrains.buildServer.configs.kotlin.v2019_2.vcs.GitVcsRoot
import jetbrains.buildServer.configs.kotlin.v2019_2.version

version = "2021.1"

project {
    vcsRoot(CSharpScriptRepo)
    buildType(HelloWorldBuildType)
    buildType(BuildAndDeployBuildType)
}

object CSharpScriptRepo : GitVcsRoot({
    name = "C# Script"
    url = "https://github.com/JetBrains/teamcity-csharp-interactive.git"
    branch = "refs/heads/master"
})

object HelloWorldBuildType: BuildType({
    name = "Say hello"
    steps {
        csharpScript {
            content = "WriteLine(\"Hello World from project \" + Args[0])"
            arguments = "%system.teamcity.projectName%"
        }
    }
})

object BuildAndDeployBuildType: BuildType({
    name = "Build and deploy"
    params {
        param("demo.package", "MySampleLib")
        param("demo.path", "Samples/DemoProject/MySampleLib")
        param("system.configuration", "Release")
        param("system.version", "1.0.0")
    }
    vcs { root(CSharpScriptRepo) }
    steps {
        csharpScript {
            name = "Evaluate a next NuGet package version"
            content =
                    "using System.Linq;\n" +
                    "Props[\"version\"] = \n" +
                    "  GetService<INuGet>()\n" +
                    "  .Restore(Args[0], \"*\", \"netcoreapp3.1\")\n" +
                    "  .Where(i => i.Name == Args[0])\n" +
                    "  .Select(i => i.Version)\n" +
                    "  .Select(i => new Version(i.Major, i.Minor, i.Build + 1))\n" +
                    "  .DefaultIfEmpty(new Version(1, 0, 0))\n" +
                    "  .Max()\n" +
                    "  .ToString();\n" +
                    "WriteLine($\"Version: {Props[\"version\"]}\", Success);"
            arguments = "%demo.package%"
        }
        dotnetBuild {
            name = "Build library"
            workingDir = "%demo.path%"
            sdk = "3.1"
        }
        dotnetTest {
            name = "Run tests"
            workingDir = "%demo.path%"
            skipBuild = true
        }
        csharpFile {
            name = "Make a pol"
            path = "Samples/Scripts/TelegramBot.csx"
            arguments = "%teamcity.serverUrl%/viewLog.html?buildId=%teamcity.build.id%&buildTypeId=%system.teamcity.buildType.id%&guest=1"
        }
        dotnetPack {
            name = "Create a NuGet package"
            workingDir = "%demo.path%"
            skipBuild = true
        }
        dotnetNugetPush {
            name = "Push the NuGet package"
            packages = "%demo.path%/%demo.package%/bin/%system.configuration%/%demo.package%.%system.version%.nupkg"
            serverUrl = "https://api.nuget.org/v3/index.json"
            apiKey = "%NuGetKey%"
        }
    }
})