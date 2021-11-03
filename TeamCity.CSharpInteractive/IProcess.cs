namespace TeamCity.CSharpInteractive
{
    using System;
    using Contracts;

    internal interface IProcess: IDisposable
    {
        event Action<CommandLineOutput> OnOutput;
        
        event Action OnExit;

        int Id { get; }

        int ExitCode { get; }

        bool Start(CommandLine commandLine);

        void WaitForExit();
        
        bool WaitForExit(TimeSpan timeout);
        
        void Kill();
    }
}