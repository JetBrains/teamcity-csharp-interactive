// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
namespace Script.DotNet;

public enum BuildMessageState
{
    ServiceMessage,
    StdOut,
    Warning,
    StdError,
    Failure,
    BuildProblem
}