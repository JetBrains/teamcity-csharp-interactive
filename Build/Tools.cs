using HostApi;

static class Property
{
    public static string Get(string name, string defaultProp, bool showWarning = false)
    {
        if (Props.TryGetValue(name, out var prop) && !string.IsNullOrWhiteSpace(prop))
        {
            return prop;
        }

        var message = $"The property \"{name}\" was not defined, the default value \"{defaultProp}\" was used.";
        if (showWarning)
        {
            Warning(message);
        }
        else
        {
            Info(message);
        }

        return defaultProp;
    }
}

static class Assertion
{
    private static bool CheckBuildResult(IBuildResult result)
    {
        if (result.ExitCode == 0)
        {
            return true;
        }

        foreach (var failedTest in
                 from testResult in result.Tests
                 where testResult.State == TestState.Failed
                 select testResult.ToString())
        {
            Error(failedTest);
        }

        Error($"{result.StartInfo.ShortName} failed");
        return false;
    }

    public static void Succeed(IBuildResult result)
    {
        if (!CheckBuildResult(result))
        {
            Environment.Exit(1);
        }
    }

    public static void Succeed(IEnumerable<IBuildResult> resultsTask)
    {
        if (!resultsTask.All(CheckBuildResult))
        {
            Environment.Exit(1);
        }
    }
    
    public static void Succeed(int? exitCode, string shortName)
    {
        if (exitCode == 0)
        {
            return;
        }

        Error($"{shortName} failed.");
        Environment.Exit(1);
    }
}