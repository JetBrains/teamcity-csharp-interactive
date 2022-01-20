// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
namespace DotNet;

public enum BuildMessageState
{
    ServiceMessage,
    StdOut,
    StdErr,
    Warning,
    Failure,
    Error,
    BuildProblem
}