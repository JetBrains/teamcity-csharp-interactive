// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System;
    using System.Collections.Generic;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;
    using Pure.DI;

    internal class HierarchicalTeamCityWriter: ITeamCityBlockWriter<IDisposable>, ITeamCityMessageWriter, ITeamCityBuildProblemWriter
    {
        private readonly ITeamCityWriter _teamCityRootWriter;
        private readonly Stack<ITeamCityWriter> _teamCityWriters = new();

        public HierarchicalTeamCityWriter(
            [Tag("Root")] ITeamCityWriter teamCityRootWriter)
        {
            _teamCityRootWriter = teamCityRootWriter;
            _teamCityWriters.Push(teamCityRootWriter);
        }

        public IDisposable OpenBlock(string blockName)
        {
            if (string.IsNullOrWhiteSpace(blockName))
            {
                return CurrentWriter;
            }

            var block = CurrentWriter.OpenBlock(blockName);
            _teamCityWriters.Push(block);
            return Disposable.Create(() =>
            {
                _teamCityWriters.Pop();
                block.Dispose();
            });
        }

        public void WriteMessage(string text) =>
            CurrentWriter.WriteMessage(text);

        public void WriteWarning(string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                CurrentWriter.WriteWarning(text);
            }
        }

        public void WriteError(string text, string? errorDetails)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                CurrentWriter.WriteError(text, errorDetails);
            }
        }
        
        public void WriteBuildProblem(string identity, string description)
        {
            if (!string.IsNullOrWhiteSpace(description))
            {
                CurrentWriter.WriteBuildProblem(identity, description);
            }
        }

        internal ITeamCityWriter CurrentWriter => 
            _teamCityWriters.TryPeek(out var writer) ? writer : _teamCityRootWriter;
    }
}