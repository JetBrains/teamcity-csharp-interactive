import jetbrains.buildServer.configs.kotlin.v2019_2.*
import jetbrains.buildServer.configs.kotlin.v2019_2.buildFeatures.*
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.*
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.csharpFile
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.csharpScript
import jetbrains.buildServer.configs.kotlin.v2019_2.vcs.*

version = "2021.1"

project {
    vcsRoot(CSharpScriptRepo)
    subProject(DemoProject)
}

object CSharpScriptRepo : GitVcsRoot({
    name = "C# Script"
    //url = "C:\\Projects\\TeamCity\\Teamcity.CSharpInteractive"
    url = "https://github.com/JetBrains/teamcity-csharp-interactive.git"
    branch = "refs/heads/master"
})

object DemoProject: Project({
    name = "Demo"
    buildType(HelloWorldBuildType)
    buildType(InteractiveBuildType)
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
        script {
            scriptContent = "echo Deploying ..."
        }
    }
})