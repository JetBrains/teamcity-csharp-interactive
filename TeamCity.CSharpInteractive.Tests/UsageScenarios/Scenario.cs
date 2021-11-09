namespace TeamCity.CSharpInteractive.Tests.UsageScenarios
{
    public class Scenario
    {
        protected static T GetService<T>() => Composer.Resolve<T>();
    }
}