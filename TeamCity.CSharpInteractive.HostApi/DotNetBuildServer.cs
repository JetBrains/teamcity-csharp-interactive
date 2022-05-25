// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
namespace HostApi;

public enum DotNetBuildServer
{
    /// <summary>
    /// MSBuild build server
    /// </summary>
    MSBuild,
    
    /// <summary>
    /// VB/C# compiler build server
    /// </summary>
    VbCsCompiler,
    
    /// <summary>
    /// Razor build server
    /// </summary>
    Razor
}