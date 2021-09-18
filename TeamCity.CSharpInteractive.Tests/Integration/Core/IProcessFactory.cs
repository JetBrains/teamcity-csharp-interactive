namespace TeamCity.CSharpInteractive.Tests.Integration.Core
{
    internal interface IProcessFactory
    {
        IProcess Create(ProcessParameters parameters);
    }
}