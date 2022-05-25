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
            Composer.Resolve<IDotNetEnvironment>().Path,
            Path.Combine(Path.GetDirectoryName(typeof(DotNetScript).Assembly.Location)!, "dotnet-csi.dll"));

    public static CommandLine Create(string scriptName, string? workingDirectory, IEnumerable<string> args, params string[] lines)
    {
        workingDirectory ??= GetWorkingDirectory();
        var scriptFile = Path.Combine(workingDirectory, scriptName);
        TestComposer.ResolveIFileSystem().AppendAllLines(scriptFile, lines);
        return Create().AddArgs(args.ToArray()).AddArgs(scriptFile).WithWorkingDirectory(workingDirectory);
    }

    public static string GetWorkingDirectory()
    {
        var uniqueNameGenerator = Composer.Resolve<IUniqueNameGenerator>();
        var environment = Composer.Resolve<IEnvironment>();
        var tempDirectory = environment.GetPath(SpecialFolder.Temp);
        return Path.Combine(tempDirectory, uniqueNameGenerator.Generate());
    }
}