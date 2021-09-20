// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System;
    using System.Collections.Generic;
    using JetBrains.TeamCity.ServiceMessages;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;
    using Pure.DI;

    internal class HierarchicalTeamCityWriter: ITeamCityWriter
    {
        private readonly Stack<ITeamCityWriter> _teamCityWriters = new();

        public HierarchicalTeamCityWriter(
            [Tag("Root")] ITeamCityWriter currentWriter)
        {
            _teamCityWriters.Push(currentWriter);
        }

        internal ITeamCityWriter CurrentWriter
        {
            get
            {
                lock (_teamCityWriters)
                {
                    return _teamCityWriters.TryPeek(out var writer) ? writer : CurrentWriter;
                }
            }
        }

        private ITeamCityWriter AddWriter(ITeamCityWriter writer)
        {
            lock (_teamCityWriters)
            {
                _teamCityWriters.Push(writer);
                return new NestedWriter(
                    writer,
                    Disposable.Create(() =>
                    {
                        lock (_teamCityWriters)
                        {
                            _teamCityWriters.Pop();
                        }
                    }));
            }
        }
        
        public ITeamCityWriter OpenBlock(string blockName)
        {
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (string.IsNullOrWhiteSpace(blockName))
            {
                return new NestedWriter(CurrentWriter, Disposable.Create());
            }
            
            return AddWriter(CurrentWriter.OpenBlock(blockName));
        }

        public ITeamCityWriter OpenFlow() => AddWriter(CurrentWriter.OpenFlow());

        public void WriteMessage(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            CurrentWriter.WriteMessage(text);
        }

        public void WriteWarning(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            CurrentWriter.WriteWarning(text);
        }

        public void WriteError(string text, string? errorDetails = null)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            CurrentWriter.WriteError(text, errorDetails);
        }

        public ITeamCityTestsSubWriter OpenTestSuite(string suiteName) => CurrentWriter.OpenTestSuite(suiteName);

        public ITeamCityTestWriter OpenTest(string testName) => CurrentWriter.OpenTest(testName);

        public ITeamCityWriter OpenCompilationBlock(string compilerName) => AddWriter(CurrentWriter.OpenCompilationBlock(compilerName));

        public void PublishArtifact(string rules) => CurrentWriter.PublishArtifact(rules);

        public void WriteBuildNumber(string buildNumber) => CurrentWriter.WriteBuildNumber(buildNumber);

        public void WriteBuildProblem(string identity, string description) => CurrentWriter.WriteBuildProblem(identity, description);

        public void WriteBuildParameter(string parameterName, string parameterValue) => CurrentWriter.WriteBuildParameter(parameterName, parameterValue);

        public void WriteBuildStatistics(string statisticsKey, string statisticsValue) => CurrentWriter.WriteBuildStatistics(statisticsKey, statisticsValue);

        public void Dispose()
        {
        }

        public void WriteRawMessage(IServiceMessage message) => CurrentWriter.WriteRawMessage(message);
        
        private class NestedWriter: ITeamCityWriter
        {
            private readonly ITeamCityWriter _writer;
            private readonly IDisposable _resource;

            public NestedWriter(ITeamCityWriter writer, IDisposable resource)
            {
                _writer = writer;
                _resource = resource;
            }

            public ITeamCityWriter OpenBlock(string blockName) => _writer.OpenBlock(blockName);

            public ITeamCityWriter OpenFlow() => _writer.OpenFlow();

            public void WriteMessage(string text) => _writer.WriteMessage(text);

            public void WriteWarning(string text) => _writer.WriteWarning(text);

            public void WriteError(string text, string? errorDetails = null) => _writer.WriteError(text, errorDetails);

            public ITeamCityTestsSubWriter OpenTestSuite(string suiteName) => _writer.OpenTestSuite(suiteName);

            public ITeamCityTestWriter OpenTest(string testName) => _writer.OpenTest(testName);

            public ITeamCityWriter OpenCompilationBlock(string compilerName) => _writer.OpenCompilationBlock(compilerName);

            public void PublishArtifact(string rules) => _writer.PublishArtifact(rules);

            public void WriteBuildNumber(string buildNumber) => _writer.WriteBuildNumber(buildNumber);

            public void WriteBuildProblem(string identity, string description) => _writer.WriteBuildProblem(identity, description);

            public void WriteBuildParameter(string parameterName, string parameterValue) => _writer.WriteBuildParameter(parameterName, parameterValue);

            public void WriteBuildStatistics(string statisticsKey, string statisticsValue) => _writer.WriteBuildStatistics(statisticsKey, statisticsValue);

            public void Dispose()
            {
                _resource.Dispose();
                _writer.Dispose();
            }

            public void WriteRawMessage(IServiceMessage message) => _writer.WriteRawMessage(message);
        }
    }
}