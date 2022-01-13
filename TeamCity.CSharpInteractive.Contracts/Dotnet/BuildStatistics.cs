// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable CheckNamespace
namespace Dotnet;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cmd;

public record BuildStatistics
{
    private readonly BuildResult _buildResult;

    public BuildStatistics(BuildResult buildResult)
    {
        _buildResult = buildResult;
        Errors = buildResult.Messages.Count(i => i.State == BuildMessageState.Error);
        Warnings = buildResult.Messages.Count(i => i.State == BuildMessageState.Warning);

        var tests = (
                from testGroup in 
                    from testResult in buildResult.Tests
                    group testResult by (testResult.AssemblyName, testResult.DisplayName)
                select testGroup.OrderBy(i => i.State).Last())
            .ToArray();

        Tests = tests.Length;
        FailedTests = tests.Count(i => i.State == TestState.Failed);
        IgnoredTests = tests.Count(i => i.State == TestState.Ignored);
        PassedTests = tests.Count(i => i.State == TestState.Passed);
        
        Success =
            buildResult.State == ProcessState.Success || buildResult.State == ProcessState.Unknown
            && FailedTests == 0
            && buildResult.Messages.All(i => i.State <= BuildMessageState.Warning);
    }

    public bool Success { get; }

    public int Errors { get; }

    public int Warnings { get; }

    public int Tests { get; }

    public int FailedTests { get; }

    public int IgnoredTests { get; }

    public int PassedTests { get; }

    public override string ToString()
    {
        var sb = new StringBuilder(Success ? "Build succeeded" : "Build failed");
        foreach (var reason in FormatReasons(GetReasons().ToArray()))
        {
            sb.Append(reason);
        }

        sb.Append('.');
        return sb.ToString();
    }

    private IEnumerable<string> GetReasons()
    {
        if (Errors > 0)
        {
            yield return $"{Errors} errors";
        }

        if (Warnings > 0)
        {
            yield return $"{Errors} warnings";
        }

        if (FailedTests > 0)
        {
            yield return $"{FailedTests} failed";
        }

        if (IgnoredTests > 0)
        {
            yield return $"{IgnoredTests} ignored";
        }

        if (PassedTests > 0)
        {
            yield return $"{PassedTests} passed";
        }
        
        if (Tests > 0)
        {
            yield return $"{Tests} total tests";
        }

        if (_buildResult.ExitCode != 0)
        {
            yield return $"exit code {_buildResult.ExitCode}";
        }
    }

    private static IEnumerable<string> FormatReasons(IReadOnlyList<string> reasons)
    {
        for (var i = 0; i < reasons.Count; i++)
        {
            if (i == 0)
            {
                yield return " with ";
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
}