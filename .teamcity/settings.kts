import jetbrains.buildServer.configs.kotlin.v2019_2.*
import jetbrains.buildServer.configs.kotlin.v2019_2.buildFeatures.*
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.*
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.csharpFile
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.csharpScript
import jetbrains.buildServer.configs.kotlin.v2019_2.vcs.*

version = "2021.1"

project {
    vcsRoot(CSharpScriptRepo)
    buildType(HelloWorldBuildType)
    buildType(InteractiveBuildType)
    buildType(PushNuGetPackageBuildType)
}

object CSharpScriptRepo : GitVcsRoot({
    name = "C# Script"
    //url = "C:\\Projects\\TeamCity\\Teamcity.CSharpInteractive"
    url = "https://github.com/JetBrains/teamcity-csharp-interactive.git"
    branch = "refs/heads/master"
})

object HelloWorldBuildType: BuildType({
    name = "Say hello"
    steps {
        csharpScript {
            content = "WriteLine(\"Hello World from project \" + Args[0])"
            arguments = """"%system.teamcity.projectName%""""
        }
    }
})

object InteractiveBuildType: BuildType({
    name = "Interactive build"
    vcs { root(CSharpScriptRepo) }
    steps {
        dotnetCustom { args = "new xunit -n Tests --force" }
        dotnetTest { workingDir = "Tests" }
        csharpFile {
            path = "Samples/Scripts/TelegramBot.csx"
            arguments = """"%teamcity.serverUrl%/viewLog.html?buildId=%teamcity.build.id%&buildTypeId=%system.teamcity.buildType.id%&guest=1""""
        }
        script { scriptContent = "echo Deploying ..." }
    }
})

object PushNuGetPackageBuildType: BuildType({
    name = "Push NuGet package"
    params {
        param("system.version", "1.0.0")
    }
    vcs { root(CSharpScriptRepo) }
    steps {
        csharpScript {
            content =
                    "using System.Linq;\n" +
                    "Props[\"version\"] = \n" +
                    "  GetService<INuGet>()\n" +
                    "  .Restore(Args[0], \"*\")\n" +
                    "  .Where(i => i.Name == Args[0])\n" +
                    "  .Select(i => i.Version)\n" +
                    "  .Select(i => new Version(i.Major, i.Minor, i.Build + 1))\n" +
                    "  .DefaultIfEmpty(new Version(1, 0, 0))\n" +
                    "  .Max()\n" +
                    "  .ToString();"
            arguments = "MySampleLib"
        }
        dotnetCustom { args = "new classlib -n MySampleLib --force" }
        dotnetPack { workingDir = "MySampleLib" }
        script { scriptContent = "echo Pushing the NuGet package MySampleLib version %system.version% ..." }
    }
})