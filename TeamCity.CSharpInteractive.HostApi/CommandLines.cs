// ReSharper disable InconsistentNaming
namespace HostApi;
    
public partial record CommandLine: ICommandLine
{
    public static CommandLine operator +(CommandLine command, string arg) => command.AddArgs(arg);
    
    public static CommandLine operator -(CommandLine command, string arg) => command.RemoveArgs(arg);
    
    public static CommandLine operator +(CommandLine command, IEnumerable<string> args) => command.AddArgs(args);

    public static CommandLine operator -(CommandLine command, IEnumerable<string> args) => command.RemoveArgs(args);
    
    public static CommandLine operator +(CommandLine command, (string name, string value) var) => command.AddVars(var);
    
    public static CommandLine operator -(CommandLine command, (string name, string value) var) => command.RemoveVars(var);
    
    public static CommandLine operator +(CommandLine command, IEnumerable<(string name, string value)> vars) => command.AddVars(vars);
    
    public static CommandLine operator -(CommandLine command, IEnumerable<(string name, string value)> vars) => command.RemoveVars(vars);
}

public partial record DotNetBuild: ICommandLine
{
    public static DotNetBuild operator +(DotNetBuild command, string arg) => command.AddArgs(arg);
    
    public static DotNetBuild operator -(DotNetBuild command, string arg) => command.RemoveArgs(arg);
    
    public static DotNetBuild operator +(DotNetBuild command, IEnumerable<string> args) => command.AddArgs(args);

    public static DotNetBuild operator -(DotNetBuild command, IEnumerable<string> args) => command.RemoveArgs(args);
    
    public static DotNetBuild operator +(DotNetBuild command, (string name, string value) var) => command.AddVars(var);
    
    public static DotNetBuild operator -(DotNetBuild command, (string name, string value) var) => command.RemoveVars(var);
    
    public static DotNetBuild operator +(DotNetBuild command, IEnumerable<(string name, string value)> vars) => command.AddVars(vars);
    
    public static DotNetBuild operator -(DotNetBuild command, IEnumerable<(string name, string value)> vars) => command.RemoveVars(vars);
}

public partial record DotNetBuildServerShutdown: ICommandLine
{
    public static DotNetBuildServerShutdown operator +(DotNetBuildServerShutdown command, string arg) => command.AddArgs(arg);
    
    public static DotNetBuildServerShutdown operator -(DotNetBuildServerShutdown command, string arg) => command.RemoveArgs(arg);
    
    public static DotNetBuildServerShutdown operator +(DotNetBuildServerShutdown command, IEnumerable<string> args) => command.AddArgs(args);

    public static DotNetBuildServerShutdown operator -(DotNetBuildServerShutdown command, IEnumerable<string> args) => command.RemoveArgs(args);
    
    public static DotNetBuildServerShutdown operator +(DotNetBuildServerShutdown command, (string name, string value) var) => command.AddVars(var);
    
    public static DotNetBuildServerShutdown operator -(DotNetBuildServerShutdown command, (string name, string value) var) => command.RemoveVars(var);
    
    public static DotNetBuildServerShutdown operator +(DotNetBuildServerShutdown command, IEnumerable<(string name, string value)> vars) => command.AddVars(vars);
    
    public static DotNetBuildServerShutdown operator -(DotNetBuildServerShutdown command, IEnumerable<(string name, string value)> vars) => command.RemoveVars(vars);
}

public partial record DotNetClean: ICommandLine
{
    public static DotNetClean operator +(DotNetClean command, string arg) => command.AddArgs(arg);
    
    public static DotNetClean operator -(DotNetClean command, string arg) => command.RemoveArgs(arg);
    
    public static DotNetClean operator +(DotNetClean command, IEnumerable<string> args) => command.AddArgs(args);

    public static DotNetClean operator -(DotNetClean command, IEnumerable<string> args) => command.RemoveArgs(args);
    
    public static DotNetClean operator +(DotNetClean command, (string name, string value) var) => command.AddVars(var);
    
    public static DotNetClean operator -(DotNetClean command, (string name, string value) var) => command.RemoveVars(var);
    
    public static DotNetClean operator +(DotNetClean command, IEnumerable<(string name, string value)> vars) => command.AddVars(vars);
    
    public static DotNetClean operator -(DotNetClean command, IEnumerable<(string name, string value)> vars) => command.RemoveVars(vars);
}

public partial record DotNetCustom: ICommandLine
{
    public static DotNetCustom operator +(DotNetCustom command, string arg) => command.AddArgs(arg);
    
    public static DotNetCustom operator -(DotNetCustom command, string arg) => command.RemoveArgs(arg);
    
    public static DotNetCustom operator +(DotNetCustom command, IEnumerable<string> args) => command.AddArgs(args);

    public static DotNetCustom operator -(DotNetCustom command, IEnumerable<string> args) => command.RemoveArgs(args);
    
    public static DotNetCustom operator +(DotNetCustom command, (string name, string value) var) => command.AddVars(var);
    
    public static DotNetCustom operator -(DotNetCustom command, (string name, string value) var) => command.RemoveVars(var);
    
    public static DotNetCustom operator +(DotNetCustom command, IEnumerable<(string name, string value)> vars) => command.AddVars(vars);
    
    public static DotNetCustom operator -(DotNetCustom command, IEnumerable<(string name, string value)> vars) => command.RemoveVars(vars);
}

public partial record DotNetNew: ICommandLine
{
    public static DotNetNew operator +(DotNetNew command, string arg) => command.AddArgs(arg);
    
    public static DotNetNew operator -(DotNetNew command, string arg) => command.RemoveArgs(arg);
    
    public static DotNetNew operator +(DotNetNew command, IEnumerable<string> args) => command.AddArgs(args);

    public static DotNetNew operator -(DotNetNew command, IEnumerable<string> args) => command.RemoveArgs(args);
    
    public static DotNetNew operator +(DotNetNew command, (string name, string value) var) => command.AddVars(var);
    
    public static DotNetNew operator -(DotNetNew command, (string name, string value) var) => command.RemoveVars(var);
    
    public static DotNetNew operator +(DotNetNew command, IEnumerable<(string name, string value)> vars) => command.AddVars(vars);
    
    public static DotNetNew operator -(DotNetNew command, IEnumerable<(string name, string value)> vars) => command.RemoveVars(vars);
}

public partial record DotNetNuGetPush: ICommandLine
{
    public static DotNetNuGetPush operator +(DotNetNuGetPush command, string arg) => command.AddArgs(arg);
    
    public static DotNetNuGetPush operator -(DotNetNuGetPush command, string arg) => command.RemoveArgs(arg);
    
    public static DotNetNuGetPush operator +(DotNetNuGetPush command, IEnumerable<string> args) => command.AddArgs(args);

    public static DotNetNuGetPush operator -(DotNetNuGetPush command, IEnumerable<string> args) => command.RemoveArgs(args);
    
    public static DotNetNuGetPush operator +(DotNetNuGetPush command, (string name, string value) var) => command.AddVars(var);
    
    public static DotNetNuGetPush operator -(DotNetNuGetPush command, (string name, string value) var) => command.RemoveVars(var);
    
    public static DotNetNuGetPush operator +(DotNetNuGetPush command, IEnumerable<(string name, string value)> vars) => command.AddVars(vars);
    
    public static DotNetNuGetPush operator -(DotNetNuGetPush command, IEnumerable<(string name, string value)> vars) => command.RemoveVars(vars);
}

public partial record DotNetPack: ICommandLine
{
    public static DotNetPack operator +(DotNetPack command, string arg) => command.AddArgs(arg);
    
    public static DotNetPack operator -(DotNetPack command, string arg) => command.RemoveArgs(arg);
    
    public static DotNetPack operator +(DotNetPack command, IEnumerable<string> args) => command.AddArgs(args);

    public static DotNetPack operator -(DotNetPack command, IEnumerable<string> args) => command.RemoveArgs(args);
    
    public static DotNetPack operator +(DotNetPack command, (string name, string value) var) => command.AddVars(var);
    
    public static DotNetPack operator -(DotNetPack command, (string name, string value) var) => command.RemoveVars(var);
    
    public static DotNetPack operator +(DotNetPack command, IEnumerable<(string name, string value)> vars) => command.AddVars(vars);
    
    public static DotNetPack operator -(DotNetPack command, IEnumerable<(string name, string value)> vars) => command.RemoveVars(vars);
}

public partial record DotNetPublish: ICommandLine
{
    public static DotNetPublish operator +(DotNetPublish command, string arg) => command.AddArgs(arg);
    
    public static DotNetPublish operator -(DotNetPublish command, string arg) => command.RemoveArgs(arg);
    
    public static DotNetPublish operator +(DotNetPublish command, IEnumerable<string> args) => command.AddArgs(args);

    public static DotNetPublish operator -(DotNetPublish command, IEnumerable<string> args) => command.RemoveArgs(args);
    
    public static DotNetPublish operator +(DotNetPublish command, (string name, string value) var) => command.AddVars(var);
    
    public static DotNetPublish operator -(DotNetPublish command, (string name, string value) var) => command.RemoveVars(var);
    
    public static DotNetPublish operator +(DotNetPublish command, IEnumerable<(string name, string value)> vars) => command.AddVars(vars);
    
    public static DotNetPublish operator -(DotNetPublish command, IEnumerable<(string name, string value)> vars) => command.RemoveVars(vars);
}

public partial record DotNetRestore: ICommandLine
{
    public static DotNetRestore operator +(DotNetRestore command, string arg) => command.AddArgs(arg);
    
    public static DotNetRestore operator -(DotNetRestore command, string arg) => command.RemoveArgs(arg);
    
    public static DotNetRestore operator +(DotNetRestore command, IEnumerable<string> args) => command.AddArgs(args);

    public static DotNetRestore operator -(DotNetRestore command, IEnumerable<string> args) => command.RemoveArgs(args);
    
    public static DotNetRestore operator +(DotNetRestore command, (string name, string value) var) => command.AddVars(var);
    
    public static DotNetRestore operator -(DotNetRestore command, (string name, string value) var) => command.RemoveVars(var);
    
    public static DotNetRestore operator +(DotNetRestore command, IEnumerable<(string name, string value)> vars) => command.AddVars(vars);
    
    public static DotNetRestore operator -(DotNetRestore command, IEnumerable<(string name, string value)> vars) => command.RemoveVars(vars);
}

public partial record DotNetRun: ICommandLine
{
    public static DotNetRun operator +(DotNetRun command, string arg) => command.AddArgs(arg);
    
    public static DotNetRun operator -(DotNetRun command, string arg) => command.RemoveArgs(arg);
    
    public static DotNetRun operator +(DotNetRun command, IEnumerable<string> args) => command.AddArgs(args);

    public static DotNetRun operator -(DotNetRun command, IEnumerable<string> args) => command.RemoveArgs(args);
    
    public static DotNetRun operator +(DotNetRun command, (string name, string value) var) => command.AddVars(var);
    
    public static DotNetRun operator -(DotNetRun command, (string name, string value) var) => command.RemoveVars(var);
    
    public static DotNetRun operator +(DotNetRun command, IEnumerable<(string name, string value)> vars) => command.AddVars(vars);
    
    public static DotNetRun operator -(DotNetRun command, IEnumerable<(string name, string value)> vars) => command.RemoveVars(vars);
}

public partial record DotNetTest: ICommandLine
{
    public static DotNetTest operator +(DotNetTest command, string arg) => command.AddArgs(arg);
    
    public static DotNetTest operator -(DotNetTest command, string arg) => command.RemoveArgs(arg);
    
    public static DotNetTest operator +(DotNetTest command, IEnumerable<string> args) => command.AddArgs(args);

    public static DotNetTest operator -(DotNetTest command, IEnumerable<string> args) => command.RemoveArgs(args);
    
    public static DotNetTest operator +(DotNetTest command, (string name, string value) var) => command.AddVars(var);
    
    public static DotNetTest operator -(DotNetTest command, (string name, string value) var) => command.RemoveVars(var);
    
    public static DotNetTest operator +(DotNetTest command, IEnumerable<(string name, string value)> vars) => command.AddVars(vars);
    
    public static DotNetTest operator -(DotNetTest command, IEnumerable<(string name, string value)> vars) => command.RemoveVars(vars);
}

public partial record DotNetToolRestore: ICommandLine
{
    public static DotNetToolRestore operator +(DotNetToolRestore command, string arg) => command.AddArgs(arg);
    
    public static DotNetToolRestore operator -(DotNetToolRestore command, string arg) => command.RemoveArgs(arg);
    
    public static DotNetToolRestore operator +(DotNetToolRestore command, IEnumerable<string> args) => command.AddArgs(args);

    public static DotNetToolRestore operator -(DotNetToolRestore command, IEnumerable<string> args) => command.RemoveArgs(args);
    
    public static DotNetToolRestore operator +(DotNetToolRestore command, (string name, string value) var) => command.AddVars(var);
    
    public static DotNetToolRestore operator -(DotNetToolRestore command, (string name, string value) var) => command.RemoveVars(var);
    
    public static DotNetToolRestore operator +(DotNetToolRestore command, IEnumerable<(string name, string value)> vars) => command.AddVars(vars);
    
    public static DotNetToolRestore operator -(DotNetToolRestore command, IEnumerable<(string name, string value)> vars) => command.RemoveVars(vars);
}

public partial record MSBuild: ICommandLine
{
    public static MSBuild operator +(MSBuild command, string arg) => command.AddArgs(arg);
    
    public static MSBuild operator -(MSBuild command, string arg) => command.RemoveArgs(arg);
    
    public static MSBuild operator +(MSBuild command, IEnumerable<string> args) => command.AddArgs(args);

    public static MSBuild operator -(MSBuild command, IEnumerable<string> args) => command.RemoveArgs(args);
    
    public static MSBuild operator +(MSBuild command, (string name, string value) var) => command.AddVars(var);
    
    public static MSBuild operator -(MSBuild command, (string name, string value) var) => command.RemoveVars(var);
    
    public static MSBuild operator +(MSBuild command, IEnumerable<(string name, string value)> vars) => command.AddVars(vars);
    
    public static MSBuild operator -(MSBuild command, IEnumerable<(string name, string value)> vars) => command.RemoveVars(vars);
}

public partial record VSTest: ICommandLine
{
    public static VSTest operator +(VSTest command, string arg) => command.AddArgs(arg);
    
    public static VSTest operator -(VSTest command, string arg) => command.RemoveArgs(arg);
    
    public static VSTest operator +(VSTest command, IEnumerable<string> args) => command.AddArgs(args);

    public static VSTest operator -(VSTest command, IEnumerable<string> args) => command.RemoveArgs(args);
    
    public static VSTest operator +(VSTest command, (string name, string value) var) => command.AddVars(var);
    
    public static VSTest operator -(VSTest command, (string name, string value) var) => command.RemoveVars(var);
    
    public static VSTest operator +(VSTest command, IEnumerable<(string name, string value)> vars) => command.AddVars(vars);
    
    public static VSTest operator -(VSTest command, IEnumerable<(string name, string value)> vars) => command.RemoveVars(vars);
}

public partial record DockerCustom: ICommandLine
{
    public static DockerCustom operator +(DockerCustom command, string arg) => command.AddArgs(arg);
    
    public static DockerCustom operator -(DockerCustom command, string arg) => command.RemoveArgs(arg);
    
    public static DockerCustom operator +(DockerCustom command, IEnumerable<string> args) => command.AddArgs(args);

    public static DockerCustom operator -(DockerCustom command, IEnumerable<string> args) => command.RemoveArgs(args);
    
    public static DockerCustom operator +(DockerCustom command, (string name, string value) var) => command.AddVars(var);
    
    public static DockerCustom operator -(DockerCustom command, (string name, string value) var) => command.RemoveVars(var);
    
    public static DockerCustom operator +(DockerCustom command, IEnumerable<(string name, string value)> vars) => command.AddVars(vars);
    
    public static DockerCustom operator -(DockerCustom command, IEnumerable<(string name, string value)> vars) => command.RemoveVars(vars);
}

public partial record DockerRun: ICommandLine
{
    public static DockerRun operator +(DockerRun command, string arg) => command.AddArgs(arg);
    
    public static DockerRun operator -(DockerRun command, string arg) => command.RemoveArgs(arg);
    
    public static DockerRun operator +(DockerRun command, IEnumerable<string> args) => command.AddArgs(args);

    public static DockerRun operator -(DockerRun command, IEnumerable<string> args) => command.RemoveArgs(args);
    
    public static DockerRun operator +(DockerRun command, (string name, string value) var) => command.AddVars(var);
    
    public static DockerRun operator -(DockerRun command, (string name, string value) var) => command.RemoveVars(var);
    
    public static DockerRun operator +(DockerRun command, IEnumerable<(string name, string value)> vars) => command.AddVars(vars);
    
    public static DockerRun operator -(DockerRun command, IEnumerable<(string name, string value)> vars) => command.RemoveVars(vars);
}

