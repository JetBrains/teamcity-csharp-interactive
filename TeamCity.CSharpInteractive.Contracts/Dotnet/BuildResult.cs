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
        IStartInfo StartInfo,
        IReadOnlyList<BuildMessage> Errors,
        IReadOnlyList<BuildMessage> Warnings,
        IReadOnlyList<TestResult> Tests,
        int? ExitCode)
    {
        private readonly object _lockObject = new();
        private BuildStatistics? _summary;

        public BuildResult(IStartInfo startInfo)
            : this(startInfo, Array.Empty<BuildMessage>(), Array.Empty<BuildMessage>(), Array.Empty<TestResult>(), default)
        { }

        public BuildStatistics Summary
        {
            get
            {
                lock (_lockObject)
                {
                    _summary ??= Calculate();
                    return _summary;
                }
            }
        }

        public static implicit operator string(BuildResult it) => it.ToString();

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(string.IsNullOrWhiteSpace(StartInfo.ShortName) ? "Build" : $"\"{StartInfo.ShortName}\"");
            sb.Append(" is ");
            sb.Append(ExitCode.HasValue ? "finished" : "not finished");
            if (Summary.IsEmpty != true)
            {
                sb.Append(" with ");
                sb.Append(Summary);
            }

            sb.Append('.');
            return sb.ToString();
        }
        
        private BuildStatistics Calculate()
        {
            var testItems =
                from testGroup in
                    from testResult in Tests
                    group testResult by (testResult.AssemblyName, testResult.DisplayName)
                select testGroup.OrderByDescending(i => i.State).First();
        
            var totalTests = 0;
            var failedTests = 0;
            var ignoredTests = 0;
            var passedTests = 0;
            foreach (var test in testItems)
            {
                totalTests++;
                // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                switch (test.State)
                {
                    case TestState.Passed:
                        passedTests++;
                        break;

                    case TestState.Failed:
                        failedTests++;
                        break;

                    case TestState.Ignored:
                        ignoredTests++;
                        break;
                }
            }

            return new BuildStatistics(
                Errors.Count,
                Warnings.Count,
                totalTests,
                failedTests,
                ignoredTests,
                passedTests);
        }
        
        private class BuildResultDebugView
        {
            private readonly BuildResult _result;

            public BuildResultDebugView(BuildResult result) => _result = result;
            
            public BuildStatistics Summary => _result.Summary;

            [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
            public IReadOnlyList<BuildMessage> Errors => _result.Errors;
            
            [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
            public IReadOnlyList<BuildMessage> Warnings => _result.Warnings;

            [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
            public IReadOnlyList<TestResult> Tests => _result.Tests;

            public IStartInfo StartInfo => _result.StartInfo;
            
            public int? ExitCode => _result.ExitCode;
        }
    }
}