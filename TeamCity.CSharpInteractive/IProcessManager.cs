namespace TeamCity.CSharpInteractive;

using Script.Cmd;

internal interface IProcessManager: IDisposable
{
    event Action<Output> OnOutput;
        
    event Action OnExit;

    int Id { get; }

    int ExitCode { get; }

    bool Start(IStartInfo info);

    void WaitForExit();
        
    bool WaitForExit(TimeSpan timeout);
        
    bool Kill();
}