// ReSharper disable UnusedMemberInSuper.Global
namespace TeamCity.CSharpInteractive
{
    using System;

    internal interface IDotnetEnvironment
    {
        string Path { get; }
        
        string TargetFrameworkMoniker { get; }
        
        string Tfm { get; }
        
        Version Version { get; }
    }
}