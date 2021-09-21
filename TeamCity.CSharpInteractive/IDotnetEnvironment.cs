// ReSharper disable UnusedMemberInSuper.Global
namespace TeamCity.CSharpInteractive
{
    internal interface IDotnetEnvironment
    {
        string Path { get; }
        
        string TargetFrameworkMoniker { get; }
    }
}