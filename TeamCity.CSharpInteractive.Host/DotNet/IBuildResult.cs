// ReSharper disable UnusedMemberInSuper.Global
namespace Script.DotNet;

using Cmd;

public interface IBuildResult
{
    IStartInfo StartInfo { get; }
    
    IReadOnlyList<BuildMessage> Errors { get; }
    
    IReadOnlyList<BuildMessage> Warnings { get; }
    
    IReadOnlyList<TestResult> Tests { get; }
    
    int? ExitCode { get; }
    
    BuildStatistics Summary { get; }
}