// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using System.Diagnostics;
using HostApi;
using JetBrains.TeamCity.ServiceMessages.Write.Special;

internal class ProcessInFlowRunner : IProcessRunner
{
    private readonly IProcessRunner _baseProcessRunner;
    private readonly ICISettings _ciSettings;
    private readonly ITeamCityWriter _teamCityWriter;
    private readonly IFlowContext _flowContext;

    public ProcessInFlowRunner(
        [Tag("base")] IProcessRunner baseProcessRunner,
        ICISettings ciSettings,
        ITeamCityWriter teamCityWriter,
        IFlowContext flowContext)
    {
        _baseProcessRunner = baseProcessRunner;
        _ciSettings = ciSettings;
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
        _ciSettings.CIType == CIType.TeamCity
            ? new StartInfoInFlow(startInfo, _flowContext.CurrentFlowId)
            : startInfo;

    private IDisposable CreateFlow() =>
        _ciSettings.CIType == CIType.TeamCity ? _teamCityWriter.OpenFlow() : Disposable.Empty;

    [DebuggerTypeProxy(typeof(CommandLine.CommandLineDebugView))]
    private class StartInfoInFlow : IStartInfo
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
            new[] {(FlowIdEnvironmentVariableName: CISettings.TeamCityFlowIdEnvironmentVariableName, _flowId)}
                .Concat(_baseStartIfo.Vars);

        public override string? ToString() => _baseStartIfo.ToString();
    }
}