// ReSharper disable CheckNamespace
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
namespace Dotnet
{
    using System.Collections.Generic;
    using Cmd;

    [Immutype.Target]
    public record BuildResult(
        IReadOnlyList<BuildMessage> Messages,
        IReadOnlyList<TestResult> Tests,
        int? ExitCode = default,
        ProcessState State = ProcessState.Unknown)
    {
        private readonly object _lockObject = new();
        private BuildStatistics? _statistics;
        
        public BuildStatistics Statistics
        {
            get
            {
                lock (_lockObject)
                {
                    return _statistics ??= new BuildStatistics(this);
                }
            }
        }

        public bool Success => Statistics.Success;

        public override string ToString() => Statistics.ToString();

        public static implicit operator string(in BuildResult it) => it.ToString();
    }
}