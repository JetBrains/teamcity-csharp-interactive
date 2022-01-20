// ReSharper disable UnusedMemberInSuper.Global
namespace TeamCity.CSharpInteractive;

internal interface IDotNetEnvironment
{
    string Path { get; }
        
    string TargetFrameworkMoniker { get; }
}