// ReSharper disable PublicConstructorInAbstractClass
// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Cmd;
    using Dotnet;
    using JetBrains.TeamCity.ServiceMessages;
    using JetBrains.TeamCity.ServiceMessages.Read;
    using Pure.DI;

    internal class ReliableBuildResult: IBuildResult
    {
        private readonly ITeamCitySettings _teamCitySettings;
        private readonly IFileSystem _fileSystem;
        private readonly IMessagesReader _messagesReader;
        private readonly IServiceMessageParser _serviceMessageParser;
        private readonly IBuildResult _baseBuildResult;
        private readonly Dictionary<string, IStartInfo> _sources = new();

        public ReliableBuildResult(
            ITeamCitySettings teamCitySettings,
            IFileSystem fileSystem,
            IMessagesReader messagesReader,
            IServiceMessageParser serviceMessageParser,
            [Tag("base")] IBuildResult baseBuildResult)
        {
            _teamCitySettings = teamCitySettings;
            _fileSystem = fileSystem;
            _messagesReader = messagesReader;
            _serviceMessageParser = serviceMessageParser;
            _baseBuildResult = baseBuildResult;
        }

        public IReadOnlyList<BuildMessage> ProcessMessage(IStartInfo startInfo, IServiceMessage message) =>
            ProcessMessageInternal(startInfo, message).ToArray();

        public Dotnet.BuildResult CreateResult(int? exitCode)
        {
            var items = 
                from source in _sources
                let indicesFile = Path.Combine(_teamCitySettings.ServiceMessagesPath, source.Key)
                let messagesFile = Path.Combine(_teamCitySettings.ServiceMessagesPath, source.Key + ".msg")
                where _fileSystem.IsFileExist(indicesFile) && _fileSystem.IsFileExist(messagesFile)
                from line in _messagesReader.Read(indicesFile, messagesFile)
                where !string.IsNullOrWhiteSpace(line)
                from message in _serviceMessageParser.ParseServiceMessages(line)
                select (line, message, StartInfoFactory: source.Value);

            foreach (var (line, message, startInfoFactory) in items)
            {
                _baseBuildResult.ProcessMessage(startInfoFactory, message);
            }
            
            return _baseBuildResult.CreateResult(exitCode);
        }

        private IEnumerable<BuildMessage> ProcessMessageInternal(IStartInfo startInfo, IServiceMessage message)
        {
            {
                var source = message.GetValue("source");
                if (string.IsNullOrWhiteSpace(source))
                {
                    foreach (var buildMessage in _baseBuildResult.ProcessMessage(startInfo, message))
                    {
                        yield return buildMessage;
                    }
                }
                else
                {
                    _sources.TryAdd(source, startInfo);
                    yield return new BuildMessage(BuildMessageState.ServiceMessage, message);
                }
            }
        }
    }
}