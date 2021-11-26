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
        
        // MSBuild logger directory
        MSBuildLoggerDirectory,
        
        // VSTest logger directory
        VSTestLoggerDirectory
    }
}