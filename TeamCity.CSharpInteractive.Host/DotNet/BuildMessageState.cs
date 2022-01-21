// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
namespace DotNet;

public enum BuildMessageState
{
    ServiceMessage,
    StdOut,
    Warning,
    StdError,
    Failure,
    BuildProblem
}