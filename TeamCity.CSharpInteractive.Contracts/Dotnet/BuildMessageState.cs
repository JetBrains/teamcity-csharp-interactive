// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
namespace Dotnet
{
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
}