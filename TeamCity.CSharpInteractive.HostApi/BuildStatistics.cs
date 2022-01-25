namespace HostApi;

using System.Text;

[Immutype.Target]
public record BuildStatistics(
    int Errors = default,
    int Warnings = default,
    int Tests = default,
    int FailedTests = default,
    int IgnoredTests = default,
    int PassedTests = default)
{
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
        foreach (var reason in FormatReasons(GetReasons(this).ToArray()))
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
        
    private static IEnumerable<string> GetReasons(BuildStatistics statistics)
    {
        if (statistics.Errors > 0)
        {
            yield return $"{statistics.Errors} {GetName("error", statistics.Errors)}";
        }

        if (statistics.Warnings > 0)
        {
            yield return $"{statistics.Warnings} {GetName("warning", statistics.Warnings)}";
        }

        if (statistics.FailedTests > 0)
        {
            yield return $"{statistics.FailedTests} failed";
        }

        if (statistics.IgnoredTests > 0)
        {
            yield return $"{statistics.IgnoredTests} ignored";
        }

        if (statistics.PassedTests > 0)
        {
            yield return $"{statistics.PassedTests} passed";
        }
                
        if (statistics.Tests > 0)
        {
            yield return $"{statistics.Tests} total {GetName("test", statistics.Tests)}";
        }
    }

    private static string GetName(string baseName, int count) =>
        count switch
        {
            1 => baseName,
            _ => baseName + 's'
        };
}