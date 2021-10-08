import jetbrains.buildServer.configs.kotlin.v2019_2.*
import jetbrains.buildServer.configs.kotlin.v2019_2.buildFeatures.swabra
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.*
import jetbrains.buildServer.configs.kotlin.v2019_2.triggers.vcs
import jetbrains.buildServer.configs.kotlin.v2019_2.vcs.GitVcsRoot

version = "2021.1"

// Build settings
open class Settings {
    companion object {
        private const val dotnetToolVersion = "6.0"
        private const val dotnetSampleVersion = "3.1"

        const val dockerImageSdk = "mcr.microsoft.com/dotnet/sdk:$dotnetToolVersion"
        const val dockerImageRuntime = "mcr.microsoft.com/dotnet/runtime:$dotnetToolVersion"
        const val dockerImageSampleSdk = "mcr.microsoft.com/dotnet/sdk:$dotnetSampleVersion"
    }
}

object CSharpScriptRepo : GitVcsRoot({
    name = "C# Script"
    url = "https://github.com/JetBrains/teamcity-csharp-interactive.git"
    branch = "refs/heads/demo"
})

project {
    vcsRoot(CSharpScriptRepo)
    buildType(BuildAndTestBuildType)
    //subProject(DemoProject)
}

object BuildAndTestBuildType: BuildType({
    name = "Build and test"
    artifactRules = "Teamcity.CSharpInteractive/bin/Release/TeamCity.csi.*.nupkg => ."

    params {
        param("system.version", "1.0.0")
    }

    vcs { root(CSharpScriptRepo) }

    steps {
        dotnetTest {
            name = "Run tests"
            sdk = "5"
            dockerImage = Settings.dockerImageSdk
            dockerImagePlatform = DotnetTestStep.ImagePlatform.Linux
        }

        dotnetPack {
            name = "Pack"
            sdk = "5"
            executionMode = BuildStep.ExecutionMode.RUN_ON_SUCCESS
            configuration = "Release"
            dockerImage = Settings.dockerImageSdk
            dockerImagePlatform = DotnetPackStep.ImagePlatform.Linux
        }
    }

    triggers {
        vcs {
        }
    }

    features {
        swabra {
        }
    }

    failureConditions {
        nonZeroExitCode = true
        testFailure = true
        errorMessage = true
    }
})

/*
object DemoProject: Project({
    name = "Demo"
    buildType(HelloWorldBuildType)
    buildType(BuildAndDeployBuildType)
})

object HelloWorldBuildType: BuildType({
    name = "Say hello"
    steps {
        csharpScript {
            content = "WriteLine(\"Hello World from project \" + Args[0])"
            arguments = "%system.teamcity.projectName%"
            dockerImage = Settings.dockerImageRuntime
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
            dockerImage = Settings.dockerImageRuntime
        }
        dotnetBuild {
            name = "Build library"
            workingDir = "%demo.path%"
            sdk = "3.1"
            dockerImage = Settings.dockerImageSampleSdk
        }
        dotnetTest {
            name = "Run tests"
            workingDir = "%demo.path%"
            skipBuild = true
            dockerImage = Settings.dockerImageSampleSdk
        }
        csharpFile {
            name = "Make a pol"
            path = "Samples/Scripts/TelegramBot.csx"
            arguments = "%teamcity.serverUrl%/viewLog.html?buildId=%teamcity.build.id%&buildTypeId=%system.teamcity.buildType.id%&guest=1"
            dockerImage = Settings.dockerImageRuntime
        }
        dotnetPack {
            name = "Create a NuGet package"
            workingDir = "%demo.path%"
            skipBuild = true
            dockerImage = Settings.dockerImageSampleSdk
        }
        dotnetNugetPush {
            name = "Push the NuGet package"
            packages = "%demo.path%/%demo.package%/bin/%system.configuration%/%demo.package%.%system.version%.nupkg"
            serverUrl = "https://api.nuget.org/v3/index.json"
            apiKey = "%NuGetKey%"
            dockerImage = Settings.dockerImageSampleSdk
        }
    }
})
*/