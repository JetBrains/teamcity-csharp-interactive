// ReSharper disable CheckNamespace
namespace Dotnet
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;

    [Immutype.Target]
    [DebuggerTypeProxy(typeof(BuildStatisticsDebugView))]
    public record BuildStatistics(string Name = "", int Errors = default, int Warnings = default, int Tests = default, int FailedTests = default, int IgnoredTests = default, int PassedTests = default)
    {
        internal static readonly BuildStatistics Empty = new();

        public bool IsEmpty =>
            Errors == 0
                && Warnings == 0
                && Tests == 0
                && FailedTests == 0
                && IgnoredTests == 0
                && PassedTests == 0;

        public override string ToString()
        {
            if (IsEmpty)
            {
                return "Empty";
            }
            
            var sb = new StringBuilder();
            foreach (var reason in FormatReasons(GetReasons(this, Name).ToArray()))
            {
                sb.Append(reason);
            }

            return sb.ToString();
        }

        private static IEnumerable<string> FormatReasons(IReadOnlyList<string> reasons)
        {
            for (var i = 0; i < reasons.Count; i++)
            {
                if (i == 0)
                {
                    yield return reasons[i];
                    continue;
                }

                if (i == reasons.Count - 1)
                {
                    yield return " and ";
                    yield return reasons[i];
                    continue;
                }

                yield return ", ";
                yield return reasons[i];
            }
        }
        
        private static IEnumerable<string> GetReasons(BuildStatistics statistics, string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                name += ' ';
            }

            if (statistics.Errors > 0)
            {
                yield return $"{statistics.Errors} {name}{GetName("error", statistics.Errors)}";
            }

            if (statistics.Warnings > 0)
            {
                yield return $"{statistics.Warnings} {name}{GetName("warning", statistics.Warnings)}";
            }

            if (statistics.FailedTests > 0)
            {
                yield return $"{statistics.FailedTests} {name}failed";
            }

            if (statistics.IgnoredTests > 0)
            {
                yield return $"{statistics.IgnoredTests} {name}ignored";
            }

            if (statistics.PassedTests > 0)
            {
                yield return $"{statistics.PassedTests} {name}passed";
            }
                
            if (statistics.Tests > 0)
            {
                yield return $"{statistics.Tests} {name}{GetName("test", statistics.Tests)}";
            }
        }

        private static string GetName(string baseName, int count) =>
            count switch
            {
                0 => $"no {baseName}s",
                1 => baseName,
                _ => baseName + 's'
            };

        private class BuildStatisticsDebugView
        {
            private readonly BuildStatistics _statistics;

            public BuildStatisticsDebugView(BuildStatistics statistics) => _statistics = statistics;

            public int Errors => _statistics.Errors;
            
            public int Warnings => _statistics.Warnings;
            
            public int FailedTests => _statistics.FailedTests;
            
            public int IgnoredTests => _statistics.IgnoredTests;
            
            public int PassedTests => _statistics.PassedTests;
            
            public int Tests => _statistics.Tests;
        }
    }
}