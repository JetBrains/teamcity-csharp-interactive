// ReSharper disable ReturnTypeCanBeEnumerable.Local
namespace TeamCity.CSharpInteractive;

using System.Diagnostics;
using System.Text;
using Cmd;
using DotNet;

[Immutype.Target]
[DebuggerTypeProxy(typeof(BuildResultDebugView))]
internal class BuildResult : IResult
{
    private readonly Lazy<BuildStatistics> _summary;

    // ReSharper disable once UnusedMember.Global
    public BuildResult(IStartInfo startInfo)
        : this(startInfo, Array.Empty<BuildMessage>(), Array.Empty<BuildMessage>(), Array.Empty<TestResult>(), default)
    { }
    
    public BuildResult(IStartInfo startInfo,
        IReadOnlyList<BuildMessage> errors,
        IReadOnlyList<BuildMessage> warnings,
        IReadOnlyList<TestResult> tests,
        int? exitCode)
    {
        StartInfo = startInfo;
        Errors = errors;
        Warnings = warnings;
        Tests = tests;
        ExitCode = exitCode;
        _summary = new Lazy<BuildStatistics>(CalculateSummary, LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public BuildStatistics Summary => _summary.Value;

    public IStartInfo StartInfo { get; }
    
    public IReadOnlyList<BuildMessage> Errors { get; }
    
    public IReadOnlyList<BuildMessage> Warnings { get; }
    
    public IReadOnlyList<TestResult> Tests { get; }
    
    public int? ExitCode { get; }

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
        
    private BuildStatistics CalculateSummary()
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