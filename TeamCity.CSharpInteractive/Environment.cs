// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using System.Diagnostics.CodeAnalysis;
using Microsoft.DotNet.PlatformAbstractions;

[ExcludeFromCodeCoverage]
internal class Environment : IEnvironment, ITraceSource, IScriptContext
{
    private readonly LinkedList<string> _scriptDirectories = new();

    public Platform OperatingSystemPlatform => RuntimeEnvironment.OperatingSystemPlatform;

    public string ProcessArchitecture => RuntimeEnvironment.RuntimeArchitecture;

    public IEnumerable<string> GetCommandLineArgs() => System.Environment.GetCommandLineArgs();

    public string GetPath(SpecialFolder specialFolder)
    {
        switch (OperatingSystemPlatform)
        {
            case Platform.Windows:
                return specialFolder switch
                {
                    SpecialFolder.Bin => GetBinDirectory(),
                    SpecialFolder.Temp => System.Environment.GetEnvironmentVariable("TMP") ?? ".",
                    SpecialFolder.ProgramFiles => System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles),
                    SpecialFolder.Script => GetScriptDirectory(),
                    SpecialFolder.Working => GetWorkingDirectory(),
                    _ => throw new ArgumentOutOfRangeException(nameof(specialFolder), specialFolder, null)
                };

            case Platform.Unknown:
            case Platform.Linux:
            case Platform.Darwin:
            case Platform.FreeBSD:
                return specialFolder switch
                {
                    SpecialFolder.Bin => GetBinDirectory(),
                    SpecialFolder.Temp => System.Environment.GetEnvironmentVariable("TMP") ?? ".",
                    SpecialFolder.ProgramFiles => "usr/local/share",
                    SpecialFolder.Script => GetScriptDirectory(),
                    SpecialFolder.Working => GetWorkingDirectory(),
                    _ => throw new ArgumentOutOfRangeException(nameof(specialFolder), specialFolder, null)
                };

            default:
                throw new ArgumentOutOfRangeException(nameof(specialFolder));
        }
    }

    public void Exit(int exitCode) => System.Environment.Exit(exitCode);

    public IEnumerable<Text> Trace
    {
        get
        {
            yield return new Text($"OperatingSystemPlatform: {OperatingSystemPlatform}");
            yield return Text.NewLine;
            yield return new Text($"ProcessArchitecture: {ProcessArchitecture}");
            yield return Text.NewLine;
            foreach (var specialFolder in Enum.GetValues(typeof(SpecialFolder)).OfType<SpecialFolder>())
            {
                yield return new Text($"Path({specialFolder}): {GetPath(specialFolder)}");
                yield return Text.NewLine;
            }

            yield return new Text("Command line arguments:");
            yield return Text.NewLine;
            foreach (var arg in System.Environment.GetCommandLineArgs())
            {
                yield return Text.Tab;
                yield return new Text(arg);
                yield return Text.NewLine;
            }
        }
    }

    public IDisposable OverrideScriptDirectory(string? scriptDirectory)
    {
        if (scriptDirectory == null)
        {
            return Disposable.Empty;
        }

        _scriptDirectories.AddLast(scriptDirectory);
        return Disposable.Create(() => _scriptDirectories.Remove(scriptDirectory));
    }

    private static string GetWorkingDirectory() => Directory.GetCurrentDirectory();

    private string GetBinDirectory() => Path.GetDirectoryName(typeof(Environment).Assembly.Location) ?? GetScriptDirectory();

    private string GetScriptDirectory() => _scriptDirectories.Count > 0 ? _scriptDirectories.Last!.Value : GetWorkingDirectory();
}