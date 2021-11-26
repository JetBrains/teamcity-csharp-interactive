namespace TeamCity.CSharpInteractive
{
    using System;
    using System.Diagnostics;
    using Cmd;

    internal interface IProcess: IDisposable
    {
        event Action<CommandLineOutput> OnOutput;
        
        event Action OnExit;

        int Id { get; }

        int ExitCode { get; }

        bool Start(CommandLine commandLine, out ProcessStartInfo startInfo);

        void WaitForExit();
        
        bool WaitForExit(TimeSpan timeout);
        
        void Kill();
    }
}