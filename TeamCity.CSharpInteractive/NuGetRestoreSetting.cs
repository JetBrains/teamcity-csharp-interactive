namespace TeamCity.CSharpInteractive
{
    internal enum NuGetRestoreSetting
    {
        Default,

        Parallel,
        NonParallel,

        IgnoreFailedSources,
        ConsiderFailedSources,
        
        HideWarningsAndErrors,
        ShowWarningsAndErrors,
        
        NoCache,
        WithCache
    }
}