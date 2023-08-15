// ReSharper disable PublicConstructorInAbstractClass
// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using HostApi;
using JetBrains.TeamCity.ServiceMessages;

internal class ReliableBuildContext : IBuildContext
{
    private readonly ITeamCitySettings _teamCitySettings;
    private readonly IFileSystem _fileSystem;
    private readonly IMessagesReader _messagesReader;
    private readonly IBuildContext _baseBuildContext;
    private readonly Dictionary<string, (IStartInfo, int)> _sources = new();

    public ReliableBuildContext(
        ITeamCitySettings teamCitySettings,
        IFileSystem fileSystem,
        IMessagesReader messagesReader,
        [Tag("base")] IBuildContext baseBuildContext)
    {
        _teamCitySettings = teamCitySettings;
        _fileSystem = fileSystem;
        _messagesReader = messagesReader;
        _baseBuildContext = baseBuildContext;
    }

    public IReadOnlyList<BuildMessage> ProcessMessage(in Output output, IServiceMessage message)
    {
        return ProcessMessage(output.StartInfo, output.ProcessId, message);
    }

    public IReadOnlyList<BuildMessage> ProcessMessage(IStartInfo startInfo, int processId, IServiceMessage message)
    {
        var source = message.GetValue("source");
        if (string.IsNullOrWhiteSpace(source))
        {
            return _baseBuildContext.ProcessMessage(startInfo, processId, message);
        }

        _sources.TryAdd(source, (startInfo, processId));
        return Array.Empty<BuildMessage>();
    }

    public IReadOnlyList<BuildMessage> ProcessOutput(in Output output) =>
        _baseBuildContext.ProcessOutput(output);

    public IBuildResult Create(IStartInfo startInfo, int? exitCode)
    {
        var fallbackToStdOutTestReporting =
            string.Equals(_teamCitySettings.FallbackToStdOutTestReportingEnvValue, "true", StringComparison.InvariantCultureIgnoreCase);

        if (fallbackToStdOutTestReporting == false || _teamCitySettings.ServiceMessagesBackupPathEnvValue == null)
        {
            return _baseBuildContext.Create(startInfo, exitCode);
        }

        var items =
            from source in _sources
            let indicesFile = Path.Combine(_teamCitySettings.ServiceMessagesBackupPathEnvValue, source.Key)
            where _fileSystem.IsFileExist(indicesFile)
            let messagesFile = indicesFile + ".msg"
            where _fileSystem.IsFileExist(messagesFile)
            from message in _messagesReader.Read(indicesFile, messagesFile)
            select (message, outputStartInfo: source.Value.Item1, outputProcessId: source.Value.Item2);

        foreach (var (message, outputStartInfo, outputProcessId) in items)
        {
            _baseBuildContext.ProcessMessage(outputStartInfo, outputProcessId, message);
        }

        return _baseBuildContext.Create(startInfo, exitCode);
    }
}
