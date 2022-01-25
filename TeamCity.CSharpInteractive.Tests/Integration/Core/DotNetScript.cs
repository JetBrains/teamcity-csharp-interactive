namespace TeamCity.CSharpInteractive.Tests.Integration.Core;

using HostApi;

internal static class DotNetScript
{
    public static CommandLine Create(IEnumerable<string> args, params string[] lines) =>
        Create("script.csx", default, args, lines);

    public static CommandLine Create(params string[] lines) =>
        Create(Enumerable.Empty<string>(), lines);
        
    public static CommandLine Create() =>
        new(
            TeamCity.CSharpInteractive.Composer.Resolve<IDotNetEnvironment>().Path,
            Path.Combine(Path.GetDirectoryName(typeof(DotNetScript).Assembly.Location)!, "dotnet-csi.dll"));
        
    public static CommandLine Create(string scriptName, string? workingDirectory, IEnumerable<string> args, params string[] lines)
    {
        workingDirectory ??= GetWorkingDirectory();
        var scriptFile = Path.Combine(workingDirectory, scriptName);
        Composer.ResolveIFileSystem().AppendAllLines(scriptFile, lines);
        return Create().AddArgs(args.ToArray()).AddArgs(scriptFile).WithWorkingDirectory(workingDirectory);
    }

    public static string GetWorkingDirectory()
    {
        var uniqueNameGenerator = TeamCity.CSharpInteractive.Composer.Resolve<IUniqueNameGenerator>();
        var environment = TeamCity.CSharpInteractive.Composer.Resolve<IEnvironment>();
        var tempDirectory = environment.GetPath(SpecialFolder.Temp);
        return Path.Combine(tempDirectory, uniqueNameGenerator.Generate());
    }
}