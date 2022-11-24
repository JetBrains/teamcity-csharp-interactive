// ReSharper disable PublicConstructorInAbstractClass
// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using HostApi;
using JetBrains.TeamCity.ServiceMessages;

internal class ReliableBuildContext : IBuildContext
{
    private readonly ICISettings _ciSettings;
    private readonly IFileSystem _fileSystem;
    private readonly IMessagesReader _messagesReader;
    private readonly IBuildContext _baseBuildContext;
    private readonly Dictionary<string, Output> _sources = new();

    public ReliableBuildContext(
        ICISettings ciSettings,
        IFileSystem fileSystem,
        IMessagesReader messagesReader,
        [Tag("base")] IBuildContext baseBuildContext)
    {
        _ciSettings = ciSettings;
        _fileSystem = fileSystem;
        _messagesReader = messagesReader;
        _baseBuildContext = baseBuildContext;
    }

    public IReadOnlyList<BuildMessage> ProcessMessage(in Output output, IServiceMessage message)
    {
        var source = message.GetValue("source");
        if (string.IsNullOrWhiteSpace(source))
        {
            return _baseBuildContext.ProcessMessage(output, message);
        }

        _sources.TryAdd(source, output.WithLine(string.Empty));
        return Array.Empty<BuildMessage>();
    }

    public IReadOnlyList<BuildMessage> ProcessOutput(in Output output) =>
        _baseBuildContext.ProcessOutput(output);

    public IBuildResult Create(IStartInfo startInfo, int? exitCode)
    {
        var items =
            from source in _sources
            let indicesFile = Path.Combine(_ciSettings.ServiceMessagesPath, source.Key)
            where _fileSystem.IsFileExist(indicesFile)
            let messagesFile = indicesFile + ".msg"
            where _fileSystem.IsFileExist(messagesFile)
            from message in _messagesReader.Read(indicesFile, messagesFile)
            select (message, startInfoFactory: source.Value);

        // ReSharper disable once UseDeconstruction
        foreach (var (message, output) in items)
        {
            _baseBuildContext.ProcessMessage(output, message);
        }

        return _baseBuildContext.Create(startInfo, exitCode);
    }
}