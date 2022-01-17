// ReSharper disable CheckNamespace
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable NotAccessedPositionalProperty.Global
// ReSharper disable ReturnTypeCanBeEnumerable.Local
namespace Dotnet
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using Cmd;

    [Immutype.Target]
    [DebuggerTypeProxy(typeof(BuildResultDebugView))]
    public record BuildResult(
        BuildState State,
        IStatisticsCalculator StatisticsCalculator,
        IReadOnlyList<BuildMessage> Errors,
        IReadOnlyList<BuildMessage> Warnings,
        IReadOnlyList<TestResult> Tests,
        IReadOnlyList<CommandLineResult> CommandLines,
        IReadOnlyList<BuildResult> Results)
    {
        private readonly object _lockObject = new();
        private BuildStatistics? _summary;
        private BuildStatistics? _totals;
        private readonly BuildState _state = State;

        internal static readonly BuildResult Succeeded = new(BuildState.Succeeded);
        internal static readonly BuildResult Failed = new(BuildState.Failed);
        internal static readonly BuildResult Canceled = new(BuildState.Canceled);

        public BuildResult(BuildState state)
            : this(state, EmptyStatisticsCalculator.Shared, Array.Empty<BuildMessage>(), Array.Empty<BuildMessage>(), Array.Empty<TestResult>(), Array.Empty<CommandLineResult>(), Array.Empty<BuildResult>())
        { }

        public BuildStatistics Summary
        {
            get
            {
                lock (_lockObject)
                {
                    _summary ??= StatisticsCalculator.Calculate(new []{ this });
                    return _summary;
                }
            }
        }
        
        public BuildStatistics Totals
        {
            get
            {
                lock (_lockObject)
                {
                    _totals ??= StatisticsCalculator.Calculate(GetResults(this).ToArray()).WithName("total");
                    return _totals;
                }
            }
        }

        public BuildState State
        {
            get => Results.Select(i => i.State).Concat(new [] { _state }).Max();
            init => _state = value;
        }

        public static implicit operator string(BuildResult it) => it.ToString();

        public static implicit operator BuildResult(BuildResult[] results)
        {
            var actualResults = results.Distinct().ToArray();
            switch (actualResults.Length)
            {
                case 0:
                    return Succeeded;
            
                case 1:
                    return actualResults[0];
            
                default:
                    var state = actualResults
                        .Select(i => i.State)
                        .DefaultIfEmpty(BuildState.Succeeded).Max();

                    var statisticsCalculator = actualResults
                        .Select(i => i.StatisticsCalculator)
                        .FirstOrDefault(i => i != EmptyStatisticsCalculator.Shared) ?? EmptyStatisticsCalculator.Shared;

                    return new BuildResult(state)
                        .WithResults(actualResults)
                        .WithStatisticsCalculator(statisticsCalculator);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            var statistics = string.Empty;
            var commandLines = CommandLines.Distinct().ToArray();
            switch (commandLines.Length)
            {
                case 0:
                    sb.Append("This build");
                    break;
                
                case 1:
                    sb.Append('"');
                    sb.Append(commandLines[0].StartInfo.ShortName);
                    sb.Append('"');
                    break;
                
                default:
                    sb.Append(string.Join(", ", commandLines.Select(i => $"\"{i.StartInfo.ShortName}\"")));
                    break;
            }

            var buildsCount = GetResults(this).Count() - 1;
            switch (buildsCount)
            {
                case 0:
                    sb.Append(commandLines.Length > 1 ? " are " : " is ");
                    break;
                
                default:
                    sb.Append(", including ");
                    sb.Append(buildsCount);
                    sb.Append(" build");
                    sb.Append(buildsCount > 1 ? "s before are " : " before are ");
                    break;
            }
            
            // ReSharper disable once ConstantConditionalAccessQualifier
            if (Summary?.IsEmpty != true)
            {
                statistics = " with " + Summary;
            }

            // ReSharper disable once ConstantConditionalAccessQualifier
            if (Results.Count > 0 && Totals?.IsEmpty != true)
            {
                if (Summary?.IsEmpty != true)
                {
                    statistics += " and";
                }

                statistics = statistics + " with " + Totals;
            }

            sb.Append(State.ToString().ToLowerInvariant());
            sb.Append(statistics);
            sb.Append('.');
            return sb.ToString();
        }
        
        private static IEnumerable<BuildResult> GetResults(BuildResult result) =>
            result.Results.SelectMany(GetResults).Concat(new [] { result }).Distinct();

        private class BuildResultDebugView
        {
            private readonly BuildResult _result;

            public BuildResultDebugView(BuildResult result) => _result = result;
            
            public BuildState State => _result.State;

            [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
            public IReadOnlyList<BuildResult> Results => _result.Results;
            
            public BuildStatistics Summary => _result.Summary;

            public BuildStatistics Totals => _result.Totals;

            [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
            public IReadOnlyList<BuildMessage> Errors => _result.Errors;
            
            [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
            public IReadOnlyList<BuildMessage> Warnings => _result.Warnings;

            [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
            public IReadOnlyList<TestResult> Tests => _result.Tests;

            [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
            public IReadOnlyList<CommandLineResult> CommandLines => _result.CommandLines;
        }
    }
}