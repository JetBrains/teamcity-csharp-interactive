// ReSharper disable UnusedMemberInSuper.Global
namespace TeamCity.CSharpInteractive;

using Microsoft.DotNet.PlatformAbstractions;

internal interface IEnvironment
{
    Platform OperatingSystemPlatform { get; }

    string ProcessArchitecture { get; }

    IEnumerable<string> GetCommandLineArgs();

    string GetPath(SpecialFolder specialFolder);

    void Exit(int exitCode);
}