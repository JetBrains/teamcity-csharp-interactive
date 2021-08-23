namespace Teamcity.CSharpInteractive.Tests.Integration.Core
{
    internal interface IProcessFactory
    {
        IProcess Create(ProcessParameters parameters);
    }
}