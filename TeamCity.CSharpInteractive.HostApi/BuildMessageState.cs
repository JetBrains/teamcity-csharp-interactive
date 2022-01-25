// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
namespace HostApi;

public enum BuildMessageState
{
    ServiceMessage,
    StdOut,
    Warning,
    StdError,
    Failure,
    BuildProblem
}