namespace TeamCity.CSharpInteractive
{
    using System;
    using System.Diagnostics;
    using Cmd;

    internal interface IProcessManager: IDisposable
    {
        event Action<Output> OnOutput;
        
        event Action OnExit;

        int Id { get; }

        int ExitCode { get; }

        bool Start(IStartInfo info, out ProcessStartInfo startInfo);

        void WaitForExit();
        
        bool WaitForExit(TimeSpan timeout);
        
        void Kill();
    }
}