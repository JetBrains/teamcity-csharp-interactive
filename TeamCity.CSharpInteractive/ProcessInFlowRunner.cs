// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using System.Diagnostics;
using JetBrains.TeamCity.ServiceMessages.Write.Special;
using Pure.DI;
using Script.Cmd;

internal class ProcessInFlowRunner: IProcessRunner
{
    private readonly IProcessRunner _baseProcessRunner;
    private readonly ITeamCitySettings _teamCitySettings;
    private readonly ITeamCityWriter _teamCityWriter;
    private readonly IFlowContext _flowContext;

    public ProcessInFlowRunner(
        [Tag("base")] IProcessRunner baseProcessRunner,
        ITeamCitySettings teamCitySettings,
        ITeamCityWriter teamCityWriter,
        IFlowContext flowContext)
    {
        _baseProcessRunner = baseProcessRunner;
        _teamCitySettings = teamCitySettings;
        _teamCityWriter = teamCityWriter;
        _flowContext = flowContext;
    }
        
    public ProcessResult Run(ProcessInfo processInfo, TimeSpan timeout)
    {
        using var flow = CreateFlow();
        return _baseProcessRunner.Run(processInfo.WithStartInfo(WrapInFlow(processInfo.StartInfo)), timeout);
    }
        
    public Task<ProcessResult> RunAsync(ProcessInfo processInfo, CancellationToken cancellationToken)
    {
        var flow = CreateFlow();
        return _baseProcessRunner.RunAsync(processInfo.WithStartInfo(WrapInFlow(processInfo.StartInfo)), cancellationToken)
            .ContinueWith(
                task =>
                {
                    flow.Dispose();
                    return task.Result;
                }, cancellationToken);
    }

    private IStartInfo WrapInFlow(IStartInfo startInfo) =>
        _teamCitySettings.IsUnderTeamCity
            ? new StartInfoInFlow(startInfo, _flowContext.CurrentFlowId)
            : startInfo;
        
    private IDisposable CreateFlow() =>
        _teamCitySettings.IsUnderTeamCity ? _teamCityWriter.OpenFlow() : Disposable.Empty;

    [DebuggerTypeProxy(typeof(CommandLine.StartInfoDebugView))]
    private class StartInfoInFlow: IStartInfo
    {
        private readonly IStartInfo _baseStartIfo;
        private readonly string _flowId;

        public StartInfoInFlow(IStartInfo baseStartIfo, string flowId)
        {
            _baseStartIfo = baseStartIfo;
            _flowId = flowId;
        }

        public string ShortName => _baseStartIfo.ShortName;

        public string ExecutablePath => _baseStartIfo.ExecutablePath;

        public string WorkingDirectory => _baseStartIfo.WorkingDirectory;

        public IEnumerable<string> Args => _baseStartIfo.Args;

        public IEnumerable<(string name, string value)> Vars => 
            new []{ (TeamCitySettings.FlowIdEnvironmentVariableName, _flowId) }
                .Concat(_baseStartIfo.Vars);

        public override string? ToString() => _baseStartIfo.ToString();
    }
}