// ReSharper disable UnusedMemberInSuper.Global
// ReSharper disable CheckNamespace
namespace DotNet;

using Cmd;

public interface IResult
{
    IStartInfo StartInfo { get; }
    
    IReadOnlyList<BuildMessage> Errors { get; }
    
    IReadOnlyList<BuildMessage> Warnings { get; }
    
    IReadOnlyList<TestResult> Tests { get; }
    
    int? ExitCode { get; }
    
    BuildStatistics Summary { get; }
}