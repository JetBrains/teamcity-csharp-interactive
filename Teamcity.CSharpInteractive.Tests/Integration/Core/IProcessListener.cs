namespace Teamcity.CSharpInteractive.Tests.Integration.Core
{
    using System.Diagnostics;

    internal interface IProcessListener
    {
        void OnStart(ProcessStartInfo startInfo);

        void OnStdOut(string? line);

        void OnStdErr(string? line);

        void OnExitCode(ExitCode exitCode);
    }
}
