// ReSharper disable InconsistentNaming
namespace TeamCity.CSharpInteractive
{
    internal enum SpecialFolder
    {
        // Directory with executable file
        Bin,

        // Temp directory
        Temp,

        // "Program Files" on Windows and "usr/local/share" on others
        ProgramFiles,

        // A directory of current executing script
        Script,
        
        // Working directory of process
        Working,
        
        // Path to MSBuild logger
        MSBuildLoggerPath,
        
        // Path to VSTest logger
        VSTestLoggerDirectory
    }
}