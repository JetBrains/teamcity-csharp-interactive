namespace Teamcity.CSharpInteractive
{
    internal interface IRunner
    {
        InteractionMode InteractionMode { get; }

        ExitCode Run();
    }
}