// ReSharper disable PublicConstructorInAbstractClass
// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Cmd;
    using DotNet;
    using JetBrains.TeamCity.ServiceMessages;
    using Pure.DI;

    internal class ReliableBuildResult: IBuildResult
    {
        private readonly ITeamCitySettings _teamCitySettings;
        private readonly IFileSystem _fileSystem;
        private readonly IMessagesReader _messagesReader;
        private readonly IBuildResult _baseBuildResult;
        private readonly Dictionary<string, Output> _sources = new();

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

        public IReadOnlyList<BuildMessage> ProcessMessage(in Output output, IServiceMessage message)
        {
            var source = message.GetValue("source");
            if (string.IsNullOrWhiteSpace(source))
            {
                return _baseBuildResult.ProcessMessage(output, message);
            }

            _sources.TryAdd(source, output.WithLine(string.Empty));
            return Array.Empty<BuildMessage>();
        }

        public IReadOnlyList<BuildMessage> ProcessOutput(in Output output) =>
            _baseBuildResult.ProcessOutput(output);

        public DotNet.BuildResult Create(IStartInfo startInfo, int? exitCode)
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
            foreach (var (message, output) in items)
            {
                _baseBuildResult.ProcessMessage(output, message);
            }
            
            return _baseBuildResult.Create(startInfo, exitCode);
        }
    }
}