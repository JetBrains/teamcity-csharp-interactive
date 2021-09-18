namespace TeamCity.CSharpInteractive.Tests.Integration.Core
{
    using System.Diagnostics;

    internal interface IProcessListener
    {
        // ReSharper disable once UnusedParameter.Global
        void OnStart(ProcessStartInfo startInfo);

        void OnStdOut(string? line);

        void OnStdErr(string? line);

        void OnExitCode(ExitCode exitCode);
    }
}
