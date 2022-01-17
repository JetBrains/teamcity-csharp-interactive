// ReSharper disable PublicConstructorInAbstractClass
// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Cmd;
    using Dotnet;
    using JetBrains.TeamCity.ServiceMessages;
    using Pure.DI;

    internal class ReliableBuildResult: IBuildResult
    {
        private readonly ITeamCitySettings _teamCitySettings;
        private readonly IFileSystem _fileSystem;
        private readonly IMessagesReader _messagesReader;
        private readonly IBuildResult _baseBuildResult;
        private readonly Dictionary<string, ProcessInfo> _sources = new();

        public ReliableBuildResult(
            ITeamCitySettings teamCitySettings,
            IFileSystem fileSystem,
            IMessagesReader messagesReader,
            [Tag("base")] IBuildResult baseBuildResult)
        {
            _teamCitySettings = teamCitySettings;
            _fileSystem = fileSystem;
            _messagesReader = messagesReader;
            _baseBuildResult = baseBuildResult;
        }

        public IReadOnlyList<BuildMessage> ProcessMessage(IStartInfo startInfo, int processId, IServiceMessage message)
        {
            var source = message.GetValue("source");
            if (string.IsNullOrWhiteSpace(source))
            {
                return _baseBuildResult.ProcessMessage(startInfo, processId, message);
            }

            _sources.TryAdd(source, new ProcessInfo(startInfo, processId));
            return Array.Empty<BuildMessage>();
        }

        public Dotnet.BuildResult Create(IStartInfo startInfo, ProcessState state, int? exitCode)
        {
            var items = 
                from source in _sources
                let indicesFile = Path.Combine(_teamCitySettings.ServiceMessagesPath, source.Key)
                where _fileSystem.IsFileExist(indicesFile)
                let messagesFile = indicesFile + ".msg"
                where _fileSystem.IsFileExist(messagesFile)
                from message in _messagesReader.Read(indicesFile, messagesFile)
                select (message, startInfoFactory: source.Value);

            // ReSharper disable once UseDeconstruction
            foreach (var (message, processInfo) in items)
            {
                _baseBuildResult.ProcessMessage(processInfo.StartInfo, processInfo.ProcessId, message);
            }
            
            return _baseBuildResult.Create(startInfo, state, exitCode);
        }

        private record ProcessInfo(IStartInfo StartInfo, int ProcessId);
    }
}