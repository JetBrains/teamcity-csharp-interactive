namespace TeamCity.CSharpInteractive
{
    using System.Diagnostics;

    internal interface IStartInfoFactory
    {
        ProcessStartInfo Create(Contracts.CommandLine commandLine);
    }
}