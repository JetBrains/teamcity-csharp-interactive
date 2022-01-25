// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using CSharpInteractive;
using HostApi;
using Pure.DI;

internal class BuildMessageLogWriter : IBuildMessageLogWriter
{
    private readonly ILog<BuildMessageLogWriter> _log;
    private readonly IStdOut _stdOut;
    private readonly IStdErr _stdErr;

    public BuildMessageLogWriter(
        ILog<BuildMessageLogWriter> log,
        [Tag("Default")] IStdOut stdOut,
        [Tag("Default")] IStdErr stdErr)
    {
        _log = log;
        _stdOut = stdOut;
        _stdErr = stdErr;
    }

    public void Write(BuildMessage message)
    {
        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (message.State)
        {
            case BuildMessageState.StdOut:
                _stdOut.WriteLine(new []{ new Text(message.Text)});
                break;

            case BuildMessageState.StdError:
                _stdErr.WriteLine(new []{new Text(message.Text)});
                break;

            case BuildMessageState.Warning:
                _log.Warning(message.Text);
                break;
                
            case BuildMessageState.Failure:
            case BuildMessageState.BuildProblem:
                _log.Error(ErrorId.Build, message.Text);
                break;
        }
    }
}