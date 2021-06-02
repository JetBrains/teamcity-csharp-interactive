// ReSharper disable UnusedMemberInSuper.Global
namespace Teamcity.CSharpInteractive
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